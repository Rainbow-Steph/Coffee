using UnityEngine;

/// <summary>
/// Handles the delivery action - attach to objects that can initiate deliveries
/// This is a simpler alternative if you want a separate delivery trigger button
/// </summary>
public class DeliverAction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the DeliverableManager")]
    [SerializeField] private DeliverableManager deliverableManager;

    [Header("Delivery Settings")]
    [Tooltip("Target to deliver to (if known in advance)")]
    [SerializeField] private DeliveryTarget targetDeliveryTarget;

    [Tooltip("Automatically find nearest delivery target")]
    [SerializeField] private bool autoFindNearestTarget = true;

    [Tooltip("Maximum distance to search for targets")]
    [SerializeField] private float maxSearchDistance = 10f;

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugInfo = true;

    /// <summary>
    /// Attempt to deliver currently held item
    /// Can be called from UI button or other trigger
    /// </summary>
    public void AttemptDelivery()
    {
        // Check if player is holding something
        if (!ClickableObject.IsAnyItemHeld)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[DeliverAction] No item currently held!");
            }
            return;
        }

        ClickableObject heldObject = ClickableObject.GetHeldObject();
        
        if (heldObject == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[DeliverAction] Held object is null!");
            }
            return;
        }

        // Check if it's a delivery item
        if (heldObject.itemType != ItemType.Delivery)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[DeliverAction] Held item {heldObject.gameObject.name} is not a Delivery item! Type: {heldObject.itemType}");
            }
            return;
        }

        // Find target
        DeliveryTarget target = FindDeliveryTarget();

        if (target == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[DeliverAction] No delivery target found!");
            }
            return;
        }

        // Process delivery
        ProcessDelivery(heldObject, target);
    }

    /// <summary>
    /// Find the delivery target to use
    /// </summary>
    private DeliveryTarget FindDeliveryTarget()
    {
        // Use assigned target if available
        if (targetDeliveryTarget != null && !autoFindNearestTarget)
        {
            return targetDeliveryTarget;
        }

        // Find nearest target
        if (autoFindNearestTarget)
        {
            DeliveryTarget[] allTargets = FindObjectsOfType<DeliveryTarget>();
            DeliveryTarget nearest = null;
            float nearestDistance = maxSearchDistance;

            foreach (var target in allTargets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < nearestDistance)
                {
                    nearest = target;
                    nearestDistance = distance;
                }
            }

            if (nearest != null && showDebugInfo)
            {
                Debug.Log($"[DeliverAction] Found nearest target: {nearest.GetTargetName()} at distance {nearestDistance:F2}");
            }

            return nearest;
        }

        return targetDeliveryTarget;
    }

    /// <summary>
    /// Process the delivery
    /// </summary>
    private void ProcessDelivery(ClickableObject deliveredObject, DeliveryTarget target)
    {
        if (deliverableManager == null)
        {
            Debug.LogError("[DeliverAction] DeliverableManager not assigned!");
            return;
        }

        // Get target configuration
        var targetConfig = deliverableManager.FindTargetByName(target.GetTargetName());
        
        if (targetConfig == null)
        {
            Debug.LogError($"[DeliverAction] Target '{target.GetTargetName()}' not found in DeliverableManager!");
            return;
        }

        // Check satisfaction level
        SatisfactionLevel satisfaction = targetConfig.GetSatisfactionLevel(deliveredObject.gameObject);

        if (showDebugInfo)
        {
            Debug.Log($"[DeliverAction] Delivering {deliveredObject.gameObject.name} to {target.GetTargetName()}. Satisfaction: {satisfaction}");
        }

        // Let the target handle the actual delivery
        // (This triggers the DeliveryTarget's OnTriggerEnter logic programmatically)
        
        if (satisfaction != SatisfactionLevel.None)
        {
            // Manual delivery success
            SpawnRewards(satisfaction, target.transform.position);
            Destroy(deliveredObject.gameObject);

            if (showDebugInfo)
            {
                Debug.Log("[DeliverAction] ? Delivery successful!");
            }
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning("[DeliverAction] ? Delivery failed - item not accepted by target!");
            }
        }
    }

    /// <summary>
    /// Spawn rewards at target location
    /// </summary>
    private void SpawnRewards(SatisfactionLevel satisfaction, Vector3 position)
    {
        if (deliverableManager == null)
            return;

        var rewardRanges = deliverableManager.GetRewardRanges(satisfaction);
        
        if (rewardRanges == null)
        {
            Debug.LogWarning($"[DeliverAction] No reward ranges configured for {satisfaction} satisfaction!");
            return;
        }

        // Spawn rocks using shared prefab
        int rockCount = rewardRanges.rocksRange.GetRandomAmount();
        for (int i = 0; i < rockCount; i++)
        {
            SpawnRewardItem(deliverableManager.rockPrefab, position, "Rock");
        }

        // Spawn coins using shared prefab
        int coinCount = rewardRanges.coinsRange.GetRandomAmount();
        for (int i = 0; i < coinCount; i++)
        {
            SpawnRewardItem(deliverableManager.coinPrefab, position, "Coin");
        }

        // Spawn bills using shared prefab
        int billCount = rewardRanges.billsRange.GetRandomAmount();
        for (int i = 0; i < billCount; i++)
        {
            SpawnRewardItem(deliverableManager.billPrefab, position, "Bill");
        }

        if (showDebugInfo)
        {
            Debug.Log($"[DeliverAction] Spawned rewards: {rockCount} rocks, {coinCount} coins, {billCount} bills");
        }
    }

    /// <summary>
    /// Spawn a single reward item
    /// </summary>
    private void SpawnRewardItem(GameObject prefab, Vector3 position, string itemName)
    {
        if (prefab == null)
            return;

        GameObject reward = Instantiate(prefab, position + Vector3.up, Quaternion.identity);
        reward.name = itemName;

        // Apply upward force
        Rigidbody rb = reward.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 launchDirection = Vector3.up + Random.insideUnitSphere * 0.3f;
            rb.velocity = launchDirection.normalized * 3f;
        }
    }

    /// <summary>
    /// Check if delivery is possible
    /// </summary>
    public bool CanDeliver()
    {
        if (!ClickableObject.IsAnyItemHeld)
            return false;

        ClickableObject heldObject = ClickableObject.GetHeldObject();
        if (heldObject == null)
            return false;

        if (heldObject.itemType != ItemType.Delivery)
            return false;

        DeliveryTarget target = FindDeliveryTarget();
        return target != null;
    }

    /// <summary>
    /// Get the nearest delivery target
    /// </summary>
    public DeliveryTarget GetNearestTarget()
    {
        return FindDeliveryTarget();
    }

    private void Start()
    {
        // Auto-find deliverable manager if not assigned
        if (deliverableManager == null)
        {
            deliverableManager = Resources.Load<DeliverableManager>("DeliverableManager");
            
            if (deliverableManager != null && showDebugInfo)
            {
                Debug.Log("[DeliverAction] Auto-found DeliverableManager");
            }
        }

        if (deliverableManager == null)
        {
            Debug.LogError("[DeliverAction] DeliverableManager not found! Please assign it or place it in a Resources folder.");
        }
    }

    // Visual helper in editor
    private void OnDrawGizmosSelected()
    {
        if (autoFindNearestTarget)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, maxSearchDistance);
        }

        if (targetDeliveryTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetDeliveryTarget.transform.position);
        }
    }
}

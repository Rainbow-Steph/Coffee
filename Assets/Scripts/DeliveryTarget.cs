using UnityEngine;

/// <summary>
/// Component to mark GameObjects as delivery targets
/// Attach to objects that can receive deliveries
/// </summary>
[RequireComponent(typeof(Collider))]
public class DeliveryTarget : MonoBehaviour
{
    [Header("Target Configuration")]
    [Tooltip("Name of this delivery target (e.g., 'Black Target', 'Red Target', 'Blue Target')")]
    [SerializeField] private string targetName = "Black Target";

    [Header("References")]
    [Tooltip("Reference to the DeliverableManager")]
    [SerializeField] private DeliverableManager deliverableManager;

    [Header("Delivery Collider")]
    [Tooltip("Child object with collider for detecting deliveries (auto-finds 'Delivery Collider')")]
    [SerializeField] private Transform deliveryCollider;

    [Header("Spawn Settings")]
    [Tooltip("Where to spawn rewards (defaults to this transform)")]
    [SerializeField] private Transform rewardSpawnPoint;

    [Tooltip("Launch force for spawned rewards")]
    [SerializeField] private float rewardLaunchForce = 3f;

    [Tooltip("Add random variation to reward spawn")]
    [SerializeField] private bool addRandomVariation = true;

    [Tooltip("Maximum random angle for reward spawn")]
    [Range(0f, 45f)]
    [SerializeField] private float maxRandomAngle = 15f;

    [Header("Visual Feedback")]
    [Tooltip("Particle effect on successful delivery")]
    [SerializeField] private ParticleSystem deliverySuccessParticles;

    [Tooltip("Particle effect on failed delivery")]
    [SerializeField] private ParticleSystem deliveryFailedParticles;

    [Header("Audio (Optional)")]
    [Tooltip("Audio source for delivery sounds")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Sound on successful delivery")]
    [SerializeField] private AudioClip successSound;

    [Tooltip("Sound on failed delivery")]
    [SerializeField] private AudioClip failSound;

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugInfo = true;

    private Collider triggerCollider;
    private DeliverableManager.DeliveryTarget targetConfig;

    private void Start()
    {
        // Auto-find delivery collider if not assigned
        if (deliveryCollider == null)
        {
            Transform found = transform.Find("Delivery Collider");
            if (found != null)
            {
                deliveryCollider = found;
                if (showDebugInfo)
                {
                    Debug.Log($"[DeliveryTarget] Auto-found 'Delivery Collider' child on {gameObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($"[DeliveryTarget] No 'Delivery Collider' child found on {gameObject.name}! Deliveries won't work!");
            }
        }

        // Ensure delivery collider is a trigger
        if (deliveryCollider != null)
        {
            triggerCollider = deliveryCollider.GetComponent<Collider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
                
                if (showDebugInfo)
                {
                    Debug.Log($"[DeliveryTarget] {gameObject.name} delivery collider set as trigger");
                }
            }
            else
            {
                Debug.LogError($"[DeliveryTarget] Delivery Collider on {gameObject.name} has no Collider component!");
            }
        }

        // Auto-find deliverable manager if not assigned
        if (deliverableManager == null)
        {
            // Try to find in Resources folder
            deliverableManager = Resources.Load<DeliverableManager>("DeliverableManager");
            
            if (deliverableManager == null)
            {
                // Try to find any instance
                deliverableManager = FindObjectOfType<DeliverableManager>();
            }

            if (deliverableManager != null && showDebugInfo)
            {
                Debug.Log($"[DeliveryTarget] Auto-found DeliverableManager");
            }
        }

        // Use self as reward spawn point if not assigned
        if (rewardSpawnPoint == null)
        {
            rewardSpawnPoint = transform;
        }

        // Get audio source if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Load target configuration
        LoadTargetConfiguration();

        // Validate setup
        ValidateSetup();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only process if this is the delivery collider
        if (deliveryCollider != null && other.transform != deliveryCollider)
            return;

        // Check if the entering object is a held ClickableObject
        ClickableObject clickableObject = other.GetComponent<ClickableObject>();
        
        if (clickableObject == null)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[DeliveryTarget] Object {other.gameObject.name} has no ClickableObject component");
            }
            return;
        }

        // Only process if the object is currently being held
        if (!ClickableObject.IsAnyItemHeld || ClickableObject.GetHeldObject() != clickableObject)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[DeliveryTarget] Object {other.gameObject.name} is not being held");
            }
            return;
        }

        // Only process delivery items
        if (clickableObject.itemType != ItemType.Delivery)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[DeliveryTarget] Object {other.gameObject.name} is not a Delivery item! Type: {clickableObject.itemType}");
            }
            return;
        }

        // Process the delivery
        ProcessDelivery(clickableObject);
    }

    /// <summary>
    /// Process a delivery attempt
    /// </summary>
    private void ProcessDelivery(ClickableObject deliveredObject)
    {
        if (deliverableManager == null || targetConfig == null)
        {
            Debug.LogError($"[DeliveryTarget] Cannot process delivery - missing manager or config!");
            return;
        }

        // Get the prefab reference from the delivered object
        GameObject deliveredPrefab = deliveredObject.gameObject;

        // Determine satisfaction level
        SatisfactionLevel satisfaction = targetConfig.GetSatisfactionLevel(deliveredPrefab);

        if (showDebugInfo)
        {
            Debug.Log($"[DeliveryTarget] Delivered {deliveredPrefab.name} to {targetName}. Satisfaction: {satisfaction}");
        }

        // Handle delivery result
        if (satisfaction != SatisfactionLevel.None)
        {
            HandleSuccessfulDelivery(deliveredObject, satisfaction);
        }
        else
        {
            HandleFailedDelivery(deliveredObject);
        }
    }

    /// <summary>
    /// Handle successful delivery
    /// </summary>
    private void HandleSuccessfulDelivery(ClickableObject deliveredObject, SatisfactionLevel satisfaction)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[DeliveryTarget] ? Successful delivery! Satisfaction: {satisfaction}");
        }

        // Spawn rewards
        SpawnRewards(satisfaction);

        // Play success visual/audio feedback
        if (deliverySuccessParticles != null)
        {
            deliverySuccessParticles.Play();
        }

        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        // Destroy the delivered object
        Destroy(deliveredObject.gameObject);
    }

    /// <summary>
    /// Handle failed delivery
    /// </summary>
    private void HandleFailedDelivery(ClickableObject deliveredObject)
    {
        if (showDebugInfo)
        {
            Debug.LogWarning($"[DeliveryTarget] ? Failed delivery! {deliveredObject.gameObject.name} is not accepted by {targetName}");
        }

        // Play failure visual/audio feedback
        if (deliveryFailedParticles != null)
        {
            deliveryFailedParticles.Play();
        }

        if (audioSource != null && failSound != null)
        {
            audioSource.PlayOneShot(failSound);
        }

        // Don't destroy the object - let player try elsewhere
    }

    /// <summary>
    /// Spawn rewards based on satisfaction level
    /// </summary>
    private void SpawnRewards(SatisfactionLevel satisfaction)
    {
        DeliverableManager.SatisfactionRewardRanges rewardRanges = deliverableManager.GetRewardRanges(satisfaction);
        
        if (rewardRanges == null)
        {
            Debug.LogWarning($"[DeliveryTarget] No reward ranges configured for {satisfaction} satisfaction!");
            return;
        }

        // Spawn rocks using shared prefab
        int rockCount = rewardRanges.rocksRange.GetRandomAmount();
        for (int i = 0; i < rockCount; i++)
        {
            SpawnRewardItem(deliverableManager.rockPrefab, "Rock");
        }

        // Spawn coins using shared prefab
        int coinCount = rewardRanges.coinsRange.GetRandomAmount();
        for (int i = 0; i < coinCount; i++)
        {
            SpawnRewardItem(deliverableManager.coinPrefab, "Coin");
        }

        // Spawn bills using shared prefab
        int billCount = rewardRanges.billsRange.GetRandomAmount();
        for (int i = 0; i < billCount; i++)
        {
            SpawnRewardItem(deliverableManager.billPrefab, "Bill");
        }

        if (showDebugInfo)
        {
            Debug.Log($"[DeliveryTarget] Spawned rewards: {rockCount} rocks, {coinCount} coins, {billCount} bills");
        }
    }

    /// <summary>
    /// Spawn a single reward item
    /// </summary>
    private void SpawnRewardItem(GameObject prefab, string itemName)
    {
        if (prefab == null)
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"[DeliveryTarget] {itemName} prefab not assigned, skipping");
            }
            return;
        }

        // Calculate spawn position and rotation
        Vector3 spawnPosition = rewardSpawnPoint.position;
        Quaternion spawnRotation = rewardSpawnPoint.rotation;

        // Instantiate the reward
        GameObject reward = Instantiate(prefab, spawnPosition, spawnRotation);
        reward.name = itemName;

        // Apply launch force if rigidbody exists
        Rigidbody rb = reward.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate launch direction
            Vector3 launchDirection = Vector3.up + rewardSpawnPoint.forward * 0.3f;
            launchDirection.Normalize();

            // Add random variation if enabled
            if (addRandomVariation && maxRandomAngle > 0)
            {
                float randomX = Random.Range(-maxRandomAngle, maxRandomAngle);
                float randomY = Random.Range(-maxRandomAngle, maxRandomAngle);
                Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);
                launchDirection = randomRotation * launchDirection;
            }

            // Apply velocity
            rb.velocity = launchDirection * rewardLaunchForce;

            // Add slight random angular velocity
            if (addRandomVariation)
            {
                rb.angularVelocity = new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f),
                    Random.Range(-2f, 2f)
                );
            }
        }
    }

    /// <summary>
    /// Load target configuration from deliverable manager
    /// </summary>
    private void LoadTargetConfiguration()
    {
        if (deliverableManager == null)
        {
            Debug.LogError($"[DeliveryTarget] DeliverableManager not assigned on {gameObject.name}!");
            return;
        }

        targetConfig = deliverableManager.FindTargetByName(targetName);

        if (targetConfig == null)
        {
            Debug.LogError($"[DeliveryTarget] Target '{targetName}' not found in DeliverableManager!");
        }
        else if (showDebugInfo)
        {
            Debug.Log($"[DeliveryTarget] Loaded configuration for '{targetName}'");
        }
    }

    /// <summary>
    /// Validate setup
    /// </summary>
    private bool ValidateSetup()
    {
        bool valid = true;

        if (deliveryCollider == null)
        {
            Debug.LogError($"[DeliveryTarget] No delivery collider assigned on {gameObject.name}!");
            valid = false;
        }

        if (deliverableManager == null)
        {
            Debug.LogError($"[DeliveryTarget] DeliverableManager not assigned on {gameObject.name}!");
            valid = false;
        }

        if (targetConfig == null)
        {
            Debug.LogError($"[DeliveryTarget] Target configuration not loaded for '{targetName}'!");
            valid = false;
        }

        return valid;
    }

    /// <summary>
    /// Get the target name
    /// </summary>
    public string GetTargetName()
    {
        return targetName;
    }

    /// <summary>
    /// Check if an item can be delivered to this target
    /// </summary>
    public bool CanAcceptItem(GameObject item)
    {
        if (targetConfig == null)
            return false;

        return targetConfig.GetSatisfactionLevel(item) != SatisfactionLevel.None;
    }

    // Visual helper in editor
    private void OnDrawGizmos()
    {
        if (deliveryCollider != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.matrix = deliveryCollider.localToWorldMatrix;
            
            Collider col = deliveryCollider.GetComponent<Collider>();
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
        }

        // Draw spawn point
        if (rewardSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(rewardSpawnPoint.position, 0.2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw delivery zone more prominently when selected
        if (deliveryCollider != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.matrix = deliveryCollider.localToWorldMatrix;
            
            Collider col = deliveryCollider.GetComponent<Collider>();
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
    }
}

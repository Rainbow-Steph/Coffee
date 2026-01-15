using UnityEngine;

/// <summary>
/// Component to mark GameObjects as delivery targets
/// Attach to objects that can receive deliveries
/// Works by detecting when ItemType.Delivery objects enter ANY trigger collider on this GameObject
/// </summary>
public class DeliveryTarget : MonoBehaviour
{
    [Header("Target Configuration")]
    [Tooltip("Name of this delivery target (e.g., 'Black Target', 'Red Target', 'Blue Target')")]
    [SerializeField] private string targetName = "Black Target";

    [Header("References")]
    [Tooltip("Reference to the DeliverableManager")]
    [SerializeField] private DeliverableManager deliverableManager;

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

    private DeliverableManager.DeliveryTarget targetConfig;

    private void Start()
    {
        // Auto-find deliverable manager if not assigned
        if (deliverableManager == null)
        {
            deliverableManager = Resources.Load<DeliverableManager>("DeliverableManager");
            
            if (deliverableManager == null)
            {
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
        // Get the ClickableObject from what entered the trigger
        ClickableObject clickableObject = other.GetComponent<ClickableObject>();
        
        // Determine if ProcessDelivery will be called (check all conditions upfront)
        bool willProcessDelivery = false;
        string skipReason = "";
        
        // Check 1: Has ClickableObject?
        bool hasClickableObject = (clickableObject != null);
        
        // Check 2: Is Delivery type?
        bool isDeliveryType = hasClickableObject && clickableObject.itemType == ItemType.Delivery;
        
        // Check 3: Is being held?
        bool isBeingHeld = hasClickableObject && ClickableObject.IsAnyItemHeld && ClickableObject.GetHeldObject() == clickableObject;
        
        // Get satisfaction level early (if possible)
        SatisfactionLevel satisfactionLevel = SatisfactionLevel.None;
        if (targetConfig != null && hasClickableObject)
        {
            satisfactionLevel = targetConfig.GetSatisfactionLevel(clickableObject.gameObject);
        }
        
        // Determine if all checks pass
        if (hasClickableObject && isDeliveryType && isBeingHeld)
        {
            willProcessDelivery = true;
        }
        else
        {
            // Determine skip reason
            if (!hasClickableObject)
                skipReason = "No ClickableObject component";
            else if (!isDeliveryType)
                skipReason = "Not Delivery item type";
            else if (!isBeingHeld)
                skipReason = "Not being held";
        }
        
        // Debug: Show all trigger events with comprehensive information
        if (showDebugInfo)
        {
            Debug.Log($"[DeliveryTarget - {targetName}] ??? TRIGGER ENTERED ???\n" +
                     $"  Object: {other.gameObject.name}\n" +
                     $"  Layer: {LayerMask.LayerToName(other.gameObject.layer)}\n" +
                     $"  Has Rigidbody: {(other.attachedRigidbody != null ? "Yes" : "No")}\n" +
                     $"  Collider Type: {other.GetType().Name}\n" +
                     $"  Is ClickableObject: {(hasClickableObject ? "YES" : "NO")}\n" +
                     $"  Item Type: {(hasClickableObject ? clickableObject.itemType.ToString() : "N/A")}\n" +
                     $"  Is Being Held: {(isBeingHeld ? "YES" : "NO")}\n" +
                     $"  Satisfaction Level: {satisfactionLevel}\n" +
                     $"  ProcessDelivery() Called: {(willProcessDelivery ? "YES" : "NO")}" +
                     (willProcessDelivery ? "" : $"\n  Skip Reason: {skipReason}"));
        }

        // Validate checks in order
        if (!hasClickableObject)
        {
            return;
        }

        // Only process delivery items
        if (!isDeliveryType)
        {
            return;
        }

        // Only process if the object is currently being held
        if (!isBeingHeld)
        {
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
}

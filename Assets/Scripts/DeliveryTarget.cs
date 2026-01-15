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

    [Header("Animation Settings")]
    [Tooltip("Direction to move the target (normalized automatically)")]
    [SerializeField] private Vector3 moveDirection = Vector3.back;
    
    [Tooltip("How far to move the target")]
    [SerializeField] private float moveDistance = 0.5f;
    
    [Tooltip("How fast to move the target")]
    [SerializeField] private float moveSpeed = 2f;
    
    [Tooltip("Delay before spawning rewards after animation")]
    [SerializeField] private float rewardSpawnDelay = 1f;
    
    [Tooltip("Make target disappear after spawning rewards (no return animation)")]
    [SerializeField] private bool disappearAfterRewards = true;

    [Header("Spawn Settings")]
    [Tooltip("Where to spawn rewards (defaults to this transform)")]
    [SerializeField] private Transform rewardSpawnPoint;
    
    [Tooltip("Base direction for reward launch (normalized automatically)")]
    [SerializeField] private Vector3 rewardLaunchDirection = Vector3.up;
    
    [Tooltip("Additional forward component for reward direction")]
    [Range(0f, 1f)]
    [SerializeField] private float rewardForwardInfluence = 0.3f;

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
    private ClickableObject pendingDelivery = null;
    private Vector3 originalPosition;
    private bool isProcessingDelivery = false;

    private void Start()
    {
        // Store original position for animation
        originalPosition = transform.position;
        
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
        // Don't process if already handling a delivery
        if (isProcessingDelivery)
            return;
            
        // Get the ClickableObject from what entered the trigger
        ClickableObject clickableObject = other.GetComponent<ClickableObject>();
        
        // Determine if we should track this delivery
        bool willTrackDelivery = false;
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
            willTrackDelivery = true;
            pendingDelivery = clickableObject; // Track this for release detection
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
                     $"  Tracking for Release: {(willTrackDelivery ? "YES" : "NO")}" +
                     (willTrackDelivery ? "" : $"\n  Skip Reason: {skipReason}"));
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        // Check if we're tracking a pending delivery and player has released it
        if (pendingDelivery != null && !isProcessingDelivery)
        {
            ClickableObject clickableObject = other.GetComponent<ClickableObject>();
            
            // Check if this is our pending delivery and it's no longer being held
            if (clickableObject == pendingDelivery)
            {
                bool isStillHeld = ClickableObject.IsAnyItemHeld && ClickableObject.GetHeldObject() == clickableObject;
                
                if (!isStillHeld)
                {
                    // Player released the coffee in the delivery zone!
                    if (showDebugInfo)
                    {
                        Debug.Log($"[DeliveryTarget - {targetName}] Coffee released in delivery zone! Processing...");
                    }
                    
                    StartCoroutine(ProcessDeliverySequence(pendingDelivery));
                    pendingDelivery = null;
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // If the pending delivery leaves the zone, clear it
        if (pendingDelivery != null)
        {
            ClickableObject clickableObject = other.GetComponent<ClickableObject>();
            if (clickableObject == pendingDelivery)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"[DeliveryTarget - {targetName}] Pending delivery left the zone");
                }
                pendingDelivery = null;
            }
        }
    }

    /// <summary>
    /// Process delivery sequence with animation
    /// </summary>
    private System.Collections.IEnumerator ProcessDeliverySequence(ClickableObject deliveredObject)
    {
        isProcessingDelivery = true;
        
        if (deliverableManager == null || targetConfig == null)
        {
            Debug.LogError($"[DeliveryTarget] Cannot process delivery - missing manager or config!");
            isProcessingDelivery = false;
            yield break;
        }

        // Get the prefab reference from the delivered object
        GameObject deliveredPrefab = deliveredObject.gameObject;

        // Determine satisfaction level
        SatisfactionLevel satisfaction = targetConfig.GetSatisfactionLevel(deliveredPrefab);

        // Handle delivery result
        if (satisfaction != SatisfactionLevel.None)
        {
            yield return StartCoroutine(HandleSuccessfulDeliverySequence(deliveredObject, satisfaction));
        }
        else
        {
            HandleFailedDelivery(deliveredObject);
        }
        
        isProcessingDelivery = false;
    }

    /// <summary>
    /// Handle successful delivery with animation and consolidated debug
    /// </summary>
    private System.Collections.IEnumerator HandleSuccessfulDeliverySequence(ClickableObject deliveredObject, SatisfactionLevel satisfaction)
    {
        // Calculate rewards BEFORE animation for debug message
        DeliverableManager.SatisfactionRewardRanges rewardRanges = deliverableManager.GetRewardRanges(satisfaction);
        
        int rockCount = 0;
        int coinCount = 0;
        int billCount = 0;
        
        if (rewardRanges != null)
        {
            rockCount = rewardRanges.rocksRange.GetRandomAmount();
            coinCount = rewardRanges.coinsRange.GetRandomAmount();
            billCount = rewardRanges.billsRange.GetRandomAmount();
        }
        
        // CONSOLIDATED DEBUG MESSAGE
        if (showDebugInfo)
        {
            Debug.Log($"[DeliveryTarget - {targetName}] ??? PROCESSING DELIVERY ???\n" +
                     $"  Delivered: {deliveredObject.gameObject.name}\n" +
                     $"  Satisfaction: {satisfaction}\n" +
                     $"  Result: ? Successful Delivery!\n" +
                     $"  Rewards: {rockCount} rocks, {coinCount} coins, {billCount} bills\n" +
                     $"  Animation: Move {moveDirection.normalized} by {moveDistance}m\n" +
                     $"  Disappear After: {(disappearAfterRewards ? "YES" : "NO")}");
        }
        
        // Play success audio immediately
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
        
        // Destroy the delivered object
        Destroy(deliveredObject.gameObject);
        
        // Animate target in configured direction
        Vector3 normalizedDirection = moveDirection.normalized;
        Vector3 targetPosition = originalPosition + (normalizedDirection * moveDistance);
        float moveTime = moveDistance / moveSpeed;
        float elapsed = 0f;
        
        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
        
        // Wait before spawning rewards
        yield return new WaitForSeconds(rewardSpawnDelay);
        
        // Spawn rewards
        for (int i = 0; i < rockCount; i++)
        {
            SpawnRewardItem(deliverableManager.rockPrefab, "Rock");
        }
        
        for (int i = 0; i < coinCount; i++)
        {
            SpawnRewardItem(deliverableManager.coinPrefab, "Coin");
        }
        
        for (int i = 0; i < billCount; i++)
        {
            SpawnRewardItem(deliverableManager.billPrefab, "Bill");
        }
        
        // Play success visual feedback
        if (deliverySuccessParticles != null)
        {
            deliverySuccessParticles.Play();
        }
        
        // Handle post-reward behavior
        if (disappearAfterRewards)
        {
            // Disappear: disable renderers and colliders
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"[DeliveryTarget - {targetName}] Target disappeared after spawning rewards");
            }
        }
        else
        {
            // Return to original position
            elapsed = 0f;
            Vector3 currentPos = transform.position;
            
            while (elapsed < moveTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveTime;
                transform.position = Vector3.Lerp(currentPos, originalPosition, t);
                yield return null;
            }
            
            transform.position = originalPosition;
            
            if (showDebugInfo)
            {
                Debug.Log($"[DeliveryTarget - {targetName}] Target returned to original position");
            }
        }
    }

    /// <summary>
    /// Handle failed delivery
    /// </summary>
    private void HandleFailedDelivery(ClickableObject deliveredObject)
    {
        if (showDebugInfo)
        {
            Debug.LogWarning($"[DeliveryTarget - {targetName}] ? Failed delivery! {deliveredObject.gameObject.name} is not accepted by {targetName}");
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
            // Calculate launch direction using configurable base direction and forward influence
            Vector3 baseDirection = rewardLaunchDirection.normalized;
            Vector3 forwardComponent = rewardSpawnPoint.forward * rewardForwardInfluence;
            Vector3 launchDirection = (baseDirection + forwardComponent).normalized;

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

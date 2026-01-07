using UnityEngine;
using System.Collections;

/// <summary>
/// Supply spawner component - handles spawning resources when clicked
/// Attach to spawner GameObjects (one for water, one for coffee, one for extras)
/// </summary>
[RequireComponent(typeof(ClickableObject))]
public class Supply : MonoBehaviour
{
    [Header("Spawner Configuration")]
    [Tooltip("Type of resource this spawner provides")]
    [SerializeField] private ResourceType resourceType = ResourceType.Water;
    
 [Tooltip("Supply configuration ScriptableObject")]
    [SerializeField] private SupplyConfig supplyConfig;

    [Header("References")]
    [Tooltip("Inventory manager to check/deduct money")]
    [SerializeField] private InventoryManager inventoryManager;
    
    [Tooltip("System messages for dialogue")]
    [SerializeField] private SystemMessages systemMessages;
    
    [Tooltip("Dialogue manager for showing messages")]
    [SerializeField] private DialogueManager dialogueManager;

  [Header("Spawn Point")]
    [Tooltip("Transform where items will spawn from")]
    [SerializeField] private Transform spawnPoint;

    [Header("Animation (Optional)")]
    [Tooltip("Animator component for spawn/fail animations")]
    [SerializeField] private Animator animator;
    
 [Tooltip("Animation trigger name for successful spawn")]
    [SerializeField] private string spawnAnimationTrigger = "Spawn";
    
    [Tooltip("Animation trigger name for failed spawn")]
    [SerializeField] private string failedAnimationTrigger = "Failed";

    [Header("Audio (Optional)")]
    [Tooltip("Audio source for playing sounds")]
    [SerializeField] private AudioSource audioSource;

    [Header("Visual Feedback (Optional)")]
    [Tooltip("Particle effect on successful spawn")]
    [SerializeField] private ParticleSystem spawnParticles;
    
    [Tooltip("Particle effect on failed spawn")]
    [SerializeField] private ParticleSystem failedParticles;

  [Header("Dialogue Messages")]
    [Tooltip("Dialogue to show when not enough money")]
    [SerializeField] private DialogueNodeSO notEnoughMoneyDialogue;

 [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugInfo = true;

    private ClickableObject clickableObject;
    private SupplyConfig.ResourceTypeConfig currentConfig;
    private bool isSpawning = false;

    void Start()
    {
        // Get ClickableObject component
   clickableObject = GetComponent<ClickableObject>();
        
        // Subscribe to click event
        if (clickableObject != null)
        {
    clickableObject.onClickEvent.AddListener(OnSpawnerClicked);
        }
 else
        {
            Debug.LogError($"[Supply] No ClickableObject found on {gameObject.name}!");
   }

        // Auto-find references if not assigned
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
 }

        if (systemMessages == null)
        {
   systemMessages = FindObjectOfType<SystemMessages>();
        }

        if (dialogueManager == null)
        {
         dialogueManager = FindObjectOfType<DialogueManager>();
     }

        if (audioSource == null)
 {
   audioSource = GetComponent<AudioSource>();
        }

if (animator == null)
 {
      animator = GetComponent<Animator>();
      }

        // Use self as spawn point if not assigned
        if (spawnPoint == null)
        {
    spawnPoint = transform;
         if (showDebugInfo)
     {
   Debug.LogWarning($"[Supply] No spawn point assigned on {gameObject.name}, using self");
            }
   }

        // Load configuration for this resource type
      if (supplyConfig != null)
     {
       currentConfig = supplyConfig.GetConfig(resourceType);
     
  if (showDebugInfo)
    {
            Debug.Log($"[Supply] {gameObject.name} initialized as {resourceType} spawner. Cost: ${currentConfig.costPerSpawn}");
   }
        }
        else
    {
    Debug.LogError($"[Supply] SupplyConfig not assigned on {gameObject.name}!");
        }

        ValidateReferences();
    }

    /// <summary>
    /// Called when the spawner is clicked
    /// </summary>
    public void OnSpawnerClicked()
    {
    if (showDebugInfo)
        {
 Debug.Log($"[Supply] {resourceType} spawner clicked!");
  }

        // Don't allow clicking while spawning
        if (isSpawning)
   {
       if (showDebugInfo)
    {
             Debug.Log($"[Supply] Already spawning, ignoring click");
            }
         return;
        }

        // Check if we have valid configuration
      if (supplyConfig == null || currentConfig == null)
    {
     Debug.LogError($"[Supply] Supply configuration not set!");
         return;
        }

        // Check if player has enough money
        int cost = currentConfig.costPerSpawn;
        int currentMoney = inventoryManager != null ? inventoryManager.Money : 0;

      if (showDebugInfo)
        {
    Debug.Log($"[Supply] Cost: ${cost}, Current money: ${currentMoney}");
        }

        if (currentMoney < cost)
    {
     // Not enough money - play failed animation and dialogue
      HandleFailedSpawn(currentMoney, cost);
   }
   else
        {
    // Enough money - spawn items
         HandleSuccessfulSpawn(cost);
        }
    }

    /// <summary>
    /// Handle failed spawn attempt (not enough money)
    /// </summary>
    private void HandleFailedSpawn(int currentMoney, int cost)
    {
      if (showDebugInfo)
        {
            Debug.LogWarning($"[Supply] Not enough money! Need ${cost}, have ${currentMoney}");
        }

        // Play failed animation
        if (animator != null && !string.IsNullOrEmpty(failedAnimationTrigger))
        {
            animator.SetTrigger(failedAnimationTrigger);
        }

        // Play failed sound
      if (audioSource != null && supplyConfig.failedSound != null)
 {
  audioSource.PlayOneShot(supplyConfig.failedSound);
  }

        // Play failed particles
  if (failedParticles != null)
      {
       failedParticles.Play();
        }

        // Show dialogue
     if (dialogueManager != null && notEnoughMoneyDialogue != null)
        {
     dialogueManager.StartDialogue(notEnoughMoneyDialogue);
     }
        else if (showDebugInfo)
        {
  Debug.LogWarning($"[Supply] No dialogue assigned for failed spawn!");
        }
    }

    /// <summary>
    /// Handle successful spawn
    /// </summary>
    private void HandleSuccessfulSpawn(int cost)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[Supply] Spawning {resourceType} items!");
     }

        // Deduct money
      if (inventoryManager != null)
        {
     inventoryManager.Money -= cost;
            
     if (showDebugInfo)
            {
        Debug.Log($"[Supply] Deducted ${cost}. Remaining: ${inventoryManager.Money}");
            }
    }

      // Play spawn animation
        if (animator != null && !string.IsNullOrEmpty(spawnAnimationTrigger))
      {
            animator.SetTrigger(spawnAnimationTrigger);
        }

        // Play spawn sound
        if (audioSource != null && supplyConfig.spawnSuccessSound != null)
 {
       audioSource.PlayOneShot(supplyConfig.spawnSuccessSound);
        }

    // Play spawn particles
        if (spawnParticles != null)
        {
         spawnParticles.Play();
        }

        // Start spawning items
        StartCoroutine(SpawnItemsCoroutine());
    }

    /// <summary>
    /// Coroutine to spawn multiple items with delay
    /// </summary>
    private IEnumerator SpawnItemsCoroutine()
    {
        isSpawning = true;

     // Determine how many items to spawn
        int spawnCount = Random.Range(currentConfig.minSpawnCount, currentConfig.maxSpawnCount + 1);
        
        if (showDebugInfo)
  {
            Debug.Log($"[Supply] Spawning {spawnCount} {resourceType} items");
        }

        // Check if there are available items
        int availableCount = currentConfig.GetAvailableItemCount();
        if (availableCount == 0)
      {
            Debug.LogError($"[Supply] No available items to spawn for {resourceType}!");
        isSpawning = false;
         yield break;
        }

        // Spawn items one by one
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnSingleItem();
            
   // Wait before spawning next item
            if (i < spawnCount - 1)
            {
        yield return new WaitForSeconds(currentConfig.spawnDelay);
        }
        }

        isSpawning = false;
        
        if (showDebugInfo)
        {
            Debug.Log($"[Supply] Finished spawning {spawnCount} items");
        }
    }

    /// <summary>
    /// Spawn a single item
    /// </summary>
    private void SpawnSingleItem()
    {
        // Get random item from configuration
    GameObject prefab = currentConfig.GetRandomAvailableItem();
        
        if (prefab == null)
        {
    Debug.LogError($"[Supply] Failed to get random item for {resourceType}!");
       return;
     }

     // Spawn the item
        Vector3 spawnPosition = spawnPoint.position;
   Quaternion spawnRotation = spawnPoint.rotation;
        
  GameObject spawnedItem = Instantiate(prefab, spawnPosition, spawnRotation);
        spawnedItem.name = prefab.name; // Remove "(Clone)" suffix

 if (showDebugInfo)
 {
            Debug.Log($"[Supply] Spawned {spawnedItem.name} at {spawnPosition}");
        }

        // Apply launch force
        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
   if (rb != null)
        {
            // Calculate launch direction
      Vector3 launchDir = spawnPoint.TransformDirection(currentConfig.launchDirection.normalized);
        
     // Add random variation if enabled
            if (currentConfig.addRandomVariation && currentConfig.maxRandomAngle > 0)
      {
   float randomX = Random.Range(-currentConfig.maxRandomAngle, currentConfig.maxRandomAngle);
      float randomY = Random.Range(-currentConfig.maxRandomAngle, currentConfig.maxRandomAngle);
      Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);
     launchDir = randomRotation * launchDir;
    }

      // Apply velocity
            rb.velocity = launchDir * currentConfig.launchForce;
       
  // Add slight random angular velocity for natural movement
     if (currentConfig.addRandomVariation)
 {
                rb.angularVelocity = new Vector3(
       Random.Range(-2f, 2f),
         Random.Range(-2f, 2f),
 Random.Range(-2f, 2f)
       );
            }

            if (showDebugInfo)
            {
    Debug.Log($"[Supply] Launched {spawnedItem.name} with velocity {rb.velocity}");
      }
        }
        else if (showDebugInfo)
        {
     Debug.LogWarning($"[Supply] Spawned item {spawnedItem.name} has no Rigidbody - cannot apply launch force!");
 }
    }

    /// <summary>
    /// Enable/disable a specific item by name
    /// </summary>
    public void SetItemAvailability(string itemName, bool available)
    {
        if (currentConfig != null)
        {
            bool success = currentConfig.SetItemAvailability(itemName, available);
 
     if (showDebugInfo)
       {
       if (success)
       {
     Debug.Log($"[Supply] Set {itemName} availability to {available}");
     }
           else
     {
       Debug.LogWarning($"[Supply] Item {itemName} not found in {resourceType} config!");
     }
            }
        }
    }

    /// <summary>
    /// Get current spawn cost
    /// </summary>
    public int GetSpawnCost()
    {
     return currentConfig != null ? currentConfig.costPerSpawn : 0;
    }

    /// <summary>
    /// Check if player can afford to spawn
    /// </summary>
    public bool CanAffordSpawn()
    {
        if (inventoryManager == null || currentConfig == null)
            return false;

      return inventoryManager.Money >= currentConfig.costPerSpawn;
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (supplyConfig == null)
        {
  Debug.LogError($"[Supply] SupplyConfig not assigned on {gameObject.name}!");
      valid = false;
        }

        if (inventoryManager == null)
  {
          Debug.LogError($"[Supply] InventoryManager not assigned on {gameObject.name}!");
            valid = false;
        }

        if (spawnPoint == null)
        {
         Debug.LogWarning($"[Supply] No spawn point assigned on {gameObject.name}!");
        }

        if (notEnoughMoneyDialogue == null)
        {
            Debug.LogWarning($"[Supply] No 'not enough money' dialogue assigned on {gameObject.name}!");
        }

        return valid;
    }

    // Visual helper in editor
    void OnDrawGizmos()
    {
 if (spawnPoint == null)
     spawnPoint = transform;

        // Draw spawn point
        Gizmos.color = GetGizmoColor();
Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);

        // Draw launch direction
        if (supplyConfig != null)
        {
            SupplyConfig.ResourceTypeConfig config = supplyConfig.GetConfig(resourceType);
            if (config != null)
        {
         Vector3 direction = spawnPoint.TransformDirection(config.launchDirection.normalized);
          Gizmos.DrawRay(spawnPoint.position, direction * config.launchForce * 0.5f);
   }
        }
    }

void OnDrawGizmosSelected()
    {
if (spawnPoint == null)
     spawnPoint = transform;

        if (supplyConfig != null)
        {
SupplyConfig.ResourceTypeConfig config = supplyConfig.GetConfig(resourceType);
  if (config != null)
         {
          // Draw spawn cone
    Gizmos.color = new Color(GetGizmoColor().r, GetGizmoColor().g, GetGizmoColor().b, 0.3f);
     Vector3 direction = spawnPoint.TransformDirection(config.launchDirection.normalized);
      
     // Draw center line
   Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + direction * config.launchForce);
      
          // Draw cone edges if random variation enabled
    if (config.addRandomVariation && config.maxRandomAngle > 0)
    {
     Quaternion leftRotation = Quaternion.Euler(0, -config.maxRandomAngle, 0);
  Quaternion rightRotation = Quaternion.Euler(0, config.maxRandomAngle, 0);
      
          Vector3 leftDir = leftRotation * direction;
     Vector3 rightDir = rightRotation * direction;
           
        Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + leftDir * config.launchForce);
             Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + rightDir * config.launchForce);
       }
 }
        }
    }

    private Color GetGizmoColor()
    {
        switch (resourceType)
    {
        case ResourceType.Water:
         return new Color(0.3f, 0.6f, 1f); // Blue
            case ResourceType.Coffee:
        return new Color(0.6f, 0.3f, 0f); // Brown
      case ResourceType.Extras:
      return Color.yellow;
            default:
   return Color.white;
        }
    }
}

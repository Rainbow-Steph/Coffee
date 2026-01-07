using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject configuration for supply spawner system
/// Defines spawn settings, costs, and available items for each resource type
/// </summary>
[CreateAssetMenu(fileName = "SupplyConfig", menuName = "Coffee/Supply Configuration")]
public class SupplyConfig : ScriptableObject
{
    [System.Serializable]
    public class SpawnableItem
    {
     [Tooltip("The prefab to spawn")]
        public GameObject prefab;
        
      [Tooltip("Display name for this item")]
 public string itemName;

        [Tooltip("Is this item currently available for spawning?")]
        public bool isAvailable = true;
      
        [Tooltip("Spawn weight (higher = more likely to spawn)")]
  [Range(1, 100)]
        public int spawnWeight = 50;
    }

    [System.Serializable]
    public class ResourceTypeConfig
{
        [Header("Resource Info")]
        [Tooltip("Name of this resource type")]
        public string resourceTypeName = "Water";
        
        [Tooltip("Color for debug/UI display")]
        public Color debugColor = Color.blue;

        [Header("Cost Settings")]
      [Tooltip("How much money does one spawn cost?")]
        public int costPerSpawn = 10;

  [Header("Spawn Settings")]
        [Tooltip("Minimum number of items to spawn")]
        [Range(1, 10)]
        public int minSpawnCount = 1;
        
        [Tooltip("Maximum number of items to spawn")]
        [Range(1, 10)]
        public int maxSpawnCount = 3;
        
      [Tooltip("Delay between spawning multiple items (seconds)")]
        [Range(0f, 1f)]
        public float spawnDelay = 0.1f;

        [Header("Launch Settings")]
        [Tooltip("Launch force applied to spawned items")]
        [Range(0f, 20f)]
        public float launchForce = 5f;
        
        [Tooltip("Launch direction (relative to spawn point)")]
        public Vector3 launchDirection = Vector3.forward;
 
        [Tooltip("Add random variation to launch direction")]
        public bool addRandomVariation = true;
      
        [Tooltip("Maximum random angle variation in degrees")]
        [Range(0f, 45f)]
        public float maxRandomAngle = 15f;

        [Header("Available Items")]
    [Tooltip("List of items that can be spawned for this resource type")]
        public List<SpawnableItem> availableItems = new List<SpawnableItem>();

   /// <summary>
        /// Get a random available item from the list based on spawn weights
     /// </summary>
  public GameObject GetRandomAvailableItem()
        {
      // Filter available items
   List<SpawnableItem> available = availableItems.FindAll(item => 
                item != null && item.prefab != null && item.isAvailable);

     if (available.Count == 0)
  {
Debug.LogError("[SupplyConfig] No available items to spawn!");
              return null;
}

       // Debug: Log available items
#if UNITY_EDITOR
      Debug.Log($"[SupplyConfig] Available items count: {available.Count}");
     for (int i = 0; i < available.Count; i++)
  {
    Debug.Log($"  [{i}] {available[i].itemName} - Weight: {available[i].spawnWeight}, Available: {available[i].isAvailable}");
 }
#endif

            // Calculate total weight
            int totalWeight = 0;
     foreach (var item in available)
            {
    totalWeight += item.spawnWeight;
      }

   // Select random item based on weight
            int randomValue = Random.Range(0, totalWeight);
      int currentWeight = 0;
int selectedIndex = 0;

            foreach (var item in available)
            {
      currentWeight += item.spawnWeight;
      
     // If our random value falls within this item's range, select it
      if (randomValue < currentWeight)
     {
#if UNITY_EDITOR
            Debug.Log($"[SupplyConfig] Random: {randomValue}/{totalWeight}, Selected: [{selectedIndex}] {item.itemName}");
#endif
 return item.prefab;
        }
           selectedIndex++;
    }

   // Fallback to last available item (should never reach here)
            Debug.LogWarning($"[SupplyConfig] Fallback triggered! Random: {randomValue}, Total: {totalWeight}");
return available[available.Count - 1].prefab;
        }

        /// <summary>
        /// Get count of currently available items
        /// </summary>
        public int GetAvailableItemCount()
{
    return availableItems.FindAll(item => 
      item != null && item.prefab != null && item.isAvailable).Count;
        }

     /// <summary>
    /// Enable/disable a specific item by name
        /// </summary>
        public bool SetItemAvailability(string itemName, bool available)
        {
 SpawnableItem item = availableItems.Find(i => i.itemName == itemName);
     if (item != null)
 {
                item.isAvailable = available;
         return true;
            }
       return false;
        }
    }

    [Header("Resource Configurations")]
    [Tooltip("Configuration for water spawner")]
    public ResourceTypeConfig waterConfig = new ResourceTypeConfig();
    
    [Tooltip("Configuration for coffee spawner")]
    public ResourceTypeConfig coffeeConfig = new ResourceTypeConfig();
    
    [Tooltip("Configuration for extras spawner")]
    public ResourceTypeConfig extrasConfig = new ResourceTypeConfig();

    [Header("Animation Settings")]
    [Tooltip("Duration of spawn animation")]
    [Range(0.1f, 2f)]
    public float spawnAnimationDuration = 0.5f;
    
    [Tooltip("Duration of failed animation")]
    [Range(0.1f, 2f)]
    public float failedAnimationDuration = 0.3f;

    [Header("Audio Settings (Optional)")]
    [Tooltip("Sound played on successful spawn")]
  public AudioClip spawnSuccessSound;
    
    [Tooltip("Sound played when not enough money")]
    public AudioClip failedSound;

    private void OnEnable()
    {
        // Initialize default values if needed
        if (waterConfig.resourceTypeName == "")
        {
            waterConfig.resourceTypeName = "Water";
            waterConfig.debugColor = Color.blue;
    }

        if (coffeeConfig.resourceTypeName == "")
        {
            coffeeConfig.resourceTypeName = "Coffee";
     coffeeConfig.debugColor = new Color(0.6f, 0.3f, 0f); // Brown
        }

        if (extrasConfig.resourceTypeName == "")
  {
  extrasConfig.resourceTypeName = "Extras";
            extrasConfig.debugColor = Color.yellow;
        }
    }

    /// <summary>
    /// Get configuration for a specific resource type
    /// </summary>
    public ResourceTypeConfig GetConfig(ResourceType resourceType)
 {
        switch (resourceType)
        {
       case ResourceType.Water:
      return waterConfig;
        case ResourceType.Coffee:
      return coffeeConfig;
    case ResourceType.Extras:
        return extrasConfig;
            default:
          return null;
        }
    }

    /// <summary>
    /// Validate configuration
    /// </summary>
    public bool Validate()
    {
        bool valid = true;

        // Check water config
      if (waterConfig.availableItems.Count == 0)
        {
  Debug.LogWarning("[SupplyConfig] Water config has no items configured!");
    valid = false;
   }

        // Check coffee config
   if (coffeeConfig.availableItems.Count == 0)
 {
     Debug.LogWarning("[SupplyConfig] Coffee config has no items configured!");
      valid = false;
        }

        // Check extras config
        if (extrasConfig.availableItems.Count == 0)
        {
            Debug.LogWarning("[SupplyConfig] Extras config has no items configured!");
          valid = false;
      }

    return valid;
    }
}

/// <summary>
/// Resource type enum
/// </summary>
public enum ResourceType
{
    Water,
    Coffee,
    Extras
}

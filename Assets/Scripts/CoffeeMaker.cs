using UnityEngine;

/// <summary>
/// Handles coffee crafting logic
/// </summary>
public class CoffeeMaker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerActionTracker actionTracker;
    [SerializeField] private CraftingManager craftingManager;
 [SerializeField] private SystemMessages systemMessages;
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Spawn Settings")]
    [Tooltip("Where to spawn the crafted coffee")]
[SerializeField] private Transform spawnPoint;
    
  [Tooltip("Default spawn offset if no spawn point is set")]
    [SerializeField] private Vector3 defaultSpawnOffset = new Vector3(0, 1, 0);

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    /// <summary>
    /// Main function to craft coffee - can be called from ClickableObject
    /// </summary>
    public void MakeCoffee()
    {
        if (!ValidateReferences())
   return;

        if (showDebugLogs)
        {
    Debug.Log("[CoffeeMaker] MakeCoffee called");
        Debug.Log($"[CoffeeMaker] Current liquid amount: {actionTracker.LiquidAmount}");
 Debug.Log($"[CoffeeMaker] Machine contents: {actionTracker.LiquidName}, {actionTracker.CapsuleAName}, {actionTracker.CapsuleBName}, {actionTracker.AdditiveName}");
        }

        // Check if there's enough liquid
   if (actionTracker.LiquidAmount < 1)
     {
     ShowLiquidNeededMessage();
return;
        }

        // Try to find a matching recipe
        CraftingManager.CraftableRecipe matchingRecipe = craftingManager.FindMatchingRecipe(
 actionTracker.LiquidName,
    actionTracker.CapsuleAName,
   actionTracker.CapsuleBName,
        actionTracker.AdditiveName
        );

        if (matchingRecipe == null)
        {
if (showDebugLogs)
   {
       Debug.LogWarning("[CoffeeMaker] No matching recipe found for current machine contents");
        }
       // Could add a "no recipe found" dialogue here
  return;
  }

      // We have a valid recipe and enough liquid - craft it!
        CraftCoffee(matchingRecipe);
    }

    private void CraftCoffee(CraftingManager.CraftableRecipe recipe)
    {
  if (showDebugLogs)
        {
   Debug.Log($"[CoffeeMaker] Crafting: {recipe.recipeName}");
   }

        // Spawn the crafted item
   SpawnCraftedItem(recipe);

   // Reduce liquid amount by 1
  actionTracker.LiquidAmount--;

        // Clear capsules and additive from machine
        actionTracker.CapsuleAName = "";
     actionTracker.CapsuleBName = "";
  actionTracker.AdditiveName = "";

  // Show success message
 ShowCraftSuccessMessage(recipe.recipeName);

   if (showDebugLogs)
     {
 Debug.Log($"[CoffeeMaker] Crafting complete! Remaining liquid: {actionTracker.LiquidAmount}");
        }
    }

    private void SpawnCraftedItem(CraftingManager.CraftableRecipe recipe)
    {
     if (recipe.resultPrefab == null)
        {
            Debug.LogError($"[CoffeeMaker] Recipe '{recipe.recipeName}' has no result prefab!");
   return;
        }

  Vector3 spawnPosition;
        if (spawnPoint != null)
        {
  spawnPosition = spawnPoint.position;
  }
        else
        {
   spawnPosition = transform.position + defaultSpawnOffset;
     }

        GameObject craftedItem = Instantiate(recipe.resultPrefab, spawnPosition, Quaternion.identity);
   craftedItem.name = recipe.recipeName; // Remove "(Clone)" suffix

    if (showDebugLogs)
      {
       Debug.Log($"[CoffeeMaker] Spawned {recipe.recipeName} at {spawnPosition}");
        }
    }

    private void ShowLiquidNeededMessage()
    {
if (systemMessages != null && systemMessages.liquidNeededMessage != null && dialogueManager != null)
        {
      dialogueManager.StartDialogue(systemMessages.liquidNeededMessage);
   }
   else
        {
 Debug.LogWarning("[CoffeeMaker] Cannot show liquid needed message - missing references!");
 }
    }

    private void ShowCraftSuccessMessage(string recipeName)
    {
      if (systemMessages != null && systemMessages.craftSuccessMessage != null && dialogueManager != null)
        {
       dialogueManager.StartDialogue(systemMessages.craftSuccessMessage);
        }
        else
        {
  Debug.LogWarning("[CoffeeMaker] Cannot show craft success message - missing references!");
   }
    }

    private bool ValidateReferences()
    {
   if (actionTracker == null)
        {
      Debug.LogError("[CoffeeMaker] PlayerActionTracker not assigned!", this);
     return false;
        }

  if (craftingManager == null)
        {
Debug.LogError("[CoffeeMaker] CraftingManager not assigned!", this);
      return false;
        }

     if (systemMessages == null)
     {
   Debug.LogError("[CoffeeMaker] SystemMessages not assigned!", this);
      return false;
  }

        if (dialogueManager == null)
   {
            Debug.LogError("[CoffeeMaker] DialogueManager not assigned!", this);
    return false;
  }

        return true;
    }

    /// <summary>
/// Check if coffee can be made with current machine contents
    /// </summary>
    public bool CanMakeCoffee()
    {
     if (actionTracker == null || craftingManager == null)
     return false;

        if (actionTracker.LiquidAmount < 1)
       return false;

   CraftingManager.CraftableRecipe recipe = craftingManager.FindMatchingRecipe(
   actionTracker.LiquidName,
       actionTracker.CapsuleAName,
        actionTracker.CapsuleBName,
   actionTracker.AdditiveName
        );

        return recipe != null;
  }
}

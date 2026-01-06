using UnityEngine;
using System.Collections.Generic;

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

        // Consolidated debug output - all info in one message
        System.Text.StringBuilder debugOutput = new System.Text.StringBuilder();
        debugOutput.AppendLine("╔════════════════════════════════════════════════════════╗");
        debugOutput.AppendLine("║           MAKE COFFEE - DEBUG REPORT   ║");
        debugOutput.AppendLine("╚════════════════════════════════════════════════════════╝");
        debugOutput.AppendLine();
        debugOutput.AppendLine("═══ CURRENT MACHINE STATE ═══");
        debugOutput.AppendLine($"  Water Input:     {(string.IsNullOrEmpty(actionTracker.LiquidName) ? "EMPTY" : actionTracker.LiquidName)}");
        debugOutput.AppendLine($"  Capsule A: {(string.IsNullOrEmpty(actionTracker.CapsuleAName) ? "EMPTY" : actionTracker.CapsuleAName)}");
        debugOutput.AppendLine($"  Capsule B:    {(string.IsNullOrEmpty(actionTracker.CapsuleBName) ? "EMPTY" : actionTracker.CapsuleBName)}");
        debugOutput.AppendLine($"Additive:        {(string.IsNullOrEmpty(actionTracker.AdditiveName) ? "EMPTY" : actionTracker.AdditiveName)}");
        debugOutput.AppendLine($"  Liquid Amount:   {actionTracker.LiquidAmount}");
        debugOutput.AppendLine();

        // Check if there's enough liquid
        if (actionTracker.LiquidAmount < 1)
        {
            debugOutput.AppendLine("═══ RESULT ═══");
            debugOutput.AppendLine("  Status: ❌ FAILED");
            debugOutput.AppendLine("  Reason: Not enough liquid (need at least 1)");
            debugOutput.AppendLine("  Action: Showing 'Liquid Needed' dialogue");
            debugOutput.AppendLine("╚════════════════════════════════════════════════════════╝");
            Debug.Log(debugOutput.ToString());
      
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
            debugOutput.AppendLine("═══ RECIPE MATCHING ═══");
            debugOutput.AppendLine("  Matching Recipe: NONE FOUND");
            debugOutput.AppendLine($"  Available Recipes: {craftingManager.recipes.Count}");
            debugOutput.AppendLine();
            debugOutput.AppendLine("  Why no match? Checking all recipes:");
        
 // Show why each recipe doesn't match
            for (int i = 0; i < craftingManager.recipes.Count; i++)
     {
              var recipe = craftingManager.recipes[i];
             debugOutput.AppendLine($"  Recipe {i + 1}: {recipe.recipeName}");
   debugOutput.AppendLine($"    Requires: Liquid={recipe.requiredLiquid ?? "None"}, Coffee1={recipe.requiredCoffee1 ?? "None"}, Coffee2={recipe.requiredCoffee2 ?? "None"}, Extra={recipe.requiredExtra ?? "None"}");
      
        // Check why it doesn't match
           string reason = GetNonMatchReason(recipe, actionTracker.LiquidName, 
       actionTracker.CapsuleAName, actionTracker.CapsuleBName, actionTracker.AdditiveName);
    debugOutput.AppendLine($"    ❌ {reason}");
            }
    
            debugOutput.AppendLine();
         debugOutput.AppendLine("═══ RESULT ═══");
        debugOutput.AppendLine("  Status: ❌ FAILED");
         debugOutput.AppendLine("  Reason: No recipe matches current ingredients (PERFECT MATCH required)");
            debugOutput.AppendLine("  Note: All components must match exactly - no partial matches allowed");
            debugOutput.AppendLine("  Tip: Empty components in recipe = none required, Empty in machine = none provided");
         debugOutput.AppendLine("╚════════════════════════════════════════════════════════╝");
    Debug.LogWarning(debugOutput.ToString());
   return;
        }

        // Log successful recipe match
        debugOutput.AppendLine("═══ RECIPE MATCHING ═══");
        debugOutput.AppendLine($"  Matching Recipe: ✅ {matchingRecipe.recipeName}");
        debugOutput.AppendLine();
        debugOutput.AppendLine("  Recipe Requirements:");
        debugOutput.AppendLine($"    Liquid:    {(string.IsNullOrEmpty(matchingRecipe.requiredLiquid) ? "Any" : matchingRecipe.requiredLiquid)}");
        debugOutput.AppendLine($"    Coffee 1:  {(string.IsNullOrEmpty(matchingRecipe.requiredCoffee1) ? "None" : matchingRecipe.requiredCoffee1)}");
        debugOutput.AppendLine($"    Coffee 2:  {(string.IsNullOrEmpty(matchingRecipe.requiredCoffee2) ? "None" : matchingRecipe.requiredCoffee2)}");
        debugOutput.AppendLine($"    Extra:     {(string.IsNullOrEmpty(matchingRecipe.requiredExtra) ? "None" : matchingRecipe.requiredExtra)}");
        debugOutput.AppendLine();
        debugOutput.AppendLine("═══ CRAFTING ACTION ═══");
        debugOutput.AppendLine($"  Spawning:        {matchingRecipe.recipeName}");
        debugOutput.AppendLine($"  Prefab:          {(matchingRecipe.resultPrefab != null ? matchingRecipe.resultPrefab.name : "NULL - ERROR!")}");
        debugOutput.AppendLine($"  Liquid Before:   {actionTracker.LiquidAmount}");
        debugOutput.AppendLine($"  Liquid After:    {actionTracker.LiquidAmount - 1}");
        debugOutput.AppendLine();
        debugOutput.AppendLine("  Clearing:");
        debugOutput.AppendLine($"    ├─ Capsule A: {actionTracker.CapsuleAName} → CLEARED");
        debugOutput.AppendLine($"    ├─ Capsule B: {actionTracker.CapsuleBName} → CLEARED");
        debugOutput.AppendLine($"    └─ Additive:  {actionTracker.AdditiveName} → CLEARED");
        debugOutput.AppendLine();
        debugOutput.AppendLine("═══ RESULT ═══");
        debugOutput.AppendLine("  Status: ✅ SUCCESS");
        debugOutput.AppendLine($"  Created: {matchingRecipe.recipeName}");
        debugOutput.AppendLine("  Action: Showing 'Craft Success' dialogue");
        debugOutput.AppendLine("╚════════════════════════════════════════════════════════╝");
        Debug.Log(debugOutput.ToString());

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

    /// <summary>
    /// Get a human-readable reason why a recipe doesn't match the current machine contents
    /// </summary>
    private string GetNonMatchReason(CraftingManager.CraftableRecipe recipe, string liquid, string capsuleA, string capsuleB, string additive)
    {
        // Normalize empty strings
        liquid = string.IsNullOrEmpty(liquid) ? null : liquid;
        capsuleA = string.IsNullOrEmpty(capsuleA) ? null : capsuleA;
        capsuleB = string.IsNullOrEmpty(capsuleB) ? null : capsuleB;
        additive = string.IsNullOrEmpty(additive) ? null : additive;

        // Check liquid
        if (!string.IsNullOrEmpty(recipe.requiredLiquid))
        {
            if (liquid == null)
                return $"Needs liquid '{recipe.requiredLiquid}' but machine has none";
            if (!liquid.Contains(recipe.requiredLiquid))
                return $"Needs liquid '{recipe.requiredLiquid}' but machine has '{liquid}'";
        }
        else if (liquid != null)
        {
            return $"Recipe needs NO liquid but machine has '{liquid}'";
        }

        // Check capsule count
        int machineCapsulesCount = (capsuleA != null ? 1 : 0) + (capsuleB != null ? 1 : 0);
        bool needsCoffee1 = !string.IsNullOrEmpty(recipe.requiredCoffee1);
        bool needsCoffee2 = !string.IsNullOrEmpty(recipe.requiredCoffee2);
        int recipeCapsulesCount = (needsCoffee1 ? 1 : 0) + (needsCoffee2 ? 1 : 0);

        if (machineCapsulesCount != recipeCapsulesCount)
        {
            return $"Recipe needs {recipeCapsulesCount} capsule(s) but machine has {machineCapsulesCount}";
        }

        // Check specific capsules
        if (needsCoffee1 || needsCoffee2)
        {
            List<string> machineCapsules = new List<string>();
            if (capsuleA != null) machineCapsules.Add(capsuleA);
            if (capsuleB != null) machineCapsules.Add(capsuleB);

            List<string> requiredCapsules = new List<string>();
            if (needsCoffee1) requiredCapsules.Add(recipe.requiredCoffee1);
            if (needsCoffee2) requiredCapsules.Add(recipe.requiredCoffee2);

            foreach (string requiredCapsule in requiredCapsules)
            {
                bool found = false;
                foreach (string machineCapsule in machineCapsules)
                {
                    if (machineCapsule.Contains(requiredCapsule))
                    {
                        found = true;
                        break;
                    }
                }
  
                if (!found)
                    return $"Needs capsule containing '{requiredCapsule}' but not found in machine";
            }
        }

        // Check extra
        if (!string.IsNullOrEmpty(recipe.requiredExtra))
        {
            if (additive == null)
                return $"Needs extra '{recipe.requiredExtra}' but machine has none";
            if (!additive.Contains(recipe.requiredExtra))
                return $"Needs extra '{recipe.requiredExtra}' but machine has '{additive}'";
        }
        else if (additive != null)
        {
            return $"Recipe needs NO extra but machine has '{additive}'";
        }

        return "Unknown mismatch";
    }
}

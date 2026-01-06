using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages craftable coffee recipes
/// </summary>
[CreateAssetMenu(fileName = "CraftingManager", menuName = "Coffee/Crafting Manager")]
public class CraftingManager : ScriptableObject
{
    [System.Serializable]
 public class CraftableRecipe
    {
  [Header("Recipe Info")]
    public string recipeName;
  
        [Header("Required Components")]
        [Tooltip("Type of liquid required (e.g., 'Water')")]
   public string requiredLiquid;

        [Tooltip("First coffee capsule component (e.g., 'Red', 'Blue', 'Black')")]
     public string requiredCoffee1;
        
        [Tooltip("Second coffee capsule component (optional)")]
     public string requiredCoffee2;
   
        [Tooltip("Extra/additive component (optional, e.g., 'Salt', 'Sugar', 'Pepper')")]
        public string requiredExtra;

        [Header("Result")]
        [Tooltip("Prefab to spawn when this recipe is crafted")]
        public GameObject resultPrefab;

        /// <summary>
        /// Check if this recipe PERFECTLY matches the given machine contents
        /// Empty/null components in recipe are treated as "None required"
        /// Empty/null components in machine are treated as "None provided"
 /// All non-empty components must match exactly (using Contains for flexibility)
        /// </summary>
        public bool MatchesContents(string liquid, string capsuleA, string capsuleB, string additive)
        {
            // Normalize empty strings to null for consistent comparison
   liquid = string.IsNullOrEmpty(liquid) ? null : liquid;
        capsuleA = string.IsNullOrEmpty(capsuleA) ? null : capsuleA;
            capsuleB = string.IsNullOrEmpty(capsuleB) ? null : capsuleB;
   additive = string.IsNullOrEmpty(additive) ? null : additive;

    // === LIQUID CHECK ===
      // If recipe requires liquid, machine must have it
   if (!string.IsNullOrEmpty(requiredLiquid))
  {
     if (liquid == null || !liquid.Contains(requiredLiquid))
   return false;
         }
        // If recipe doesn't require liquid, machine must NOT have liquid (perfect match)
       else
            {
        if (liquid != null)
     return false; // Recipe wants no liquid, but machine has liquid
            }

          // === COFFEE COMPONENTS CHECK ===
            // Count how many coffee components the recipe requires
         bool needsCoffee1 = !string.IsNullOrEmpty(requiredCoffee1);
         bool needsCoffee2 = !string.IsNullOrEmpty(requiredCoffee2);
     
            // Count how many capsules the machine has
        int machineCapsulesCount = (capsuleA != null ? 1 : 0) + (capsuleB != null ? 1 : 0);
            int recipeCapsulesCount = (needsCoffee1 ? 1 : 0) + (needsCoffee2 ? 1 : 0);

            // Machine must have EXACTLY the same number of capsules as recipe requires
            if (machineCapsulesCount != recipeCapsulesCount)
return false;

// If recipe requires coffee components, check if they match
   if (needsCoffee1 || needsCoffee2)
         {
                // Collect machine capsules into a list
                List<string> machineCapsules = new List<string>();
     if (capsuleA != null) machineCapsules.Add(capsuleA);
 if (capsuleB != null) machineCapsules.Add(capsuleB);

                // Collect required capsules into a list
    List<string> requiredCapsules = new List<string>();
         if (needsCoffee1) requiredCapsules.Add(requiredCoffee1);
     if (needsCoffee2) requiredCapsules.Add(requiredCoffee2);

     // Check if all required capsules are found in machine capsules
  foreach (string requiredCapsule in requiredCapsules)
         {
 bool found = false;
         for (int i = 0; i < machineCapsules.Count; i++)
        {
          if (machineCapsules[i].Contains(requiredCapsule))
     {
          found = true;
 machineCapsules.RemoveAt(i); // Remove to prevent double-matching
         break;
   }
      }
        
        if (!found)
          return false; // Required capsule not found
     }

                // If there are leftover machine capsules, it's not a perfect match
     if (machineCapsules.Count > 0)
    return false;
     }

     // === EXTRA/ADDITIVE CHECK ===
      // If recipe requires extra, machine must have it
   if (!string.IsNullOrEmpty(requiredExtra))
   {
                if (additive == null || !additive.Contains(requiredExtra))
 return false;
   }
            // If recipe doesn't require extra, machine must NOT have extra (perfect match)
            else
            {
         if (additive != null)
  return false; // Recipe wants no extra, but machine has extra
   }

            // All checks passed - perfect match!
          return true;
        }

        public override string ToString()
     {
      return $"{recipeName}: Liquid={requiredLiquid}, Coffee1={requiredCoffee1}, Coffee2={requiredCoffee2}, Extra={requiredExtra}";
        }
    }

    [Header("Available Recipes")]
    [Tooltip("List of all craftable recipes")]
    public List<CraftableRecipe> recipes = new List<CraftableRecipe>();

    /// <summary>
    /// Find a recipe that matches the given machine contents
    /// </summary>
  public CraftableRecipe FindMatchingRecipe(string liquid, string capsuleA, string capsuleB, string additive)
    {
        foreach (var recipe in recipes)
        {
            if (recipe.MatchesContents(liquid, capsuleA, capsuleB, additive))
            {
   return recipe;
   }
        }

        return null;
    }

    /// <summary>
    /// Get recipe by name
    /// </summary>
    public CraftableRecipe GetRecipeByName(string name)
    {
  return recipes.Find(r => r.recipeName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }

  private void OnValidate()
  {
        // Validate recipes
    foreach (var recipe in recipes)
        {
    if (recipe.resultPrefab == null)
       {
     Debug.LogWarning($"CraftingManager: Recipe '{recipe.recipeName}' is missing a result prefab!", this);
  }
        }
    }
}

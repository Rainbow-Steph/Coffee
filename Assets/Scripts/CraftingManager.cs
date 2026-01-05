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
        /// Check if this recipe matches the given machine contents
        /// </summary>
        public bool MatchesContents(string liquid, string capsuleA, string capsuleB, string additive)
        {
            // Check liquid (must match if required)
         if (!string.IsNullOrEmpty(requiredLiquid))
  {
         if (string.IsNullOrEmpty(liquid) || !liquid.Contains(requiredLiquid))
      return false;
          }

            // Check first coffee component
            if (!string.IsNullOrEmpty(requiredCoffee1))
        {
      bool matchesA = !string.IsNullOrEmpty(capsuleA) && capsuleA.Contains(requiredCoffee1);
       bool matchesB = !string.IsNullOrEmpty(capsuleB) && capsuleB.Contains(requiredCoffee1);
      
if (!matchesA && !matchesB)
        return false;
            }

            // Check second coffee component (if specified)
     if (!string.IsNullOrEmpty(requiredCoffee2))
   {
             bool hasCoffee2 = false;
    
    // Check if Coffee2 is in either slot (and different from Coffee1 if both specified)
              if (!string.IsNullOrEmpty(capsuleA) && capsuleA.Contains(requiredCoffee2))
          hasCoffee2 = true;
   if (!string.IsNullOrEmpty(capsuleB) && capsuleB.Contains(requiredCoffee2))
        hasCoffee2 = true;
       
        if (!hasCoffee2)
return false;
            }

   // Check extra/additive component (if specified)
   if (!string.IsNullOrEmpty(requiredExtra))
     {
   if (string.IsNullOrEmpty(additive) || !additive.Contains(requiredExtra))
         return false;
         }

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

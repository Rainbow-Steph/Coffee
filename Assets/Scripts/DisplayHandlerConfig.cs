using UnityEngine;

/// <summary>
/// ScriptableObject to configure display materials for the coffee machine
/// </summary>
[CreateAssetMenu(fileName = "DisplayHandlerConfig", menuName = "Coffee/Display Handler Config")]
public class DisplayHandlerConfig : ScriptableObject
{
    [Header("Water Display Materials")]
    [Tooltip("Material when water display is empty")]
    public Material waterEmptyMaterial;
    
    [Tooltip("Material when water display is filled with water")]
    public Material waterFilledMaterial;

    [Header("Coffee Display Materials")]
    [Tooltip("Material when coffee display is empty")]
    public Material coffeeEmptyMaterial;
    
    [Tooltip("Material when filled with red capsules")]
    public Material redCapsuleMaterial;
    
    [Tooltip("Material when filled with blue capsules")]
    public Material blueCapsuleMaterial;
    
    [Tooltip("Material when filled with black capsules")]
    public Material blackCapsuleMaterial;

    [Header("Extra Display Materials")]
    [Tooltip("Material when extra display is empty")]
    public Material extraEmptyMaterial;
    
    [Tooltip("Material when filled with salt")]
    public Material saltMaterial;
    
    [Tooltip("Material when filled with sugar")]
    public Material sugarMaterial;
    
    [Tooltip("Material when filled with pepper")]
    public Material pepperMaterial;

    [Header("Capsule Name Matching")]
    [Tooltip("Text that identifies red capsules in their name")]
    public string redCapsuleIdentifier = "Red";
    
    [Tooltip("Text that identifies blue capsules in their name")]
    public string blueCapsuleIdentifier = "Blue";
    
    [Tooltip("Text that identifies black capsules in their name")]
    public string blackCapsuleIdentifier = "Black";

    [Header("Extra Name Matching")]
    [Tooltip("Text that identifies salt in its name")]
    public string saltIdentifier = "Salt";
    
    [Tooltip("Text that identifies sugar in its name")]
    public string sugarIdentifier = "Sugar";
    
    [Tooltip("Text that identifies pepper in its name")]
    public string pepperIdentifier = "Pepper";

    /// <summary>
    /// Validates that all required materials are assigned
    /// </summary>
    public bool ValidateConfig()
    {
        bool isValid = true;

    // Check water materials
        if (waterEmptyMaterial == null || waterFilledMaterial == null)
        {
            Debug.LogWarning("DisplayHandlerConfig: Water materials not fully assigned!", this);
   isValid = false;
        }

        // Check coffee materials
        if (coffeeEmptyMaterial == null || redCapsuleMaterial == null || 
   blueCapsuleMaterial == null || blackCapsuleMaterial == null)
    {
            Debug.LogWarning("DisplayHandlerConfig: Coffee materials not fully assigned!", this);
  isValid = false;
    }

        // Check extra materials
      if (extraEmptyMaterial == null || saltMaterial == null || 
         sugarMaterial == null || pepperMaterial == null)
     {
            Debug.LogWarning("DisplayHandlerConfig: Extra materials not fully assigned!", this);
         isValid = false;
        }

        return isValid;
    }
}

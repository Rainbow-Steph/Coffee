using UnityEngine;

/// <summary>
/// ClickableObject variant that modifies shader properties for outline highlighting.
/// Changes shader properties at runtime without swapping materials.
/// Works with materials using Custom/RimOutlineHighlight or Custom/OutlineHighlight shaders.
/// </summary>
public class ShaderPropertyClickable : ClickableObject
{
    [Header("Shader Property Settings")]
    [Tooltip("Rim intensity when hovering")]
    [Range(0, 10)]
    public float hoverRimIntensity = 5.0f;
    
    [Tooltip("Rim color when hovering")]
    public Color hoverRimColor = new Color(1, 0.5f, 0, 1);
  
    [Tooltip("Use outline intensity property (for OutlineHighlight shader)")]
    public bool useOutlineIntensity = false;
    
    [Tooltip("Outline intensity when hovering (if using OutlineHighlight shader)")]
    [Range(0, 5)]
    public float hoverOutlineIntensity = 3.0f;
    
  private Material materialInstance;
    private float originalRimIntensity = 0f;
  private float originalOutlineIntensity = 0f;
    private Color originalRimColor;
    private bool hasRimProperties = false;
    private bool hasOutlineProperties = false;
    
    void Start()
    {
   Renderer rend = GetComponent<Renderer>();
        if (rend != null)
     {
       // Create material instance to avoid modifying shared material
        materialInstance = rend.material;
   
       // Check which properties the shader has
        hasRimProperties = materialInstance.HasProperty("_RimIntensity") && 
         materialInstance.HasProperty("_RimColor");
            
    hasOutlineProperties = materialInstance.HasProperty("_OutlineIntensity") && 
          materialInstance.HasProperty("_OutlineColor");
       
       // Store original values
        if (hasRimProperties)
      {
          originalRimIntensity = materialInstance.GetFloat("_RimIntensity");
      originalRimColor = materialInstance.GetColor("_RimColor");
        
  // Set initial rim intensity to 0 (no outline when not hovering)
            materialInstance.SetFloat("_RimIntensity", 0f);
       }
            
            if (hasOutlineProperties && useOutlineIntensity)
        {
       originalOutlineIntensity = materialInstance.GetFloat("_OutlineIntensity");
    
         // Set initial outline intensity to 0
           materialInstance.SetFloat("_OutlineIntensity", 0f);
}
            
       if (showDebugInfo)
      {
    Debug.Log($"ShaderPropertyClickable on {gameObject.name}: " +
        $"Rim properties: {hasRimProperties}, Outline properties: {hasOutlineProperties}");
       }
 }
        else
  {
       Debug.LogWarning($"ShaderPropertyClickable on {gameObject.name}: No Renderer component found!");
     }
    }
    
    /// <summary>
    /// Called when mouse enters - activates outline by changing shader properties
    /// </summary>
    public override void OnHoverEnter()
    {
        if (materialInstance != null)
        {
    // Set rim properties
            if (hasRimProperties)
  {
   materialInstance.SetFloat("_RimIntensity", hoverRimIntensity);
       materialInstance.SetColor("_RimColor", hoverRimColor);
     }

       // Set outline properties
 if (hasOutlineProperties && useOutlineIntensity)
         {
         materialInstance.SetFloat("_OutlineIntensity", hoverOutlineIntensity);
         materialInstance.SetColor("_OutlineColor", hoverRimColor);
  }
  
         if (showDebugInfo)
   {
  Debug.Log($"ShaderPropertyClickable: {gameObject.name} - Outline activated");
       }
   }
   
        // Don't call base.OnHoverEnter() to avoid default color change
    }
    
    /// <summary>
    /// Called when mouse exits - deactivates outline by resetting shader properties
    /// </summary>
    public override void OnHoverExit()
    {
    if (materialInstance != null)
        {
       // Restore rim properties
     if (hasRimProperties)
  {
      materialInstance.SetFloat("_RimIntensity", 0f);
         materialInstance.SetColor("_RimColor", originalRimColor);
     }
 
      // Restore outline properties
       if (hasOutlineProperties && useOutlineIntensity)
  {
       materialInstance.SetFloat("_OutlineIntensity", 0f);
            }
 
    if (showDebugInfo)
 {
    Debug.Log($"ShaderPropertyClickable: {gameObject.name} - Outline deactivated");
            }
     }
  
    // Don't call base.OnHoverExit() to avoid default color change
    }
 
    /// <summary>
    /// Public method to manually control outline state
    /// </summary>
    /// <param name="active">True to show outline, false to hide</param>
    public void SetOutlineActive(bool active)
    {
   if (active)
        {
        OnHoverEnter();
      }
      else
        {
       OnHoverExit();
 }
    }
    
    /// <summary>
    /// Set custom outline color at runtime
    /// </summary>
    public void SetOutlineColor(Color color)
    {
     hoverRimColor = color;
        if (materialInstance != null && (hasRimProperties || hasOutlineProperties))
        {
       if (hasRimProperties)
      {
            materialInstance.SetColor("_RimColor", color);
      }
            if (hasOutlineProperties)
            {
       materialInstance.SetColor("_OutlineColor", color);
}
    }
    }
    
 /// <summary>
    /// Set custom outline intensity at runtime
    /// </summary>
    public void SetOutlineIntensity(float intensity)
    {
        hoverRimIntensity = Mathf.Clamp(intensity, 0f, 10f);
        if (materialInstance != null && hasRimProperties)
        {
      materialInstance.SetFloat("_RimIntensity", hoverRimIntensity);
        }
    }
    
    /// <summary>
    /// Clean up resources when object is destroyed
    /// </summary>
    protected override void OnDestroy()
    {
        // Clean up material instance
        if (materialInstance != null)
        {
         Destroy(materialInstance);
        }
        
        // Call base OnDestroy
        base.OnDestroy();
}
}

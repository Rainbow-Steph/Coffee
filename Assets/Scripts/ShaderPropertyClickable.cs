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
    
    private Material shaderMaterialInstance;
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
 shaderMaterialInstance = rend.material;
   
  // Check which properties the shader has
      hasRimProperties = shaderMaterialInstance.HasProperty("_RimIntensity") && 
                shaderMaterialInstance.HasProperty("_RimColor");
      
            hasOutlineProperties = shaderMaterialInstance.HasProperty("_OutlineIntensity") && 
            shaderMaterialInstance.HasProperty("_OutlineColor");
    
 // Store original values
  if (hasRimProperties)
            {
    originalRimIntensity = shaderMaterialInstance.GetFloat("_RimIntensity");
                originalRimColor = shaderMaterialInstance.GetColor("_RimColor");
        
             // Set initial rim intensity to 0 (no outline when not hovering)
     shaderMaterialInstance.SetFloat("_RimIntensity", 0f);
            }
 
            if (hasOutlineProperties && useOutlineIntensity)
        {
       originalOutlineIntensity = shaderMaterialInstance.GetFloat("_OutlineIntensity");
    
     // Set initial outline intensity to 0
            shaderMaterialInstance.SetFloat("_OutlineIntensity", 0f);
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
    if (shaderMaterialInstance != null)
        {
            // Set rim properties
     if (hasRimProperties)
            {
    shaderMaterialInstance.SetFloat("_RimIntensity", hoverRimIntensity);
         shaderMaterialInstance.SetColor("_RimColor", hoverRimColor);
            }

 // Set outline properties
            if (hasOutlineProperties && useOutlineIntensity)
            {
        shaderMaterialInstance.SetFloat("_OutlineIntensity", hoverOutlineIntensity);
           shaderMaterialInstance.SetColor("_OutlineColor", hoverRimColor);
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
        if (shaderMaterialInstance != null)
        {
   // Restore rim properties
   if (hasRimProperties)
         {
      shaderMaterialInstance.SetFloat("_RimIntensity", 0f);
     shaderMaterialInstance.SetColor("_RimColor", originalRimColor);
            }
 
       // Restore outline properties
          if (hasOutlineProperties && useOutlineIntensity)
{
    shaderMaterialInstance.SetFloat("_OutlineIntensity", 0f);
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
        if (shaderMaterialInstance != null && (hasRimProperties || hasOutlineProperties))
     {
       if (hasRimProperties)
{
    shaderMaterialInstance.SetColor("_RimColor", color);
            }
      if (hasOutlineProperties)
     {
    shaderMaterialInstance.SetColor("_OutlineColor", color);
          }
      }
    }
  
    /// <summary>
    /// Set custom outline intensity at runtime
    /// </summary>
    public void SetOutlineIntensity(float intensity)
    {
        hoverRimIntensity = Mathf.Clamp(intensity, 0f, 10f);
        if (shaderMaterialInstance != null && hasRimProperties)
        {
   shaderMaterialInstance.SetFloat("_RimIntensity", hoverRimIntensity);
        }
    }
    
    /// <summary>
    /// Clean up resources when object is destroyed
  /// </summary>
    protected override void OnDestroy()
    {
        // Clean up material instance
    if (shaderMaterialInstance != null)
        {
            Destroy(shaderMaterialInstance);
 }

        // Call base OnDestroy
        base.OnDestroy();
    }
}

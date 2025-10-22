using UnityEngine;

/// <summary>
/// ClickableObject variant that uses material swapping for outline highlighting.
/// Replace the entire material on hover for complete control over the outline effect.
/// </summary>
public class OutlineClickable : ClickableObject
{
    [Header("Outline Material Settings")]
    [Tooltip("Material to use when hovering (should use outline shader)")]
    public Material outlineMaterial;
    
    [Tooltip("Transition speed for material swap")]
  [Range(0.1f, 1f)]
    public float transitionSpeed = 0.3f;
 
    private Material originalMaterial;
    private Renderer objectRenderer;
    private bool isTransitioning = false;
    
    void Start()
    {
        // Store original material
        objectRenderer = GetComponent<Renderer>();
    if (objectRenderer != null)
        {
         originalMaterial = objectRenderer.sharedMaterial;
        }
        else
        {
    Debug.LogWarning($"OutlineClickable on {gameObject.name}: No Renderer component found!");
        }
    
      // Validate outline material
        if (outlineMaterial == null)
    {
       Debug.LogWarning($"OutlineClickable on {gameObject.name}: No outline material assigned!");
        }
    }
    
    /// <summary>
    /// Called when mouse enters this object - switches to outline material
    /// </summary>
    public override void OnHoverEnter()
    {
        if (objectRenderer != null && outlineMaterial != null && !isTransitioning)
        {
         objectRenderer.material = outlineMaterial;
    
       if (showDebugInfo)
          {
       Debug.Log($"OutlineClickable: {gameObject.name} - Outline material applied");
    }
      }
   
        // Don't call base.OnHoverEnter() to avoid default color change
    }
    
    /// <summary>
    /// Called when mouse exits this object - restores original material
    /// </summary>
    public override void OnHoverExit()
    {
        if (objectRenderer != null && originalMaterial != null && !isTransitioning)
        {
            objectRenderer.material = originalMaterial;
 
      if (showDebugInfo)
      {
          Debug.Log($"OutlineClickable: {gameObject.name} - Original material restored");
       }
  }

 // Don't call base.OnHoverExit() to avoid default color change
    }
    
    /// <summary>
    /// Public method to manually set outline state
    /// </summary>
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
    /// Clean up resources when object is destroyed
/// </summary>
    protected override void OnDestroy()
    {
        // Clean up if we created any material instances
        if (objectRenderer != null && objectRenderer.material != originalMaterial && objectRenderer.material != outlineMaterial)
  {
   Destroy(objectRenderer.material);
        }
        
        // Call base OnDestroy
        base.OnDestroy();
    }
}

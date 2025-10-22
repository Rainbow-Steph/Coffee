# Outline Highlight Shaders

This document explains how to use the custom outline shaders with the ClickableObject system.

## Available Shaders

### 1. **OutlineHighlight.shader** (Advanced)
Full outline shader with two rendering passes:
- **Pass 1**: Renders a solid outline by expanding vertices
- **Pass 2**: Renders the object with rim lighting

**Best for:** Objects that need a thick, solid outline

**Settings:**
- `Main Color` - Base color of the object
- `Outline Color` - Color of the outline/rim
- `Outline Width` - Thickness of the outline (0.0 - 0.1)
- `Outline Intensity` - Brightness of the outline (0 - 5)
- `Rim Power` - Controls falloff of rim lighting (0.1 - 8.0)
- `Rim Intensity` - Brightness of rim effect (0 - 3)

---

### 2. **RimOutlineHighlight.shader** (Simple)
Rim-lighting based shader that creates a glowing edge effect.

**Best for:** Objects that need a subtle, glowing outline

**Settings:**
- `Main Color` - Base color of the object
- `Rim Color` - Color of the rim/outline
- `Rim Power` - Controls falloff of rim (0.1 - 8.0)
- `Rim Intensity` - Brightness of rim (0 - 10)

---

## How to Use with ClickableObject

### Method 1: Replace Material on Hover (Recommended)

This method swaps the entire material when hovering, giving you complete control over the outline effect.

**Steps:**

1. **Create Materials:**
   - Create a normal material for the object
   - Create a highlight material using one of the outline shaders
   - Configure the outline shader settings to your liking

2. **Create a Custom Script:**

```csharp
using UnityEngine;

public class OutlineClickable : ClickableObject
{
    [Header("Outline Material Settings")]
    [Tooltip("Material to use when hovering (with outline shader)")]
    public Material outlineMaterial;
    
    private Material originalMaterial;
    private Renderer objectRenderer;
    
    void Start()
    {
    // Store original material
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
     originalMaterial = objectRenderer.material;
}
    }
    
  public override void OnHoverEnter()
    {
        // Switch to outline material
   if (objectRenderer != null && outlineMaterial != null)
     {
            objectRenderer.material = outlineMaterial;
     }
        
        // Call base implementation (optional)
        // base.OnHoverEnter();
    }
    
  public override void OnHoverExit()
    {
        // Restore original material
 if (objectRenderer != null && originalMaterial != null)
        {
 objectRenderer.material = originalMaterial;
     }
        
      // Call base implementation (optional)
    // base.OnHoverExit();
    }
}
```

3. **Setup in Unity:**
- Remove `ClickableObject` component from your object
   - Add `OutlineClickable` component instead
   - Assign the outline material to the `Outline Material` field
   - Disable "Highlight On Hover" (we're handling it manually)

---

### Method 2: Modify Shader Properties

This method changes shader properties at runtime without swapping materials.

**Steps:**

1. **Assign Outline Shader:**
   - Create a material with `Custom/RimOutlineHighlight` shader
- Assign it to your object
   - Set Rim Intensity to 0 initially

2. **Create a Custom Script:**

```csharp
using UnityEngine;

public class ShaderPropertyClickable : ClickableObject
{
    [Header("Shader Property Settings")]
    [Tooltip("Rim intensity when hovering")]
    [Range(0, 10)]
    public float hoverRimIntensity = 5.0f;
    
    [Tooltip("Rim color when hovering")]
    public Color hoverRimColor = new Color(1, 0.5f, 0, 1);
    
    private Material materialInstance;
    private float originalRimIntensity = 0f;
    private Color originalRimColor;
    
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
  // Create material instance
            materialInstance = rend.material;
  
   // Store original values
       if (materialInstance.HasProperty("_RimIntensity"))
  {
originalRimIntensity = materialInstance.GetFloat("_RimIntensity");
          }
          if (materialInstance.HasProperty("_RimColor"))
  {
         originalRimColor = materialInstance.GetColor("_RimColor");
            }
        }
    }
    
  public override void OnHoverEnter()
    {
    if (materialInstance != null)
        {
            if (materialInstance.HasProperty("_RimIntensity"))
            {
       materialInstance.SetFloat("_RimIntensity", hoverRimIntensity);
      }
     if (materialInstance.HasProperty("_RimColor"))
            {
     materialInstance.SetColor("_RimColor", hoverRimColor);
       }
        }
        
      // Disable base hover highlighting
   // base.OnHoverEnter();
    }
    
    public override void OnHoverExit()
    {
        if (materialInstance != null)
 {
         if (materialInstance.HasProperty("_RimIntensity"))
       {
    materialInstance.SetFloat("_RimIntensity", originalRimIntensity);
   }
            if (materialInstance.HasProperty("_RimColor"))
            {
       materialInstance.SetColor("_RimColor", originalRimColor);
          }
   }
      
   // Disable base hover highlighting
      // base.OnHoverExit();
    }
    
    void OnDestroy()
    {
        // Clean up material instance
     if (materialInstance != null)
        {
            Destroy(materialInstance);
      }
    }
}
```

3. **Setup in Unity:**
   - Remove `ClickableObject` component
   - Add `ShaderPropertyClickable` component
   - Configure hover rim intensity and color
   - Disable "Highlight On Hover"

---

## Shader Comparison

| Feature | OutlineHighlight | RimOutlineHighlight |
|---------|-----------------|---------------------|
| **Outline Type** | Solid edge | Glowing edge |
| **Performance** | Medium | Good |
| **Customization** | High | Medium |
| **Visibility** | Excellent | Good |
| **Works on all angles** | Yes | Best at edges |
| **Complexity** | Advanced | Simple |

---

## Tips for Best Results

### For OutlineHighlight Shader:
- Start with Outline Width: 0.03
- Outline Intensity: 2.0
- Rim Power: 3.0
- Adjust based on object size

### For RimOutlineHighlight Shader:
- Start with Rim Power: 3.0
- Rim Intensity: 5.0
- Use bright colors for better visibility
- Works best on curved surfaces

### General Tips:
- Use Layer Masks to control what can be clicked
- Test with different camera angles
- Combine with audio feedback for better UX
- Consider object size when adjusting outline width
- Bright colors (orange, yellow, cyan) work best for outlines

---

## Troubleshooting

**Outline too thick:**
- Reduce `Outline Width` (OutlineHighlight shader)
- Reduce `Rim Intensity` (RimOutlineHighlight shader)

**Outline not visible:**
- Increase `Outline Intensity`
- Choose a brighter color
- Check if object has a Renderer component
- Ensure material is using the outline shader

**Outline visible all the time:**
- Make sure you're resetting material/properties in `OnHoverExit()`
- Check that the base material doesn't have outline enabled
- Verify script is properly swapping materials

**Performance issues:**
- Use RimOutlineHighlight (simpler shader)
- Reduce number of outlined objects on screen
- Use object pooling for clickable objects

---

## Example Setups

### Setup 1: Subtle Glow Outline
```
Shader: RimOutlineHighlight
Settings:
- Rim Color: (1, 0.8, 0.4) // Light orange
- Rim Power: 4.0
- Rim Intensity: 3.0
```

### Setup 2: Bold Solid Outline
```
Shader: OutlineHighlight
Settings:
- Outline Color: (0, 1, 1) // Cyan
- Outline Width: 0.04
- Outline Intensity: 3.0
- Rim Power: 2.5
```

### Setup 3: Inspection Mode
```
Shader: OutlineHighlight
Settings:
- Outline Color: (1, 1, 0) // Yellow
- Outline Width: 0.05
- Outline Intensity: 4.0
- Use with Float On Click: Enabled
```

---

## Integration with Existing System

The outline shaders work seamlessly with your existing ClickableObject features:

- ? Compatible with hover detection
- ? Works with float behavior
- ? Supports click events
- ? Can be combined with audio feedback
- ? Works with single-item holding system

**Note:** When using custom shader scripts (Method 1 or 2), disable the built-in "Highlight On Hover" feature in ClickableObject to avoid conflicts.

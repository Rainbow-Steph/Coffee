# Unity Raycast Click System

This folder contains scripts for implementing a camera-based raycast click system in Unity.

## Components

### 1. CameraRaycaster.cs
Handles raycasting from the center of the camera's frustum when the player clicks.

**Setup:**
- Attach this script to your Main Camera (or any camera you want to use for clicking)
- Configure the settings in the Inspector

**Settings:**
- **Max Ray Distance**: How far the ray will travel (default: 100 units)
- **Clickable Layer Mask**: Filter which layers can be clicked
- **Mouse Button**: Which mouse button to use (0=left, 1=right, 2=middle)
- **Enable Hover Detection**: Continuously detect when mouse is over clickable objects (default: enabled)
- **Show Debug Info**: Display console messages when clicking
- **Draw Debug Ray**: Visualize the ray in Scene view (green=hit, red=miss)

### 2. ClickableObject.cs
Add this component to any GameObject you want to make clickable.

**Setup:**
- Attach this script to any GameObject with a Collider
- The script requires a Collider component to work
- Configure the response behavior in the Inspector

**Features:**
- **Unity Event System**: Add custom responses using the OnClickEvent
- **Hover Highlighting**: Objects light up when mouse hovers over them
- **Float Behavior**: Objects can float in front of the camera when clicked
- **Visual Feedback**: Optionally change color when clicked
- **Audio Feedback**: Play a sound when clicked
- **Extensible**: Override `OnClickedCustom()` method for custom behavior

**Advanced Variants:**
- **OutlineClickable.cs**: Material-swap based outline highlighting (recommended for best visual quality)
- **ShaderPropertyClickable.cs**: Shader property-based outline highlighting (better performance)

### Hover Highlighting

The hover system is **enabled by default** and provides visual feedback:

- **Automatic Highlighting**: Objects glow/change color when the crosshair is over them
- **Material Instance**: Each object gets its own material copy (no shared material issues)
- **Emission Support**: Objects can glow using emission for better visibility
- **Customizable Colors**: Choose your own hover and click colors
- **Performance**: Only checks on Update, minimal overhead

**Alternative: Outline-Only Highlighting**

For a more subtle effect that only highlights the outline/edge of objects:

1. **Use Outline Shaders**: See `Assets/Shaders/OUTLINE_SHADERS_GUIDE.md` for details
   - `Custom/OutlineHighlight` - Solid edge outline
   - `Custom/RimOutlineHighlight` - Glowing rim effect

2. **Use Outline Scripts**:
   - `OutlineClickable` - Material-swap based (best quality)
   - `ShaderPropertyClickable` - Property-based (best performance)

3. **Setup**:
   - Create material with outline shader
   - Replace `ClickableObject` with `OutlineClickable` or `ShaderPropertyClickable`
   - Assign outline material (OutlineClickable) or configure properties (ShaderPropertyClickable)
   - Disable built-in "Highlight On Hover" to avoid conflicts

**Tips for Best Results:**
- Use materials with emission support (Standard shader works great)
- Adjust `Hover Emission Intensity` for stronger/weaker glow
- Try different hover colors to match your game's aesthetic
- Disable `Use Emission` if your material doesn't support it
- **For outline-only**: Use bright colors (cyan, orange, yellow) for better visibility

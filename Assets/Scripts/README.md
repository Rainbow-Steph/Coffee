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
- **Hover Highlighting**: Multiple modes - standard color change, outline material swap, or disabled
- **Float Behavior**: Objects can float in front of the camera when clicked
- **Single Item Holding**: Only one item can be held at a time
- **Visual Feedback**: Optionally change color when clicked
- **Audio Feedback**: Play a sound when clicked
- **Extensible**: Override `OnClickedCustom()` method for custom behavior

**Highlight Modes:**
- **ColorChange** (default): Standard color and emission highlighting
- **OutlineMaterial**: Swaps to an outline material on hover (for outline-only effect)
- **Disabled**: No hover highlighting

**Advanced Variant:**
- **ShaderPropertyClickable.cs**: Shader property-based outline highlighting (modifies shader properties at runtime for better performance)

### Hover Highlighting

The hover system provides multiple modes for visual feedback:

**Highlight Modes:**

1. **ColorChange Mode** (Default)
   - Objects glow/change color when the crosshair is over them
   - Material Instance: Each object gets its own material copy (no shared material issues)
   - Emission Support: Objects can glow using emission for better visibility
   - Customizable Colors: Choose your own hover and click colors

2. **OutlineMaterial Mode**
   - Swaps to a separate outline material when hovering
   - Replaces the original material completely
   - Requires outline material assignment (use Custom/OutlineHighlight or Custom/RimOutlineHighlight shaders)
   - See `Assets/Shaders/OUTLINE_SHADERS_GUIDE.md` for shader details

3. **OutlineOverlay Mode** (NEW!)
   - Adds outline material ON TOP of the original material
   - Original material stays visible with outline rendered over it
   - Best of both worlds: keep texture/details + outline effect
   - Requires outline material with transparency/overlay shader
   - Works with Custom/OutlineHighlight shader (set to render as overlay)

4. **Disabled Mode**
   - No hover highlighting
   - Useful when you only want click responses

**Setup for Outline Mode:**
1. Create material with outline shader (`Custom/OutlineHighlight` or `Custom/RimOutlineHighlight`)
2. Select your ClickableObject
3. Set "Highlight Mode" to:
   - "OutlineMaterial" to replace the material (outline only)
   - "OutlineOverlay" to add outline over existing material (NEW!)
4. Assign the outline material to "Outline Material" field
5. Test - object should show outline on hover!

**Setup for OutlineOverlay (Recommended for Best Results):**
1. Create material with `Custom/OutlineHighlight` shader
2. Configure outline color, width, and intensity
3. Set Highlight Mode to "OutlineOverlay"
4. Assign outline material
5. Original material textures/colors stay visible with outline on top!

**Alternative: ShaderPropertyClickable**
- For property-based outline highlighting (better performance)
- Modifies shader properties at runtime instead of swapping materials
- See shader guide for setup instructions

**Tips for Best Results:**
- **ColorChange mode**: Use materials with emission support (Standard shader works great)
- **ColorChange mode**: Adjust `Hover Emission Intensity` for stronger/weaker glow
- **OutlineMaterial mode**: Use bright colors (cyan, orange, yellow) for better visibility
- Try different hover colors to match your game's aesthetic
- Disable highlighting entirely if you only want click responses

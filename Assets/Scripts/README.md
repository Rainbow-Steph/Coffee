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

**Settings:**
- **On Click Event**: Add functions to call when clicked (drag & drop method)
- **Change Color On Click**: Enable color change feedback
- **Click Color**: Color to flash when clicked
- **Color Change Duration**: How long the color change lasts
- **Highlight On Hover**: Enable highlighting when mouse hovers over object (default: enabled)
- **Hover Color**: Color to use for hover highlight (default: light orange)
- **Hover Emission Intensity**: Makes the object glow when hovered (0-5 range)
- **Use Emission**: Enable emission/glow effect for hover (default: enabled)
- **Float On Click**: Make object float in front of camera when clicked (default: disabled)
- **Float Distance**: Distance from camera to float at (default: 2 units)
- **Float Speed**: Speed at which object moves to float position (default: 5)
- **Rotation Speed**: Speed at which object rotates while floating in degrees/second (default: 30) - rotates on **world Y axis**
- **Rotation Offset**: Starting rotation applied to the floating object as Euler angles (X, Y, Z)
- **Float Offset**: Offset from center of camera view (up/down/left/right)
- **Play Sound On Click**: Enable audio feedback
- **Click Sound**: AudioClip to play when clicked

## Quick Start Guide

### Basic Setup (3 steps):

1. **Add CameraRaycaster to your camera:**
   - Select your Main Camera
   - Add Component ? CameraRaycaster
   - Leave default settings or adjust as needed

2. **Make an object clickable:**
   - Select any GameObject with a Collider (e.g., a Cube)
   - Add Component ? ClickableObject
   - The object will now highlight when you aim at it!

3. **Test:**
   - Enter Play mode
   - Aim at the object (center of screen) - it should highlight
   - Click the left mouse button to trigger the click event
   - Check the Console for click messages (if debug is enabled)

### UI Setup (Display Held Item Name)

To show the held item name on screen, you need to manually set up the HeldItemDisplay component:

**Manual Setup:**
1. **Create a UI Text element:**
   - Right-click on Canvas in Hierarchy ? UI ? Text (for Unity UI Text)
   - Or: Right-click on Canvas ? UI ? Text - TextMeshPro (for TextMeshPro)

2. **Add HeldItemDisplay component:**
   - Select the Text object you just created
   - Add Component ? HeldItemDisplay
   - The component will automatically detect the Text or TextMeshPro component

3. **Configure (optional):**
   - Adjust the prefix text (default: "Holding: ")
   - Set empty text (default: "No item held")
   - Enable "Hide When Empty" if desired

4. **Position the text:**
   - Use the RectTransform to position it where you want on screen
   - Common positions: top-left corner, bottom-center, etc.

The UI will automatically update to show the name of any held item!

**Note:** The HeldItemDisplay component supports both Unity UI Text and TextMeshPro components. It will automatically detect which one you're using.

### Float Behavior

The float system allows objects to smoothly move in front of the camera when clicked:

**How it works:**
1. Enable "Float On Click" in ClickableObject Inspector
2. Click the object once - it floats to the camera and rotates
3. **Right-click anywhere** - object returns to pickup position immediately
4. Or click it again (left-click) - object returns to its **pickup position** (where it was when you clicked it)

**Features:**
- **Smooth Movement**: Uses Lerp for smooth transitions
- **Automatic Rotation**: Objects slowly spin while floating on the **world Y axis** (vertical)
- **Right-Click Return**: Press right mouse button to instantly return object to pickup position
- **Rotation Offset**: Apply a starting rotation to the floating object (e.g., tilt it 45°)
- **Position Memory**: Returns to the exact position where it was picked up (not spawn position)
- **Collider Management**: Collider is disabled while floating to prevent physics issues
- **Customizable**: Adjust distance, speed, rotation speed, offset, and rotation angle

**Use Cases:**
- Item inspection systems
- Collectible pickup previews
- Interactive object examination
- Menu item selection with 3D preview
- Inventory item viewing

**Tips:**
- Adjust "Float Distance" to control how close objects appear (2-3 units works well)
- Increase "Rotation Speed" for faster spinning (try 60-90 for faster rotation)
- Use "Float Offset" to position objects off-center (e.g., `Vector3(0.5, -0.2, 0)`)
- Use "Rotation Offset" to tilt or angle the object while floating (e.g., `(45, 0, 0)` for 45° tilt)
- Rotation always happens on the world Y axis (up/down), so objects spin upright naturally
- **Right-click anywhere to quickly return the object** (no need to aim at it)
- Combine with color change for additional feedback
- Works great with small to medium-sized objects
- The object remembers where it was when picked up, not where it spawned

### Hover Highlighting

The hover system is **enabled by default** and provides visual feedback:

- **Automatic Highlighting**: Objects glow/change color when the crosshair is over them
- **Material Instance**: Each object gets its own material copy (no shared material issues)
- **Emission Support**: Objects can glow using emission for better visibility
- **Customizable Colors**: Choose your own hover and click colors
- **Performance**: Only checks on Update, minimal overhead

**Tips for Best Results:**
- Use materials with emission support (Standard shader works great)
- Adjust `Hover Emission Intensity` for stronger/weaker glow
- Try different hover colors to match your game's aesthetic
- Disable `Use Emission` if your material doesn't support it

### Adding Custom Responses:

**Method 1: Unity Events (No Code)**
- In ClickableObject's Inspector, expand "On Click Event"
- Click the "+" button
- Drag a GameObject into the object field
- Select a function from that GameObject's components

**Method 2: Custom Script**
Create a script that responds to clicks:

```csharp
using UnityEngine;

public class MyClickResponse : MonoBehaviour
{
    public void OnObjectClicked()
    {
        Debug.Log("My custom response!");
      // Add your custom behavior here
    }
}
```

Then attach it to your clickable object and add it to the OnClickEvent.

**Method 3: Inherit from ClickableObject**
```csharp
using UnityEngine;

public class MyCustomClickable : ClickableObject
{
    protected override void OnClickedCustom(RaycastHit hit)
    {
        Debug.Log("Custom click behavior!");
        // Add your custom behavior here
    }
}
```

## Example Setups

### Example 1: Simple Inspection System
```
ClickableObject Settings:
- Float On Click: ? Enabled
- Float Distance: 2.5
- Rotation Speed: 45
- Rotation Offset: (30, 0, 0)
- Highlight On Hover: ? Enabled
- Change Color On Click: ? Enabled
```
Result: Objects highlight on hover, float and spin with a 30° tilt when clicked for inspection

### Example 2: Collectible Pickup
```
ClickableObject Settings:
- Float On Click: ? Enabled
- Float Distance: 1.5
- Float Speed: 8
- Play Sound On Click: ? Enabled
- OnClickEvent: ? Call your CollectItem() function
```
Result: Item floats to camera with sound, then you can run custom collection logic

### Example 3: Hover-Only Feedback
```
ClickableObject Settings:
- Float On Click: ? Disabled
- Highlight On Hover: ? Enabled
- Hover Emission Intensity: 2.0
- Change Color On Click: ? Enabled
```
Result: Simple hover highlight with click feedback, no floating

## Tips

- Make sure your clickable objects have Colliders
- Use Layer Masks to filter what can be clicked
- Enable debug rays to visualize the raycast
- The ray casts from the **center** of the camera's viewport
- You can have multiple ClickableObjects in your scene
- The system works with any Collider type (Box, Sphere, Mesh, etc.)
- Hover highlighting uses material instances (no shared material pollution)
- Emission effects require materials that support the `_EmissionColor` property
- Float behavior requires a Camera tagged as "MainCamera"
- Floating objects have their collider temporarily disabled

## Troubleshooting

**Object not clicking:**
- Ensure the object has a Collider component
- Check if the object's layer is included in the Layer Mask
- Enable "Draw Debug Ray" to see where you're clicking
- Make sure the camera has the CameraRaycaster component

**No hover highlighting:**
- Ensure "Highlight On Hover" is enabled in ClickableObject
- Check that "Enable Hover Detection" is enabled in CameraRaycaster
- Verify the object has a Renderer component with a material
- Try increasing "Hover Emission Intensity" for more visible effect
- If using a custom shader, disable "Use Emission" if not supported

**Float behavior not working:**
- Make sure your camera is tagged as "MainCamera"
- Enable "Float On Click" in ClickableObject
- Check the console for warning messages
- Verify the object isn't constrained by a parent or Rigidbody

**Object doesn't return to original position:**
- Click the object a second time while it's floating
- Use the `ForceReturnToOriginal()` method to manually return it
- Check that the object hasn't been destroyed or disabled

**Floating object behaves strangely:**
- If the object has a Rigidbody, set it to Kinematic
- Check that no other scripts are moving the object
- Ensure parent objects aren't moving during float

**No visual feedback:**
- Ensure "Change Color On Click" is enabled
- Check that the object has a Renderer component
- Make sure the object uses a material (not just a shader)

**No audio feedback:**
- Enable "Play Sound On Click"
- Assign an AudioClip to the Click Sound field
- An AudioSource will be automatically added if needed

**Hover not clearing:**
- CameraRaycaster automatically clears hover when disabled
- Make sure objects aren't overlapping in unexpected ways
- Check layer mask settings aren't causing issues

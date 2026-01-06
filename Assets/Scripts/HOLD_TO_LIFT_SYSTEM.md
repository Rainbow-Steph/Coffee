# ClickableObject - Hold-to-Lift Pickup System

## Overview
The ClickableObject script has been updated to use a more intuitive "hold-to-lift" mechanic. Objects can now be picked up by clicking and holding the left mouse button, and are automatically dropped when the button is released.

## Changes Made

### 1. Parameter Rename
**Before:**
```csharp
[Header("Float Behavior")]
public bool floatOnClick = false;
public float floatDistance = 2f;
public float floatSpeed = 5f;
public Vector3 floatOffset = Vector3.zero;
```

**After:**
```csharp
[Header("Pick Up Behavior")]
public bool canPickUp = false;
public float holdDistance = 2f;
public float pickupSpeed = 5f;
public Vector3 holdOffset = Vector3.zero;
```

### 2. Behavior Change

#### Old Behavior (Toggle Float):
- **Click once** ? Object floats in front of camera
- **Click again** OR **Right-click** ? Object returns to pickup position
- Toggle mechanic (on/off states)

#### New Behavior (Hold-to-Lift):
- **Click and hold left mouse button** ? Object lifts and follows camera
- **Release left mouse button** ? Object automatically returns to pickup position
- Continuous input required to keep holding

---

## How It Works

### Step 1: Click to Pick Up
```csharp
// When object is clicked with canPickUp enabled
public void OnClicked(RaycastHit hit)
{
    if (canPickUp && !isBeingHeld)
    {
        StartHoldingObject(); // Start lifting
    }
}
```

### Step 2: Hold to Keep Lifted
```csharp
// Update loop continuously checks mouse button state
void Update()
{
    if (canPickUp)
    {
        // If holding object and mouse button released
        if (isBeingHeld && !Input.GetMouseButton(0))
        {
    ReleaseObject(); // Drop immediately
        }
        
 // While held, update position to follow camera
    if (isBeingHeld && mainCamera != null)
     {
            Vector3 targetPosition = mainCamera.transform.position +
        mainCamera.transform.forward * holdDistance +
    mainCamera.transform.TransformDirection(holdOffset);
     
       transform.position = Vector3.Lerp(
            transform.position, 
                targetPosition, 
           Time.deltaTime * pickupSpeed
            );
        }
 }
}
```

### Step 3: Release to Drop
```csharp
// Automatically called when mouse button is released
private void ReleaseObject()
{
    isBeingHeld = false;
    currentlyHeldObject = null;
    
    // Smoothly return to pickup position
    StartCoroutine(ReturnToOriginalPosition());
}
```

---

## Inspector Settings

### Pick Up Behavior Section

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| **Can Pick Up** | `bool` | `false` | Enable hold-to-lift pickup mechanic |
| **Hold Distance** | `float` | `2.0` | Distance from camera to hold object |
| **Pickup Speed** | `float` | `5.0` | Speed of movement to hold position |
| **Rotation Speed** | `float` | `30.0` | Rotation speed while held (degrees/sec) |
| **Rotation Offset** | `Vector3` | `(0,0,0)` | Euler angle offset applied to held object |
| **Hold Offset** | `Vector3` | `(0,0,0)` | Position offset from camera center |

---

## Usage Examples

### Example 1: Basic Pickup Item
```
Can Pick Up: ?
Hold Distance: 2.0
Pickup Speed: 5.0
Rotation Speed: 30.0
Rotation Offset: (0, 0, 0)
Hold Offset: (0, 0, 0)

Result: Object lifts to center of camera view, rotates smoothly
```

### Example 2: Side-Held Tool
```
Can Pick Up: ?
Hold Distance: 1.5
Pickup Speed: 8.0
Rotation Speed: 0 (no rotation)
Rotation Offset: (0, 90, 0)
Hold Offset: (0.5, -0.3, 0) (right and slightly down)

Result: Object held to the side like a tool or flashlight
```

### Example 3: Close Inspection
```
Can Pick Up: ?
Hold Distance: 0.8
Pickup Speed: 3.0
Rotation Speed: 60.0 (faster rotation)
Rotation Offset: (0, 0, 0)
Hold Offset: (0, 0, 0)

Result: Object brought close to camera for detailed viewing
```

### Example 4: Heavy Object
```
Can Pick Up: ?
Hold Distance: 2.5
Pickup Speed: 2.0 (slower)
Rotation Speed: 10.0 (slower rotation)
Rotation Offset: (0, 0, 0)
Hold Offset: (0, -0.5, 0) (lower position)

Result: Object feels heavier with slower movement
```

---

## Key Features

### ? Single-Item System
Only one item can be held at a time across all ClickableObjects:
```csharp
// Static tracking
private static ClickableObject currentlyHeldObject = null;

// Check before pickup
if (currentlyHeldObject != null && currentlyHeldObject != this)
{
    // Cannot pick up - another item is held
    return;
}
```

### ? Automatic Drop on Release
No need to click again or press another button:
```csharp
// Monitors mouse button in Update()
if (isBeingHeld && !Input.GetMouseButton(0))
{
  ReleaseObject(); // Instant response
}
```

### ? Smooth Return Animation
Object smoothly returns to pickup position:
```csharp
// Lerp back over time
while (Vector3.Distance(transform.position, pickupPosition) > 0.01f)
{
    transform.position = Vector3.Lerp(
        transform.position, 
 pickupPosition, 
    Time.deltaTime * returnSpeed
    );
yield return null;
}
```

### ? Collider Management
Collider is disabled while held, re-enabled when dropped:
```csharp
// Disable during pickup
if (objectCollider != null)
{
    objectCollider.enabled = false;
}

// Re-enable after return
objectCollider.enabled = true;
```

### ? Action Tracking Integration
Updates `PlayerActionTracker` when items are picked up/dropped:
```csharp
// On pickup
if (actionTracker != null)
{
    actionTracker.HeldItemName = gameObject.name;
}

// On release
if (actionTracker != null)
{
    actionTracker.HeldItemName = "";
}
```

---

## Comparison: Old vs New

| Aspect | Old (Toggle Float) | New (Hold-to-Lift) |
|--------|-------------------|-------------------|
| **Input** | Click to toggle | Hold left mouse button |
| **Release** | Click again or right-click | Release left mouse button |
| **Mechanic** | Toggle on/off | Continuous hold |
| **Dropped When** | Manual action | Automatic on release |
| **Feel** | Tool-like toggle | Natural grab/release |
| **Accidental Drop** | Harder (must click) | Possible (release early) |
| **Precision** | Same once floating | Must maintain hold |
| **Use Case** | Permanent carry | Temporary interactions |

---

## Migration Guide

### For Existing Projects

If you have objects set up with the old `floatOnClick` system:

**Unity will show warnings** about obsolete parameters in the Inspector. Here's how to migrate:

#### Option 1: Manual Update (Recommended)
1. Open your scene/prefab
2. Select objects with ClickableObject component
3. In Inspector, find **"Pick Up Behavior"** section
4. If you had `floatOnClick = true`:
   - Check "Can Pick Up" ?
   - Adjust "Hold Distance" (was "Float Distance")
 - Adjust "Pickup Speed" (was "Float Speed")
   - Adjust "Hold Offset" (was "Float Offset")

#### Option 2: Script Migration
If you have many objects, add this to a temporary Editor script:

```csharp
// Temporary migration script (Editor only)
[MenuItem("Tools/Migrate Float to Pickup")]
static void MigrateFloatToPickup()
{
    ClickableObject[] allClickables = FindObjectsOfType<ClickableObject>();
    
    foreach (var clickable in allClickables)
    {
        // This assumes old values are still accessible
    // Adjust based on your actual setup
     SerializedObject so = new SerializedObject(clickable);
      
        // Copy old values to new properties
        bool wasFloat = so.FindProperty("floatOnClick").boolValue;
      so.FindProperty("canPickUp").boolValue = wasFloat;
        
        float distance = so.FindProperty("floatDistance").floatValue;
        so.FindProperty("holdDistance").floatValue = distance;
 
     // ... continue for other properties
        
        so.ApplyModifiedProperties();
    }
    
    Debug.Log($"Migrated {allClickables.Length} objects");
}
```

---

## Code Changes Summary

### Methods Renamed/Updated

| Old Method | New Method | Change Type |
|-----------|------------|-------------|
| `ToggleFloat()` | *Removed* | Behavior handled in `OnClicked()` |
| `StartFloating()` | `StartHoldingObject()` | Renamed + logic update |
| `StopFloating()` | `ReleaseObject()` | Renamed + auto-call |
| `ForceReturnToOriginal()` | `ForceReturnToOriginal()` | Updated to call `ReleaseObject()` |

### Variables Renamed

| Old Variable | New Variable |
|-------------|-------------|
| `floatOnClick` | `canPickUp` |
| `floatDistance` | `holdDistance` |
| `floatSpeed` | `pickupSpeed` |
| `floatOffset` | `holdOffset` |
| `isFloating` | `isBeingHeld` |
| `floatingRotationOffset` | `holdingRotationOffset` |

### New Variables

| Variable | Type | Purpose |
|----------|------|---------|
| `isHoldingMouseButton` | `bool` | Tracks mouse button state |

---

## Input Handling Details

### Mouse Button Detection
```csharp
// Update() continuously monitors
if (isBeingHeld && !Input.GetMouseButton(0))
{
    // GetMouseButton(0) = Left mouse button
    // Returns false when button is released
    ReleaseObject();
}
```

### Why This Works Well
- **Immediate Response:** Drop happens the moment button is released
- **Natural Feel:** Mimics real-world grab/release
- **No Extra Keys:** Only uses primary mouse button
- **Clear Intent:** Holding = keeping object, releasing = dropping

### Alternative Input Options

If you want different controls, modify the condition:

**Hold Right Mouse Button:**
```csharp
if (isBeingHeld && !Input.GetMouseButton(1)) // 1 = right button
```

**Hold Specific Key:**
```csharp
if (isBeingHeld && !Input.GetKey(KeyCode.E))
```

**Combination Input:**
```csharp
if (isBeingHeld && (!Input.GetMouseButton(0) || Input.GetKey(KeyCode.Escape)))
{
    // Release on mouse release OR escape key
}
```

---

## Integration with Other Systems

### CameraRaycaster
No changes needed - continues to call `OnClicked()` as before.

### ItemInteractionHandler
Still works - interactions happen on click (when pickup starts).

### PlayerActionTracker
Updated automatically when object is picked up/released:
```csharp
// Pickup sets
actionTracker.HeldItemName = "ObjectName";

// Release clears
actionTracker.HeldItemName = "";
```

### HeldItemDisplay
Will show the held item's name while button is held.

### CoffeeMaker
Still triggers when machine is clicked (independent of pickup).

---

## Best Practices

### 1. **Set Appropriate Hold Distance**
- **Close (0.5-1.0):** Inspection items, tools
- **Medium (1.5-2.5):** General items, ingredients
- **Far (3.0+):** Large objects, decorative items

### 2. **Adjust Pickup Speed**
- **Slow (1-3):** Heavy objects, careful movements
- **Medium (4-6):** Standard items
- **Fast (7-10):** Light objects, quick interactions

### 3. **Use Hold Offset Wisely**
```csharp
// Center view (default)
Hold Offset: (0, 0, 0)

// Lower position (realistic carry)
Hold Offset: (0, -0.5, 0)

// Side position (tool belt style)
Hold Offset: (0.5, -0.3, 0)
```

### 4. **Consider Rotation**
```csharp
// Rotating items (interesting/dynamic)
Rotation Speed: 30-60

// Static items (easier to see details)
Rotation Speed: 0

// Slow rotation (subtle)
Rotation Speed: 10-20
```

### 5. **Feedback to Player**
- Enable `showDebugInfo` during development
- Use audio feedback (`playSoundOnClick`)
- Add visual feedback (`changeColorOnClick`)
- Consider adding haptic feedback on supported devices

---

## Troubleshooting

### Issue: Object Doesn't Lift
**Check:**
- ? "Can Pick Up" is enabled
- ? Camera.main is found in scene
- ? Object has Collider component
- ? CameraRaycaster is in the scene

### Issue: Object Drops Immediately
**Cause:** Mouse button not being held or input not detected

**Solutions:**
- Make sure you're holding left mouse button after clicking
- Check Input settings (Edit ? Project Settings ? Input Manager)
- Verify mouse button 0 is configured correctly

### Issue: Object Returns to Wrong Position
**Cause:** Transform changes after pickup

**Solution:**
Pickup position is stored when clicked. If object moves before being returned, it returns to the *pickup* position, not the *original spawn* position. This is by design.

### Issue: Multiple Objects Can Be Held
**This should not happen** - the system prevents it.

**If it does:**
- Check for multiple ClickableObject instances
- Verify `currentlyHeldObject` tracking
- Look for script modifications

### Issue: Object Gets Stuck
**Cause:** Coroutine interrupted or error in return logic

**Solution:**
Call `ForceReturnToOriginal()` to reset:
```csharp
// From another script
clickableObject.ForceReturnToOriginal();
```

---

## Debug Information

When `showDebugInfo` is enabled, you'll see:

**On Pickup:**
```
ClickableObject: CoffeeCup picked up from position (1.2, 0.5, 3.4). Currently held: CoffeeCup
```

**On Release:**
```
ClickableObject: CoffeeCup - Left mouse released, dropping object
ClickableObject: CoffeeCup released, returning to original position. Currently held: 
```

**On Return Complete:**
```
ClickableObject: CoffeeCup returned to pickup position (1.2, 0.5, 3.4)
```

**If Another Object Held:**
```
ClickableObject: Cannot pick up Spoon - CoffeeCup is already being held!
```

---

## Summary

**Key Changes:**
- ? Renamed "Float on Click" ? "Can Pick Up"
- ? Changed toggle mechanic ? hold-to-lift mechanic
- ? Automatic drop on mouse release
- ? More intuitive and natural interaction
- ? Better player feedback
- ? Clearer parameter names

**Result:**
A more intuitive and natural object interaction system that feels like actually picking up and holding items in a virtual space!

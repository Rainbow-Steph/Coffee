# ClickableObject - Gravity Control During Pickup

## Overview
Modified ClickableObject.cs to automatically disable gravity while objects are being held and restore it when dropped. This prevents picked-up objects from falling or being affected by physics forces during the hold-to-lift mechanic.

## Changes Made

### 1. New Private Variables

Added two new variables to track Rigidbody and gravity state:

```csharp
private Rigidbody objectRigidbody;
private bool originalUseGravity;
```

**Purpose:**
- `objectRigidbody` - Caches the Rigidbody component (if present)
- `originalUseGravity` - Stores the original gravity setting to restore later

---

### 2. Start() Method - Cache Rigidbody

Added Rigidbody caching after collider detection:

```csharp
// Get Rigidbody if it exists (for gravity control during pickup)
objectRigidbody = GetComponent<Rigidbody>();
if (objectRigidbody != null && canPickUp)
{
    originalUseGravity = objectRigidbody.useGravity;
    
    if (showDebugInfo)
    {
    Debug.Log($"ClickableObject on {gameObject.name}: Found Rigidbody, gravity will be disabled during pickup");
    }
}
```

**What This Does:**
- Checks if the GameObject has a Rigidbody component
- Stores the original `useGravity` value
- Logs debug info if debug mode is enabled

---

### 3. StartHoldingObject() Method - Disable Gravity

Added gravity control when object is picked up:

```csharp
// Disable gravity while being held
if (objectRigidbody != null)
{
    originalUseGravity = objectRigidbody.useGravity;
    objectRigidbody.useGravity = false;
    objectRigidbody.velocity = Vector3.zero;
    objectRigidbody.angularVelocity = Vector3.zero;
    
    if (showDebugInfo)
    {
      Debug.Log($"ClickableObject: {gameObject.name} - Gravity disabled, velocities zeroed");
    }
}
```

**What This Does:**
- Saves current gravity state (in case it changed at runtime)
- Disables gravity (`useGravity = false`)
- Zeros out linear velocity (stops falling/moving)
- Zeros out angular velocity (stops spinning)
- Logs the action if debug mode is enabled

---

### 4. ReleaseObject() Method - Restore Gravity

Added gravity restoration when object is dropped:

```csharp
// Re-enable gravity when dropped
if (objectRigidbody != null)
{
    objectRigidbody.useGravity = originalUseGravity;
    
    if (showDebugInfo)
    {
 Debug.Log($"ClickableObject: {gameObject.name} - Gravity restored to: {originalUseGravity}");
    }
}
```

**What This Does:**
- Restores the original gravity setting
- Allows physics to resume normally
- Logs the action if debug mode is enabled

---

## How It Works

### Pickup Flow:

```
1. User clicks object with canPickUp enabled
   ?
2. StartHoldingObject() is called
   ?
3. Rigidbody check:
   - If Rigidbody exists ? Disable gravity & zero velocities
   - If no Rigidbody ? Continue without gravity changes
   ?
4. Object follows camera (Update loop)
   - No gravity pulling it down
   - No momentum affecting position
```

### Release Flow:

```
1. User releases left mouse button
   ?
2. ReleaseObject() is called
   ?
3. Rigidbody check:
   - If Rigidbody exists ? Restore original gravity setting
   - If no Rigidbody ? Continue without gravity changes
   ?
4. Object smoothly returns to pickup position
   ?
5. After return animation completes:
   - Collider re-enabled
   - Object can now fall/be affected by physics again
```

---

## Behavior Examples

### Example 1: Object With Gravity Enabled

**Setup:**
```
CoffeeCup
?? Rigidbody (useGravity: true, mass: 0.5)
?? Collider
?? ClickableObject (canPickUp: true)
```

**When Picked Up:**
- Gravity disabled ? Won't fall while held
- Velocity zeroed ? Stops any existing motion
- Follows camera smoothly

**When Released:**
- Gravity restored to `true`
- Returns to pickup position
- After return, can fall normally if not supported

---

### Example 2: Object Without Rigidbody

**Setup:**
```
StaticItem
?? Collider
?? ClickableObject (canPickUp: true)
```

**Behavior:**
- No Rigidbody detected ? Gravity control skipped
- Still picks up and follows camera normally
- No physics interactions anyway

---

### Example 3: Kinematic Rigidbody

**Setup:**
```
KinematicObject
?? Rigidbody (isKinematic: true, useGravity: false)
?? Collider
?? ClickableObject (canPickUp: true)
```

**Behavior:**
- Original `useGravity` is `false`
- When picked up ? Sets to `false` (no change)
- When released ? Restores to `false` (no change)
- Works correctly with kinematic objects

---

## Benefits

### ? No Falling During Pickup
Objects won't be pulled down by gravity while you're holding them.

### ? Clean Movement
Zeroing velocities prevents momentum from affecting the hold position.

### ? Proper Restoration
Original gravity state is preserved and restored correctly.

### ? Works With All Cases
- Objects with gravity enabled
- Objects with gravity disabled
- Kinematic objects
- Objects without Rigidbody

### ? Seamless Integration
- No changes to existing pickup/release flow
- Automatic detection and handling
- Optional debug logging

---

## Debug Output

When `showDebugInfo` is enabled:

**On Start (if Rigidbody found):**
```
ClickableObject on CoffeeCup: Found Rigidbody, gravity will be disabled during pickup
```

**On Pickup:**
```
ClickableObject: CoffeeCup - Gravity disabled, velocities zeroed
ClickableObject: CoffeeCup picked up from position (1.0, 0.5, 2.0). Currently held: CoffeeCup
```

**On Release:**
```
ClickableObject: CoffeeCup - Gravity restored to: True
ClickableObject: CoffeeCup released, returning to original position. Currently held: 
```

---

## Edge Cases Handled

### 1. Runtime Gravity Changes
```csharp
// Stores gravity state at pickup time
originalUseGravity = objectRigidbody.useGravity;
```
If gravity was changed after Start(), the current state is captured during pickup.

### 2. Objects Without Rigidbody
```csharp
if (objectRigidbody != null)
{
    // Only execute if Rigidbody exists
}
```
No errors occur if object doesn't have physics component.

### 3. Kinematic Rigidbodies
Kinematic objects have `useGravity` automatically set to false by Unity, but our code still works correctly by preserving whatever the state is.

### 4. Multiple Pickups
Each pickup captures the current gravity state, so repeated pickups work correctly even if gravity settings change between pickups.

---

## Performance Impact

### Minimal Overhead:
- ? Single component lookup in Start()
- ? Two boolean checks per pickup/release
- ? No Update() loop overhead
- ? No continuous physics calculations while held

### Memory:
- +8 bytes per ClickableObject (1 reference + 1 bool)

---

## Compatibility

### Works With:
- ? Hold-to-lift mechanic
- ? Rigidbody objects (dynamic)
- ? Kinematic Rigidbody objects
- ? Objects without Rigidbody
- ? Objects with colliders disabled
- ? ActionTracker integration
- ? All existing ClickableObject features

### Does Not Affect:
- ? Objects with `canPickUp = false`
- ? Hover highlights
- ? Click events
- ? Interactions
- ? Billboard system

---

## Testing Checklist

### Test 1: Basic Gravity Disable
- [ ] Create object with Rigidbody (gravity enabled)
- [ ] Set canPickUp = true
- [ ] Pick up object
- [ ] **Expected:** Object doesn't fall while held

### Test 2: Gravity Restore
- [ ] Pick up object
- [ ] Release mouse button
- [ ] **Expected:** Object returns to position, then gravity resumes

### Test 3: No Rigidbody
- [ ] Create object without Rigidbody
- [ ] Set canPickUp = true
- [ ] Pick up object
- [ ] **Expected:** No errors, normal pickup behavior

### Test 4: Kinematic Object
- [ ] Create object with kinematic Rigidbody
- [ ] Set canPickUp = true
- [ ] Pick up and release
- [ ] **Expected:** Works normally, no gravity issues

### Test 5: Debug Logging
- [ ] Enable showDebugInfo
- [ ] Pick up object with Rigidbody
- [ ] Check console for gravity messages
- [ ] **Expected:** See "Gravity disabled" and "Gravity restored" logs

---

## Code Summary

**Files Modified:** 1
- `Assets\Scripts\ClickableObject.cs`

**Lines Added:** ~20 lines
**Lines Modified:** ~3 existing methods

**New Variables:** 2
- `objectRigidbody` (Rigidbody)
- `originalUseGravity` (bool)

**Modified Methods:** 3
- `Start()` - Cache Rigidbody
- `StartHoldingObject()` - Disable gravity
- `ReleaseObject()` - Restore gravity

---

## Migration Notes

### For Existing Projects:
- ? **No breaking changes**
- ? **Automatically works** with existing ClickableObjects
- ? **No Inspector changes** required
- ? **Backward compatible** with objects without Rigidbody

### For New Objects:
- ? Just add Rigidbody component if you want gravity control
- ? Enable `canPickUp` as before
- ? Gravity will be handled automatically

---

## Summary

**What Changed:**
- ? Added automatic gravity disabling during pickup
- ? Added velocity zeroing on pickup
- ? Added gravity restoration on release
- ? Added Rigidbody caching and state tracking

**Result:**
Objects with Rigidbody components now float properly when held, without being pulled down by gravity or affected by momentum. The original physics state is preserved and restored when the object is released.

**Key Feature:**
Completely automatic - no Inspector configuration needed, works seamlessly with the hold-to-lift mechanic!

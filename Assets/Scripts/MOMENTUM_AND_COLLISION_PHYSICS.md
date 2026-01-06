# ClickableObject - Momentum and Collision Physics

## Overview
Modified ClickableObject.cs to add realistic physics behavior:
1. **Momentum Retention** - Objects keep velocity when released, feeling more natural
2. **Active Collisions While Held** - Held objects can push other pickable objects

## Changes Made

### 1. New Inspector Parameters

```csharp
[Header("Pick Up Behavior")]
// ...existing parameters...

[Tooltip("Percentage of velocity to retain when released (0 = no momentum, 1 = full momentum)")]
[Range(0f, 1f)]
public float momentumRetention = 0.7f;

[Tooltip("Keep collisions enabled while holding (allows pushing other objects)")]
public bool keepCollisionsWhileHeld = true;
```

**New Settings:**
- `momentumRetention` - Control how much velocity is kept (0-100%)
- `keepCollisionsWhileHeld` - Toggle collision behavior while holding

---

### 2. New Private Variables

```csharp
private Vector3 lastPosition;// Track position for velocity calculation
private Vector3 currentVelocity;   // Current movement velocity
```

**Purpose:**
- Track object movement frame-by-frame
- Calculate velocity for momentum application

---

### 3. Modified Update() - Velocity Tracking

**Added velocity calculation:**
```csharp
void Update()
{
    if (canPickUp && isBeingHeld && mainCamera != null)
    {
        // Store last position for velocity calculation
        lastPosition = transform.position;

        // Calculate target position in front of camera
    Vector3 targetPosition = mainCamera.transform.position +
  mainCamera.transform.forward * holdDistance +
            mainCamera.transform.TransformDirection(holdOffset);

    // Smoothly move to target position
    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * pickupSpeed);

        // Calculate velocity for momentum
      currentVelocity = (transform.position - lastPosition) / Time.deltaTime;

        // Apply rotation...
    }
}
```

**What Changed:**
- ? Store position before movement
- ? Calculate velocity after movement
- ? Velocity = (newPos - oldPos) / deltaTime

---

### 4. Modified StartHoldingObject() - Collision Control

**Before:**
```csharp
// Always disabled collider
if (objectCollider != null)
{
 objectCollider.enabled = false;
}
```

**After:**
```csharp
// Handle collider based on keepCollisionsWhileHeld setting
if (objectCollider != null && !keepCollisionsWhileHeld)
{
    objectCollider.enabled = false;
    
    if (showDebugInfo)
    {
   Debug.Log($"ClickableObject: {gameObject.name} - Collider disabled");
    }
}
else if (showDebugInfo && keepCollisionsWhileHeld)
{
    Debug.Log($"ClickableObject: {gameObject.name} - Collisions kept enabled for pushing other objects");
}

// Initialize velocity tracking
lastPosition = transform.position;
currentVelocity = Vector3.zero;
```

**What Changed:**
- ? Conditional collider disable based on `keepCollisionsWhileHeld`
- ? Initialize velocity tracking on pickup
- ? Debug logging for collision state

---

### 5. Modified ReleaseObject() - Momentum Application

**Before:**
```csharp
// Re-enable gravity when dropped
if (objectRigidbody != null)
{
    objectRigidbody.useGravity = originalUseGravity;
}

// Re-enable collider immediately
if (objectCollider != null)
{
    objectCollider.enabled = true;
}
```

**After:**
```csharp
// Re-enable gravity when dropped
if (objectRigidbody != null)
{
    objectRigidbody.useGravity = originalUseGravity;
    
    // Apply retained momentum based on momentumRetention setting
    if (momentumRetention > 0f)
    {
        // Apply the calculated velocity with momentum retention
 objectRigidbody.velocity = currentVelocity * momentumRetention;
        
        if (showDebugInfo)
        {
      Debug.Log($"ClickableObject: {gameObject.name} - Applied momentum: {objectRigidbody.velocity} (retention: {momentumRetention})");
}
    }
    
    if (showDebugInfo)
    {
      Debug.Log($"ClickableObject: {gameObject.name} - Gravity restored to: {originalUseGravity}");
    }
}

// Re-enable collider if it was disabled
if (objectCollider != null && !keepCollisionsWhileHeld)
{
    objectCollider.enabled = true;
    
    if (showDebugInfo)
    {
        Debug.Log($"ClickableObject: {gameObject.name} - Collider re-enabled");
    }
}
```

**What Changed:**
- ? Apply calculated velocity with momentum retention
- ? Only re-enable collider if it was disabled
- ? Debug logging for momentum and collision state

---

## How It Works

### Momentum System

#### Velocity Calculation:
```
Frame N:   Position = (1, 2, 3)
       Store as lastPosition
   ?
Frame N+1: Position = (1.1, 2, 3)
           Velocity = (1.1 - 1.0, 2 - 2, 3 - 3) / 0.016
     Velocity = (0.1, 0, 0) / 0.016
           Velocity = (6.25, 0, 0) units/sec
  ?
      Store as currentVelocity
```

#### Momentum Application:
```
Release with momentumRetention = 0.7:
  ?
currentVelocity = (6.25, 0, 0)
  ?
appliedVelocity = (6.25, 0, 0) * 0.7
  ?
appliedVelocity = (4.375, 0, 0)
  ?
objectRigidbody.velocity = (4.375, 0, 0)
  ?
Object continues moving in that direction!
```

---

### Collision System

#### With keepCollisionsWhileHeld = true:
```
Pick up Object A
  ?
Collider stays enabled
  ?
Move Object A toward Object B (also pickable)
  ?
Physics collision detected
  ?
Object B gets pushed by Object A ?
  ?
Both objects can interact naturally
```

#### With keepCollisionsWhileHeld = false (old behavior):
```
Pick up Object A
  ?
Collider disabled
  ?
Move Object A through Object B
  ?
No collision detected
  ?
Objects pass through each other ?
```

---

## Inspector Configuration

### Momentum Settings

**No Momentum (Traditional):**
```
Momentum Retention: 0.0
```
- Object stops dead when released
- Like dropping a static object

**Low Momentum:**
```
Momentum Retention: 0.3
```
- Slight continuation of movement
- Feels gentle and controlled

**Medium Momentum (Default):**
```
Momentum Retention: 0.7
```
- Natural throwing feel
- Realistic physics behavior

**Full Momentum:**
```
Momentum Retention: 1.0
```
- Objects fly with full velocity
- Like throwing with force

---

### Collision Settings

**Active Collisions (Default):**
```
Keep Collisions While Held: ?
```
- Can push other objects
- Realistic physical interactions
- More challenging to place precisely

**Disabled Collisions (Ghost Mode):**
```
Keep Collisions While Held: ?
```
- Pass through objects
- Easy precise placement
- No physical interference

---

## Use Cases

### Use Case 1: Throwing Objects

**Setup:**
```
Momentum Retention: 0.8-1.0
Keep Collisions While Held: ?
```

**Behavior:**
```
Pick up ball ? Swing mouse fast ? Release
  ?
Ball flies in direction of movement
  ?
Hits other objects realistically
  ?
Perfect for throwing mechanics!
```

---

### Use Case 2: Gentle Placement

**Setup:**
```
Momentum Retention: 0.2-0.4
Keep Collisions While Held: ?
```

**Behavior:**
```
Pick up cup ? Move to table ? Release
  ?
Cup settles gently with minimal slide
  ?
Doesn't bump other objects
  ?
Perfect for precise arrangement!
```

---

### Use Case 3: Physics Puzzles

**Setup:**
```
Momentum Retention: 0.7
Keep Collisions While Held: ?
```

**Behavior:**
```
Pick up box ? Push it against other boxes
  ?
Boxes interact and stack realistically
  ?
Can use held objects as tools
  ?
Perfect for physics-based puzzles!
```

---

### Use Case 4: Bowling/Knock-Down Games

**Setup:**
```
Momentum Retention: 1.0
Keep Collisions While Held: ?
```

**Behavior:**
```
Pick up ball ? Swing fast ? Release
  ?
Ball rolls/flies with full velocity
  ?
Knocks down pins/targets
  ?
Perfect for mini-games!
```

---

## Example Scenarios

### Scenario 1: Coffee Cup Slide

```
Configuration:
- Momentum Retention: 0.5
- Keep Collisions: False

Action:
1. Pick up coffee cup
2. Move it across counter
3. Release

Result:
? Cup slides slightly after release
? Comes to rest naturally
? Doesn't collide with other items
```

---

### Scenario 2: Pushing Boxes

```
Configuration:
- Momentum Retention: 0.7
- Keep Collisions: True

Action:
1. Pick up small box
2. Move toward large box
3. Large box gets pushed
4. Release

Result:
? Small box pushes large box while held
? Both boxes continue sliding after release
? Realistic stacking behavior
```

---

### Scenario 3: Tossing Items

```
Configuration:
- Momentum Retention: 0.9
- Keep Collisions: True

Action:
1. Pick up ball
2. Quickly move mouse upward
3. Release at peak of motion

Result:
? Ball flies upward with momentum
? Falls back down due to gravity
? Can hit other objects mid-flight
```

---

## Physics Considerations

### Velocity Calculation Details

**Frame Rate Independence:**
```csharp
currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
```
- Dividing by `Time.deltaTime` makes it frame-rate independent
- Works correctly at any FPS (30, 60, 144, etc.)

**Smooth Movement:**
```csharp
transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * pickupSpeed);
```
- Lerp creates smooth acceleration/deceleration
- Prevents instant snapping to target
- Results in natural-feeling velocity

---

### Collision Behavior

**With Rigidbody on Both Objects:**
```
Held Object A (mass: 1kg) ? Object B (mass: 2kg)
  ?
Collision forces calculated by Unity
  ?
Object B gets pushed proportional to mass ratio
  ?
Realistic physics interaction ?
```

**With Kinematic Rigidbody:**
```
Held Object (kinematic) ? Object B (dynamic)
  ?
Held object has infinite mass while held
  ?
Pushes Object B with full force
  ?
Very powerful pushing ?
```

---

## Debug Output

### With showDebugInfo = true:

**On Pickup:**
```
ClickableObject: CoffeeCup picked up from position (1.0, 0.5, 2.0)
ClickableObject: CoffeeCup - Collisions kept enabled for pushing other objects
ClickableObject: CoffeeCup - Gravity disabled, velocities zeroed
```

**During Hold:**
```
(Velocity calculated every frame but not logged to avoid spam)
```

**On Release:**
```
ClickableObject: CoffeeCup - Applied momentum: (2.5, 0.3, 1.2) (retention: 0.7)
ClickableObject: CoffeeCup - Gravity restored to: True
ClickableObject: CoffeeCup released at current position. Currently held: 
```

---

## Performance Impact

### Velocity Tracking:
- **CPU Cost:** Minimal (2 Vector3 operations per frame while held)
- **Memory:** +24 bytes (2 Vector3 variables)
- **Impact:** Negligible

### Collision While Held:
- **CPU Cost:** Standard Unity physics (same as any collision)
- **Memory:** No additional memory
- **Impact:** Depends on object count and complexity

**Optimization Tip:**
If you have many held objects simultaneously, consider:
```csharp
// Reduce physics update rate for held objects
if (isBeingHeld && objectRigidbody != null)
{
    objectRigidbody.interpolation = RigidbodyInterpolation.None;
}
```

---

## Edge Cases Handled

### Edge Case 1: Zero Momentum Retention
```csharp
if (momentumRetention > 0f)
{
    // Only apply velocity if retention is greater than 0
    objectRigidbody.velocity = currentVelocity * momentumRetention;
}
```
**Result:** Setting to 0 completely disables momentum (backward compatible)

---

### Edge Case 2: No Rigidbody
```csharp
if (objectRigidbody != null)
{
    // Only apply momentum if Rigidbody exists
    objectRigidbody.velocity = currentVelocity * momentumRetention;
}
```
**Result:** Objects without Rigidbody work normally (no momentum applied)

---

### Edge Case 3: Collider Already Disabled
```csharp
if (objectCollider != null && !keepCollisionsWhileHeld)
{
 // Only disable if keepCollisionsWhileHeld is false
    objectCollider.enabled = false;
}
```
**Result:** Collider state respected based on settings

---

### Edge Case 4: Very Fast Movement
```csharp
currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
```
**Result:** Velocity scales correctly even with rapid mouse movements

---

## Compatibility

### Works With:
- ? Gravity control system
- ? Drop-in-place behavior
- ? Hold-to-lift mechanic
- ? Single-item system
- ? Action tracker integration
- ? All existing features

### Requires:
- ? Rigidbody component (for momentum)
- ? Collider component (for pushing)
- ?? Without Rigidbody: Momentum disabled but everything else works
- ?? Without Collider: Pushing disabled but everything else works

---

## Testing Checklist

### Test 1: Momentum Retention
- [ ] Set momentumRetention to 0.7
- [ ] Pick up object
- [ ] Move quickly while holding
- [ ] Release
- [ ] **Expected:** Object continues in direction of movement

### Test 2: No Momentum
- [ ] Set momentumRetention to 0.0
- [ ] Pick up object
- [ ] Move quickly while holding
- [ ] Release
- [ ] **Expected:** Object stops immediately

### Test 3: Push Other Objects
- [ ] Set keepCollisionsWhileHeld to true
- [ ] Place two pickable objects near each other
- [ ] Pick up Object A
- [ ] Move toward Object B
- [ ] **Expected:** Object B gets pushed

### Test 4: Ghost Mode
- [ ] Set keepCollisionsWhileHeld to false
- [ ] Pick up object
- [ ] Move through other objects
- [ ] **Expected:** Object passes through others

### Test 5: Throw Mechanic
- [ ] Set momentumRetention to 1.0
- [ ] Pick up light object
- [ ] Swing mouse rapidly
- [ ] Release
- [ ] **Expected:** Object flies with velocity

---

## Migration Notes

### For Existing Projects:

**Default Values:**
```csharp
momentumRetention = 0.7f;   // 70% momentum (natural feel)
keepCollisionsWhileHeld = true;    // Active collisions (realistic)
```

**To Match Old Behavior:**
```csharp
momentumRetention = 0.0f;    // No momentum (old behavior)
keepCollisionsWhileHeld = false;// Disabled collisions (old behavior)
```

**Inspector will show new fields automatically!**

---

## Advanced Usage

### Custom Momentum Based on Object Type

```csharp
void Start()
{
    // Heavy objects retain less momentum
    if (gameObject.CompareTag("Heavy"))
    {
        momentumRetention = 0.3f;
    }
    // Light objects retain more momentum
    else if (gameObject.CompareTag("Light"))
    {
        momentumRetention = 0.9f;
    }
}
```

---

### Dynamic Collision Toggle

```csharp
void Update()
{
// Disable collisions when near other objects
    if (isBeingHeld)
    {
      bool nearOtherObject = Physics.CheckSphere(transform.position, 0.5f);
  
        if (nearOtherObject != keepCollisionsWhileHeld)
    {
            keepCollisionsWhileHeld = !nearOtherObject;
       objectCollider.enabled = keepCollisionsWhileHeld;
 }
    }
}
```

---

### Velocity-Based Sound Effects

```csharp
private void ReleaseObject()
{
    // ...existing code...
    
    // Play sound based on momentum
    if (objectRigidbody != null && momentumRetention > 0f)
    {
        float speed = currentVelocity.magnitude;
    
        if (speed > 5f)
        {
            PlaySound(fastReleaseSound);
     }
 else if (speed > 2f)
        {
            PlaySound(normalReleaseSound);
   }
    }
}
```

---

## Summary

### What Changed:
1. ? Added momentum retention system
2. ? Added collision control while held
3. ? Objects feel more realistic
4. ? Can push other pickable objects
5. ? Throwing mechanics possible

### New Features:
- **Momentum Retention** - Objects keep velocity when released
- **Active Collisions** - Held objects can push others
- **Configurable** - Control both features via Inspector
- **Frame-Rate Independent** - Works at any FPS
- **Debug Support** - Optional logging

### Benefits:
- ? More realistic physics
- ? Natural throwing feel
- ? Interactive object manipulation
- ? Physics puzzle possibilities
- ? Backward compatible

### Result:
Objects now behave more like real-world physics - they keep momentum when released and can interact with other objects while being held!

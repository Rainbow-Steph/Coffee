# Coffee Launch System - Dynamic Item Spawning

## Overview
The CoffeeMaker now spawns crafted items with velocity and direction, making them launch from the spawn point instead of just appearing. This creates a more dynamic and realistic coffee-making experience.

## New Features

### 1. Physics-Based Launching
- Items spawn with initial velocity
- Direction can be customized
- Rotation is applied based on spawn point
- Random variation for natural movement

### 2. Configurable Settings
All launch parameters can be adjusted in the Inspector without code changes.

---

## Inspector Settings

### Launch Settings (New Section)

```
Launch Settings
?? Launch Direction: (0, 0.5, 1)
?  ?? Direction vector relative to spawn point
?     - X: Left(-) / Right(+)
?     - Y: Down(-) / Up(+)
?     - Z: Back(-) / Forward(+)
?
?? Launch Force: 5
??? Strength of the launch (units per second)
?
?? Add Random Variation: ?
?  ?? Adds randomness to each spawn
?
?? Max Random Angle: 15
   ?? Maximum random deviation in degrees
```

---

## How It Works

### Step 1: Calculate Spawn Transform
```csharp
if (spawnPoint != null)
{
    // Use spawn point's position and rotation
    spawnPosition = spawnPoint.position;
    spawnRotation = spawnPoint.rotation;
    
 // Transform launch direction relative to spawn point
    finalLaunchDirection = spawnPoint.TransformDirection(launchDirection);
}
else
{
    // Fallback to CoffeeMaker position + offset
    spawnPosition = transform.position + defaultSpawnOffset;
    spawnRotation = Quaternion.identity;
 finalLaunchDirection = launchDirection;
}
```

### Step 2: Add Random Variation (Optional)
```csharp
if (addRandomVariation && maxRandomAngle > 0)
{
    // Create random cone of variation
 float randomX = Random.Range(-maxRandomAngle, maxRandomAngle);
    float randomY = Random.Range(-maxRandomAngle, maxRandomAngle);
    Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);
    finalLaunchDirection = randomRotation * finalLaunchDirection;
}
```

### Step 3: Spawn and Launch
```csharp
// Instantiate at calculated position/rotation
GameObject craftedItem = Instantiate(prefab, spawnPosition, spawnRotation);

// Apply physics if Rigidbody exists
Rigidbody rb = craftedItem.GetComponent<Rigidbody>();
if (rb != null)
{
    // Apply velocity = direction * force
    rb.velocity = finalLaunchDirection * launchForce;
    
    // Add spin for realism
    if (addRandomVariation)
    {
      rb.angularVelocity = new Vector3(
        Random.Range(-2f, 2f),
            Random.Range(-2f, 2f),
Random.Range(-2f, 2f)
        );
    }
}
```

---

## Setup Guide

### Prerequisites

Your crafted item prefabs MUST have:
- ? **Rigidbody component** (for physics)
- ? **Collider component** (for collision detection)

Without a Rigidbody, items will spawn normally but won't launch with velocity.

### Step 1: Setup Spawn Point

1. **Create Empty GameObject** in scene
2. **Name it** "CoffeeSpawnPoint"
3. **Position it** where you want items to launch from
4. **Rotate it** to face the desired launch direction
   - Forward (blue arrow) = primary launch direction

**Example Positions:**
```
Machine at (0, 1, 0)
Spawn Point Options:
?? Above Machine: (0, 1.5, 0) - Items drop down
?? In Front: (0, 1, 0.5) - Items launch forward
?? Side Exit: (0.5, 1, 0) - Items shoot to the side
?? Angled: (0, 1.2, 0.3) + Rotation(30°, 0, 0) - Arc trajectory
```

### Step 2: Configure CoffeeMaker

Select your CoffeeMaker GameObject in Hierarchy:

```
Spawn Settings
?? Spawn Point: [Drag CoffeeSpawnPoint here]
?? Default Spawn Offset: (0, 1, 0) [fallback if no spawn point]

Launch Settings
?? Launch Direction: (0, 0.5, 1) [upward + forward arc]
?? Launch Force: 5 [moderate speed]
?? Add Random Variation: ? [enabled for variety]
?? Max Random Angle: 15 [slight randomness]
```

### Step 3: Setup Prefabs

For each crafted item prefab:

1. **Add Rigidbody:**
   - Add Component ? Physics ? Rigidbody
   - Mass: 0.5 (light item)
   - Drag: 0.5 (some air resistance)
   - Angular Drag: 0.5
   - Use Gravity: ?

2. **Add Collider:**
   - Box Collider, Sphere Collider, or Capsule Collider
   - Adjust size to match model
   - Is Trigger: ? (unless you want pass-through)

3. **Optional - Add ClickableObject:**
   - So player can pick up crafted coffee
   - Set Item Type appropriately

**Example Prefab Setup:**
```
CoffeCup_Prefab
?? Mesh Filter (visual model)
?? Mesh Renderer (materials)
?? Rigidbody
?  ?? Mass: 0.5
?  ?? Drag: 0.5
?  ?? Angular Drag: 0.5
?  ?? Use Gravity: ?
?? Capsule Collider
?  ?? Radius: 0.1
?  ?? Height: 0.2
?? ClickableObject (optional)
   ?? Item Type: Craftable
```

---

## Launch Direction Examples

### Example 1: Straight Forward
```
Launch Direction: (0, 0, 1)
Launch Force: 3
Result: Items shoot straight forward at medium speed
```

### Example 2: Upward Arc
```
Launch Direction: (0, 1, 1)
Launch Force: 5
Result: Items launch in an arc (classic vending machine style)
```

### Example 3: High Pop
```
Launch Direction: (0, 1, 0)
Launch Force: 8
Result: Items pop straight up, then fall down
```

### Example 4: Side Dispenser
```
Launch Direction: (1, 0.2, 0)
Launch Force: 4
Result: Items slide out to the right with slight lift
```

### Example 5: Gentle Drop
```
Launch Direction: (0, -0.5, 0.5)
Launch Force: 2
Result: Items gently drop forward onto counter
```

---

## Random Variation

### Purpose
Prevents identical launches - makes each spawn unique and more natural.

### How It Works

**Without Variation:**
```
Spawn 1: ? ? ?  [Same trajectory]
Spawn 2: ? ? ?  [Same trajectory]
Spawn 3: ? ? ?  [Same trajectory]
```

**With Variation:**
```
Spawn 1: ? ? ?  [Slight right]
Spawn 2: ? ? ?  [Slight up]
Spawn 3: ? ? ?  [Slight down]
```

### Configuration

```
Max Random Angle: 0°   ? No variation (precise)
Max Random Angle: 10°  ? Slight variation
Max Random Angle: 20°  ? Moderate variation
Max Random Angle: 30°+ ? Chaotic variation
```

**Recommended Values:**
- **Coffee Machine:** 10-15° (controlled dispenser)
- **Vending Machine:** 15-25° (typical variation)
- **Explosion/Burst:** 30-45° (items scatter)

---

## Angular Velocity (Spin)

When random variation is enabled, items also get random spin:

```csharp
rb.angularVelocity = new Vector3(
    Random.Range(-2f, 2f),  // Pitch
    Random.Range(-2f, 2f),  // Yaw
    Random.Range(-2f, 2f)   // Roll
);
```

This creates natural tumbling motion during flight.

**To adjust spin:**
Change the range values in `SpawnCraftedItem()`:
- `(-1f, 1f)` ? Slow spin
- `(-2f, 2f)` ? Default spin
- `(-5f, 5f)` ? Fast spin
- `(0, 0)` ? No spin

---

## Testing Scenarios

### Test 1: Basic Launch
1. Setup spawn point facing forward
2. Launch Direction: (0, 0.5, 1)
3. Launch Force: 5
4. Make coffee
5. **Expected:** Item launches in upward arc

### Test 2: Variation Check
1. Enable Random Variation
2. Max Random Angle: 20
3. Make coffee 5 times
4. **Expected:** Each item takes slightly different path

### Test 3: No Rigidbody Warning
1. Create prefab WITHOUT Rigidbody
2. Make coffee
3. **Expected:** 
   - Item spawns normally
   - Console warning: "has no Rigidbody - cannot apply launch velocity!"

### Test 4: Spawn Point Rotation
1. Rotate spawn point 45° on Y-axis
2. Make coffee
3. **Expected:** Launch direction follows spawn point rotation

### Test 5: High Force
1. Launch Force: 15
2. Make coffee
3. **Expected:** Item launches with high velocity (may fly far!)

---

## Troubleshooting

### Issue: Items Don't Launch (Just Drop)

**Cause:** Prefab missing Rigidbody

**Solution:**
1. Open prefab in Prefab Mode
2. Add Component ? Physics ? Rigidbody
3. Save prefab
4. Test again

---

### Issue: Items Fly Too Far

**Cause:** Launch force too high

**Solutions:**
- **Option A:** Reduce Launch Force (try 3-4)
- **Option B:** Increase Rigidbody Drag (try 1.0)
- **Option C:** Reduce Y component of Launch Direction

---

### Issue: Items Spin Wildly

**Cause:** High angular velocity

**Solution:**
In `SpawnCraftedItem()`, reduce angular velocity range:
```csharp
rb.angularVelocity = new Vector3(
    Random.Range(-0.5f, 0.5f),  // Reduced
    Random.Range(-0.5f, 0.5f),
    Random.Range(-0.5f, 0.5f)
);
```

---

### Issue: Items Launch in Wrong Direction

**Possible Causes:**
1. Spawn point facing wrong way
2. Launch Direction configured incorrectly

**Solutions:**
- **Check spawn point rotation:** Blue arrow = forward direction
- **Test with simple direction:** Try (0, 0, 1) for straight forward
- **Enable debug logs** to see velocity in console

---

### Issue: All Items Launch Identically

**Cause:** Random Variation disabled or Max Random Angle = 0

**Solution:**
- Enable "Add Random Variation"
- Set Max Random Angle to 10-15

---

### Issue: Items Pass Through Objects

**Cause:** Colliders not set up properly

**Solutions:**
1. Ensure prefab has Collider component
2. Check "Is Trigger" is UNCHECKED
3. Ensure collision layer settings allow collision
4. Increase Continuous Dynamic on Rigidbody for fast-moving items

---

## Advanced Customization

### Curved Trajectories

Want a perfect arc? Use physics calculations:

```csharp
// In SpawnCraftedItem(), replace velocity calculation with:
float arcHeight = 2f;  // How high the arc goes
float distance = 3f;   // How far it travels
float gravity = Physics.gravity.y;

Vector3 displacement = targetPosition - spawnPosition;
Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * arcHeight);
Vector3 velocityXZ = new Vector3(displacement.x, 0, displacement.z) / 
    Mathf.Sqrt(-2 * arcHeight / gravity);

rb.velocity = velocityXZ + velocityY;
```

### Launch Sound Effect

Add audio feedback:

```csharp
// In CoffeeMaker, add field:
[SerializeField] private AudioClip launchSound;
[SerializeField] private AudioSource audioSource;

// In SpawnCraftedItem(), after instantiation:
if (audioSource != null && launchSound != null)
{
    audioSource.PlayOneShot(launchSound);
}
```

### Particle Effect on Launch

```csharp
// Add field:
[SerializeField] private ParticleSystem launchEffect;

// In SpawnCraftedItem(), at spawn point:
if (launchEffect != null)
{
    Instantiate(launchEffect, spawnPosition, spawnRotation);
}
```

### Target-Based Launch

Launch towards a specific target:

```csharp
// Add field:
[SerializeField] private Transform targetPosition;

// In SpawnCraftedItem():
if (targetPosition != null)
{
    finalLaunchDirection = (targetPosition.position - spawnPosition).normalized;
}
```

---

## Performance Notes

### Optimization Tips

1. **Use Object Pooling** for frequently spawned items
2. **Limit active physics objects** (destroy or disable old items)
3. **Adjust Fixed Timestep** if physics feels jittery
4. **Use Continuous Collision Detection** for fast-moving items

### Rigidbody Settings for Performance

```
Collision Detection: Continuous Dynamic (for fast items)
Interpolate: Interpolate (for smooth visuals)
Constraints: Freeze rotation if items shouldn't spin
```

---

## Debug Output

When "Show Debug Logs" is enabled, you'll see:

```
[CoffeeMaker] Launched Salted Coffee with velocity (0.5, 2.5, 5.0) (force: 5)
[CoffeeMaker] Spawned Salted Coffee at (1.0, 1.5, 0.5)
```

**No Rigidbody Warning:**
```
[CoffeeMaker] Spawned Simple Coffee has no Rigidbody - cannot apply launch velocity!
```

---

## Comparison

### Before (Static Spawn):
```
Item appears at spawn point
No movement
No physics interaction
Feels instant/teleported
```

### After (Dynamic Launch):
```
Item spawns with velocity ?
Flies through air ?
Affected by gravity ?
Natural tumbling motion ?
Slight variation each time ?
Can hit/push other objects ?
```

---

## Example Configurations

### Classic Vending Machine
```
Launch Direction: (0, 0.2, 1)
Launch Force: 4
Add Random Variation: ?
Max Random Angle: 12
```

### Coffee Shop Dispenser
```
Launch Direction: (0, -0.3, 0.8)
Launch Force: 3
Add Random Variation: ?
Max Random Angle: 8
```

### Energetic Maker
```
Launch Direction: (0, 1, 1)
Launch Force: 8
Add Random Variation: ?
Max Random Angle: 20
```

### Gentle Placement
```
Launch Direction: (0, 0.1, 0.5)
Launch Force: 2
Add Random Variation: ?
Max Random Angle: 0
```

---

## Summary

**New Features:**
- ? Physics-based item launching
- ? Configurable direction and force
- ? Random variation for natural movement
- ? Automatic spin/tumbling
- ? Spawn point rotation support
- ? Debug feedback

**Requirements:**
- ? Prefabs need Rigidbody
- ? Prefabs need Collider
- ? Setup spawn point transform

**Result:**
More dynamic, realistic, and satisfying coffee crafting experience!

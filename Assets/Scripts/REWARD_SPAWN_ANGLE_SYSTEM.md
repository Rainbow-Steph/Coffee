# Configurable Reward Spawn Angle System ?

## ? **Enhanced Reward Control!**

The DeliveryTarget now supports fully configurable reward spawn angles and directions!

---

## ?? **New Features:**

### **1. Configurable Launch Direction:**

```
Before: Always launches up + forward (fixed)
After:  Launch in ANY direction!
```

**New Inspector Fields:**
```
Reward Launch Direction: (0, 1, 0)  ? Base direction (default: up)
Reward Forward Influence: 0.3       ? How much forward component
```

---

### **2. Launch Direction Control:**

The system combines:
- **Base Direction** (configurable Vector3)
- **Forward Influence** (0-1 slider)
- **Random Variation** (optional angle spread)

**Formula:**
```csharp
launchDirection = (baseDirection + forward * influence).normalized
```

---

## ?? **Inspector Configuration:**

```
????????????????????????????????????????????????
? Spawn Settings                               ?
????????????????????????????????????????????????
? Reward Spawn Point: [Transform]             ?
? Reward Launch Direction: (0, 1, 0)          ? ? NEW! Base direction
? Reward Forward Influence: 0.3               ? ? NEW! Forward blend
? Reward Launch Force: 3                      ?
? Add Random Variation: ?                     ?
? Max Random Angle: 15                        ?
????????????????????????????????????????????????
```

---

## ?? **Direction Examples:**

### **Straight Up (No Forward):**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: 0
```
**Result:** Rewards launch straight up ?

---

### **Up + Forward (Default):**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: 0.3
```
**Result:** Rewards launch up and slightly forward ?

---

### **Straight Forward:**
```
Reward Launch Direction: (0, 0, 1)
Reward Forward Influence: 0
```
**Result:** Rewards launch horizontally forward ?

---

### **Up + Lots of Forward:**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: 0.8
```
**Result:** Rewards launch at steep forward angle ?

---

### **Backwards:**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: -0.5
```
**Result:** Rewards launch up and backwards ?

---

### **Sideways:**
```
Reward Launch Direction: (1, 0.5, 0)
Reward Forward Influence: 0
```
**Result:** Rewards launch to the side at upward angle ?

---

### **Custom Angle:**
```
Reward Launch Direction: (0.5, 1, 0.5)
Reward Forward Influence: 0.2
```
**Result:** Custom diagonal launch direction

---

## ?? **Common Presets:**

### **Preset 1: Fountain Effect (Straight Up)**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: 0
Reward Launch Force: 5
Max Random Angle: 20
```
**Effect:** Rewards shoot up and spread out like a fountain ?

---

### **Preset 2: Scatter Forward**
```
Reward Launch Direction: (0, 0.5, 1)
Reward Forward Influence: 0
Reward Launch Force: 4
Max Random Angle: 30
```
**Effect:** Rewards scatter forward at shallow angle ??

---

### **Preset 3: Arc Over Counter**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: 0.6
Reward Launch Force: 3
Max Random Angle: 10
```
**Effect:** Rewards arc over counter in controlled spread ??

---

### **Preset 4: Pop Straight Up**
```
Reward Launch Direction: (0, 1, 0)
Reward Forward Influence: 0.1
Reward Launch Force: 4
Max Random Angle: 5
```
**Effect:** Rewards pop straight up with minimal spread ??

---

### **Preset 5: Side Scatter**
```
Reward Launch Direction: (1, 0.7, 0)
Reward Forward Influence: 0
Reward Launch Force: 3
Max Random Angle: 25
```
**Effect:** Rewards scatter to the side ??

---

## ?? **How It Works:**

### **Direction Calculation:**

```csharp
// Step 1: Normalize base direction
Vector3 baseDirection = rewardLaunchDirection.normalized;

// Step 2: Add forward component based on influence
Vector3 forwardComponent = rewardSpawnPoint.forward * rewardForwardInfluence;

// Step 3: Combine and normalize
Vector3 launchDirection = (baseDirection + forwardComponent).normalized;

// Step 4: Apply random variation (optional)
if (addRandomVariation)
{
    float randomX = Random.Range(-maxRandomAngle, maxRandomAngle);
    float randomY = Random.Range(-maxRandomAngle, maxRandomAngle);
    Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);
    launchDirection = randomRotation * launchDirection;
}

// Step 5: Apply velocity
rb.velocity = launchDirection * rewardLaunchForce;
```

---

## ?? **Use Cases:**

### **Use Case 1: Coffee Shop Counter**
```
Setup:
?? Spawn Point: Behind counter
?? Launch Direction: (0, 1, 0)      ? Up
?? Forward Influence: 0.5            ? Forward over counter
?? Launch Force: 3
?? Random Angle: 15

Result: Rewards arc over counter to customer side
```

---

### **Use Case 2: Underground Hatch**
```
Setup:
?? Spawn Point: Below floor
?? Launch Direction: (0, 1, 0)      ? Straight up
?? Forward Influence: 0              ? No forward
?? Launch Force: 6
?? Random Angle: 20

Result: Rewards shoot up through hatch, spreading out
```

---

### **Use Case 3: Stage Performance**
```
Setup:
?? Spawn Point: Stage center
?? Launch Direction: (0, 0.8, 0.5)  ? Up and forward
?? Forward Influence: 0              ? Custom direction
?? Launch Force: 4
?? Random Angle: 25

Result: Dramatic scatter toward audience
```

---

### **Use Case 4: Precise Delivery**
```
Setup:
?? Spawn Point: Target location
?? Launch Direction: (0, 1, 0)      ? Straight up
?? Forward Influence: 0.1            ? Minimal forward
?? Launch Force: 2
?? Random Angle: 5

Result: Rewards land right near spawn point
```

---

## ?? **Forward Influence Explained:**

### **Influence: 0**
```
Direction = baseDirection only
Result: Pure base direction (no forward component)
```

### **Influence: 0.3 (Default)**
```
Direction = baseDirection + (forward * 0.3)
Result: Mostly base direction with slight forward tilt
```

### **Influence: 0.5**
```
Direction = baseDirection + (forward * 0.5)
Result: Equal blend of base and forward
```

### **Influence: 1.0**
```
Direction = baseDirection + (forward * 1.0)
Result: Strong forward component
```

### **Influence: -0.5**
```
Direction = baseDirection + (backward * 0.5)
Result: Backward tilt!
```

---

## ?? **Visual Examples:**

### **Forward Influence Comparison:**

```
Influence: 0.0
    ?
    |
    |
  Start

Influence: 0.3 (Default)
    ?
   /
  /
Start

Influence: 0.5
   ?
  /
 /
Start

Influence: 1.0
?
|
Start
```

---

## ?? **Random Variation:**

### **With Random Variation:**

```
Max Angle: 15°

Launch attempts:
?? Reward 1: 10° left, 5° up
?? Reward 2: 8° right, 12° down
?? Reward 3: 3° left, 7° up
?? Result: Natural spread!
```

### **Without Random Variation:**

```
Max Angle: 0°

Launch attempts:
?? Reward 1: Exact direction
?? Reward 2: Exact direction
?? Reward 3: Exact direction
?? Result: All same direction (boring)
```

---

## ?? **Pro Tips:**

### **Tip 1: Match Environment**
```
Counter delivery: Launch over counter (forward influence 0.5)
Open space: Launch straight up (forward influence 0)
Narrow space: Launch forward (direction forward, influence 0)
```

### **Tip 2: Control Spread**
```
Tight landing: Max angle 5-10°
Natural spread: Max angle 15-20°
Dramatic scatter: Max angle 25-45°
```

### **Tip 3: Adjust Force for Distance**
```
Close spawn: Force 2-3
Medium spawn: Force 3-4
Far spawn: Force 5-7
```

### **Tip 4: Combine Direction + Influence**
```
Instead of: Direction (0, 0, 1), Influence 0    ? Forward only
Try: Direction (0, 1, 0), Influence 0.8        ? Up + forward (more natural)
```

---

## ?? **Testing Guide:**

### **Test Each Setting:**

1. **Test Base Direction:**
   - Try (0, 1, 0) - up
   - Try (0, 0, 1) - forward
   - Try (1, 0, 0) - right
   - Try custom directions

2. **Test Forward Influence:**
   - Try 0 - no forward
   - Try 0.3 - default
   - Try 0.5 - balanced
   - Try 1.0 - strong forward

3. **Test Launch Force:**
   - Try 2 - gentle
   - Try 3 - default
   - Try 5 - strong
   - Try 7 - powerful

4. **Test Random Angle:**
   - Try 0 - no spread
   - Try 15 - default spread
   - Try 30 - wide spread
   - Try 45 - maximum spread

---

## ?? **Direction Library:**

### **Common Directions:**

| Name | Direction | Forward Influence | Effect |
|------|-----------|-------------------|--------|
| **Fountain** | `(0, 1, 0)` | 0 | Straight up |
| **Arc Forward** | `(0, 1, 0)` | 0.5 | Up and forward |
| **Scatter Forward** | `(0, 0.5, 1)` | 0 | Forward angled |
| **Side Spray** | `(1, 0.5, 0)` | 0 | Sideways up |
| **Pop Up** | `(0, 1, 0)` | 0.1 | Minimal forward |
| **Launch Across** | `(0, 0.3, 1)` | 0 | Low forward arc |
| **Diagonal** | `(0.5, 0.7, 0.5)` | 0 | Custom angle |

---

## ?? **Animation Sequence:**

```
Complete Delivery Flow:

1. Coffee released
   ?
2. Target animates away
   ?
3. Wait (rewardSpawnDelay)
   ?
4. Calculate launch direction:
   - Base: rewardLaunchDirection
   - Add: forward * rewardForwardInfluence
   - Randomize: ±maxRandomAngle
   ?
5. Spawn rewards with calculated direction
   ?
6. Apply launch force
   ?
7. Rewards fly through air! ??
```

---

## ?? **Summary:**

### **New Configuration:**

```
? Reward Launch Direction (Vector3)
   - Any 3D direction
   - Automatically normalized

? Reward Forward Influence (0-1)
   - Blends forward component
   - Can be negative for backward

? Existing Settings Still Work:
   - Launch Force
   - Random Variation
   - Max Random Angle
```

### **Benefits:**

```
? Full control over reward direction
? Match any environment layout
? Create unique visual effects
? Balance between control and randomness
? Easy to tweak and experiment
```

---

## ?? **Files Modified:**

1. ? `DeliveryTarget.cs`
   - Added `rewardLaunchDirection` (Vector3)
   - Added `rewardForwardInfluence` (float, 0-1)
   - Updated `SpawnRewardItem()` calculation
   - Normalized direction handling
   - Forward influence blending

---

**Reward spawn angles now fully configurable with base direction and forward influence control!** ?????

Create fountains, arcs, scatters, or precise spawns with complete directional control!

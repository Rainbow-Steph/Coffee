# Coffee Delivery System - Complete Implementation ?

## Overview
A comprehensive delivery system for delivering coffee to targets and receiving rewards based on satisfaction ratings.

---

## System Components

### 1. **DeliverableManager.cs** - ScriptableObject Configuration
Central configuration for delivery targets, satisfaction levels, and rewards

### 2. **DeliveryTarget.cs** - Target Component
Attach to objects that can receive deliveries

### 3. **DeliverAction.cs** - Delivery Handler (Optional)
Alternative delivery trigger system

---

## How It Works

### Delivery Flow:

```
Player picks up coffee (ItemType.Delivery)
  ?
Brings coffee to delivery target
  ?
Places coffee in "Delivery Collider" zone
  ?
System checks satisfaction level:
  ?? High Satisfaction ? Spawn high rewards
  ?? Medium Satisfaction ? Spawn medium rewards
  ?? Low Satisfaction ? Spawn low rewards
  ?? No Match ? Delivery fails
  ?
Coffee destroyed, rewards spawned
```

---

## Setup Instructions

### Step 1: Create DeliverableManager ScriptableObject

```
1. Right-click in Project window
2. Create ? Coffee ? Deliverable Manager
3. Name it "DeliverableManager"
4. Save in Resources folder (optional, for auto-finding)
```

---

### Step 2: Configure Delivery Targets

In the DeliverableManager Inspector:

#### **Delivery Targets List:**

**Black Target:**
```
Target Name: "Black Target"
Target Color: Black

High Satisfaction Items:
?? Element 0: Espresso Prefab
?? Element 1: Dark Roast Prefab
?? Element 2: Black Coffee Prefab

Medium Satisfaction Items:
?? Element 0: Regular Coffee Prefab
?? Element 1: Americano Prefab

Low Satisfaction Items:
?? Element 0: Instant Coffee Prefab
?? Element 1: Cold Coffee Prefab
```

**Red Target:**
```
Target Name: "Red Target"
Target Color: Red

High Satisfaction Items:
?? Element 0: Strawberry Latte Prefab
?? Element 1: Red Velvet Coffee Prefab
?? Element 2: Cherry Mocha Prefab

Medium Satisfaction Items:
?? Element 0: Caramel Latte Prefab
?? Element 1: Vanilla Latte Prefab

Low Satisfaction Items:
?? Element 0: Plain Latte Prefab
?? Element 1: Simple Coffee Prefab
```

**Blue Target:**
```
Target Name: "Blue Target"
Target Color: Blue

High Satisfaction Items:
?? Element 0: Blueberry Coffee Prefab
?? Element 1: Iced Blue Latte Prefab
?? Element 2: Mint Mocha Prefab

Medium Satisfaction Items:
?? Element 0: Iced Coffee Prefab
?? Element 1: Cold Brew Prefab

Low Satisfaction Items:
?? Element 0: Water Prefab
?? Element 1: Plain Iced Prefab
```

---

### Step 3: Configure Rewards

#### **High Satisfaction Rewards:**
```
Reward Prefabs:
?? Rock Prefab: [Drag Rock prefab]
?? Coin Prefab: [Drag Coin prefab]
?? Bill Prefab: [Drag Bill prefab]

Reward Ranges:
?? Rocks Range:
?  ?? Min: 2
?  ?? Max: 5
?? Coins Range:
?  ?? Min: 3
?  ?? Max: 7
?? Bills Range:
   ?? Min: 1
   ?? Max: 3
```

#### **Medium Satisfaction Rewards:**
```
Reward Ranges:
?? Rocks Range:
?  ?? Min: 1
?  ?? Max: 3
?? Coins Range:
?  ?? Min: 2
?  ?? Max: 5
?? Bills Range:
   ?? Min: 0
   ?? Max: 1
```

#### **Low Satisfaction Rewards:**
```
Reward Ranges:
?? Rocks Range:
?  ?? Min: 0
?  ?? Max: 2
?? Coins Range:
?  ?? Min: 1
?  ?? Max: 3
?? Bills Range:
   ?? Min: 0
   ?? Max: 0
```

---

### Step 4: Create Delivery Target GameObjects

For each target (Black, Red, Blue):

```
1. Create GameObject in scene
2. Name it: "Black Target" / "Red Target" / "Blue Target"
3. Add DeliveryTarget component
4. Create child GameObject:
   ?? Name: "Delivery Collider"
   ?? Add Collider component (Box or Sphere)
   ?? Set as Trigger: ?
5. Configure DeliveryTarget component:
   ?? Target Name: Match name in DeliverableManager
   ?? Deliverable Manager: [Drag asset]
   ?? Delivery Collider: [Drag child object]
```

---

### Step 5: Mark Coffee as Delivery Items

On your coffee prefabs:

```
ClickableObject Component:
?? Can Pick Up: ?
?? Item Type: Delivery ? IMPORTANT!
?? Other settings as needed
```

---

## Features

### 1. **Satisfaction-Based Rewards** ??

```csharp
High Satisfaction:
?? Best rewards
?? More items
?? Higher value items

Medium Satisfaction:
?? Moderate rewards
?? Average quantity
?? Mixed value

Low Satisfaction:
?? Minimal rewards
?? Few items
?? Lower value
```

---

### 2. **Flexible Target Configuration** ??

```
Each target can accept different items:
?? Black Target ? Dark coffee drinks
?? Red Target ? Flavored/sweet drinks
?? Blue Target ? Cold/iced drinks

Add/remove items dynamically:
?? Just modify the lists in DeliverableManager
```

---

### 3. **Automatic Delivery Detection** ??

```
System automatically detects:
?? When item enters delivery zone
?? If item is being held
?? If item type is "Delivery"
?? Which target it's delivered to
```

---

### 4. **Visual & Audio Feedback** ?

```
On successful delivery:
?? Success particles play
?? Success sound plays
?? Rewards spawn
?? Coffee destroyed

On failed delivery:
?? Failure particles play
?? Failure sound plays
?? Coffee remains
?? No rewards
```

---

### 5. **Dynamic Reward Spawning** ??

```
Rewards spawn with:
?? Random amounts (within range)
?? Launch physics
?? Random directions
?? Natural movement
```

---

## Usage Examples

### Example 1: Basic Delivery

```
1. Player crafts Espresso
2. Espresso has ItemType.Delivery
3. Player picks up Espresso
4. Player brings to Black Target
5. Player drops Espresso in "Delivery Collider"
6. System checks: Espresso in Black Target's High Satisfaction list
7. High satisfaction rewards spawn!
8. Espresso destroyed
```

---

### Example 2: Wrong Target

```
1. Player has Blueberry Coffee
2. Player brings to Black Target
3. Player drops in "Delivery Collider"
4. System checks: Blueberry Coffee NOT in Black Target's lists
5. Delivery fails
6. Failure particles/sound play
7. Coffee remains (player can try different target)
```

---

### Example 3: Low Satisfaction

```
1. Player has Instant Coffee
2. Player brings to Black Target
3. Player drops in "Delivery Collider"
4. System checks: Instant Coffee in Low Satisfaction list
5. Low satisfaction rewards spawn
6. Player gets: 1 rock, 2 coins, 0 bills
7. Instant Coffee destroyed
```

---

## Advanced Configuration

### Adding New Targets:

```csharp
In DeliverableManager:
1. Expand "Delivery Targets" list
2. Increase Size by 1
3. Configure new target:
   ?? Target Name: "Green Target"
   ?? Target Color: Green
   ?? High Satisfaction Items: [Add prefabs]
   ?? Medium Satisfaction Items: [Add prefabs]
   ?? Low Satisfaction Items: [Add prefabs]
```

---

### Modifying Rewards:

```csharp
// Increase high satisfaction rewards
highSatisfactionRewards.billsRange.max = 5; // More bills!

// Reduce low satisfaction rewards
lowSatisfactionRewards.rocksRange.max = 0; // No rocks!
```

---

### Custom Reward Spawn Logic:

```csharp
In DeliveryTarget.SpawnRewardItem():
// Modify launch direction
Vector3 launchDirection = Vector3.up + Vector3.forward * 0.5f;

// Modify launch force
rb.velocity = launchDirection.normalized * 5f;
```

---

## DeliverAction Component (Optional)

If you want a button-triggered delivery instead of automatic:

### Setup:
```
1. Create GameObject (e.g., "Delivery Button")
2. Add DeliverAction component
3. Configure:
   ?? Deliverable Manager: [Drag asset]
   ?? Auto Find Nearest Target: ?
   ?? Max Search Distance: 10
4. Add UI Button
5. On Click Event ? DeliverAction.AttemptDelivery()
```

### Usage:
```
Player holds coffee ? Clicks button ? Nearest target receives delivery
```

---

## Debug Features

### Console Logging:

With `showDebugInfo = true`:

```
[DeliveryTarget] Delivered Espresso to Black Target. Satisfaction: High
[DeliveryTarget] ? Successful delivery! Satisfaction: High
[DeliveryTarget] Spawned rewards: 3 rocks, 5 coins, 2 bills
```

### Scene Gizmos:

```
DeliveryTarget:
?? Green box = Delivery collider zone
?? Yellow sphere = Reward spawn point

DeliverAction:
?? Yellow sphere = Search radius
?? Green line = Connection to target
```

---

## Satisfaction Level Determination

### How It Works:

```csharp
1. Player delivers coffee prefab
2. System gets prefab name
3. System checks target's lists:
   ?? In High Satisfaction list? ? High
   ?? In Medium Satisfaction list? ? Medium
   ?? In Low Satisfaction list? ? Low
   ?? Not in any list? ? None (Failed)
4. Spawn rewards based on level
```

### Matching Logic:

```csharp
// Compares prefab names (ignoring "(Clone)")
if (deliveredPrefab.name == listItem.name)
{
    return SatisfactionLevel.High; // Match found!
}
```

---

## Reward Calculation

### Random Amount Generation:

```csharp
RewardRange: Min = 2, Max = 5

Possible outcomes:
?? 2 items (20% chance)
?? 3 items (20% chance)
?? 4 items (20% chance)
?? 5 items (20% chance)
?? Random between 2-5
```

### Total Rewards Example:

```
High Satisfaction:
?? Rocks: Random(2, 5) = 4 rocks
?? Coins: Random(3, 7) = 6 coins
?? Bills: Random(1, 3) = 2 bills
Total: 12 reward items spawned!
```

---

## Troubleshooting

### Issue 1: Delivery Not Detecting

**Check:**
- [ ] Coffee has ItemType.Delivery
- [ ] Delivery Collider is set as Trigger
- [ ] Delivery Collider is child of target
- [ ] DeliveryTarget component attached

**Fix:**
Ensure all components properly configured

---

### Issue 2: Wrong Satisfaction Level

**Check:**
- [ ] Prefab names match exactly
- [ ] Prefab in correct list
- [ ] DeliverableManager assigned

**Fix:**
Verify prefab names and lists

---

### Issue 3: No Rewards Spawning

**Check:**
- [ ] Reward prefabs assigned
- [ ] Reward ranges > 0
- [ ] Reward prefabs have Rigidbody

**Fix:**
Assign prefabs and set ranges

---

### Issue 4: Delivery Fails Every Time

**Check:**
- [ ] Target Name matches DeliverableManager
- [ ] Prefab in one of the satisfaction lists
- [ ] DeliverableManager assigned to target

**Fix:**
Match names and add prefab to list

---

## API Reference

### DeliverableManager:

```csharp
// Find target by name
DeliveryTarget FindTargetByName(string targetName)

// Get rewards for satisfaction level
SatisfactionRewards GetRewards(SatisfactionLevel level)

// Validate configuration
bool ValidateConfiguration()
```

### DeliveryTarget:

```csharp
// Get target name
string GetTargetName()

// Check if item can be accepted
bool CanAcceptItem(GameObject item)
```

### DeliverAction:

```csharp
// Attempt delivery
void AttemptDelivery()

// Check if delivery possible
bool CanDeliver()

// Get nearest target
DeliveryTarget GetNearestTarget()
```

---

## Example Scenario

### Full Delivery Flow:

```
1. Player Crafts Coffee:
   ?? CoffeeMaker spawns Espresso
   ?? Espresso ItemType = Delivery

2. Player Picks Up:
   ?? ClickableObject.OnClick()
   ?? Player holds Espresso

3. Player Approaches Black Target:
   ?? Sees delivery zone (green gizmo)

4. Player Drops Coffee:
   ?? Coffee enters "Delivery Collider"
   ?? DeliveryTarget.OnTriggerEnter()

5. System Checks:
   ?? Is ItemType.Delivery? ?
   ?? Is being held? ?
   ?? In Black Target's lists? ? (High Satisfaction)

6. System Responds:
   ?? Spawns 4 rocks
   ?? Spawns 6 coins
   ?? Spawns 2 bills
   ?? Plays success particles
   ?? Plays success sound
   ?? Destroys Espresso

7. Player Collects Rewards:
   ?? Picks up spawned items
   ?? Inventory updated
```

---

## Summary

### ? System Features:

1. **DeliverableManager** ??
   - Centralized configuration
   - Satisfaction-based rewards
   - Flexible target setup

2. **DeliveryTarget** ??
   - Automatic delivery detection
   - Satisfaction level checking
   - Reward spawning

3. **DeliverAction** ?? (Optional)
   - Button-triggered delivery
   - Nearest target finding
   - Manual delivery control

---

### ?? Files Created:

1. ? `DeliverableManager.cs` - ScriptableObject config
2. ? `DeliveryTarget.cs` - Target component
3. ? `DeliverAction.cs` - Action handler

---

### ?? Setup Steps:

1. Create DeliverableManager asset
2. Configure targets and rewards
3. Create delivery target GameObjects
4. Add "Delivery Collider" children
5. Mark coffee prefabs as ItemType.Delivery
6. Test deliveries!

---

**Complete coffee delivery system ready to use!** ????

Players can now deliver coffee to different targets and receive satisfaction-based rewards!

# Supply Spawner System

## Overview
A comprehensive supply spawning system for managing resource dispensers (Water, Coffee, Extras) in your coffee game. The system uses ScriptableObjects for configuration and provides click-to-purchase spawning with animations, sound, and visual feedback.

---

## System Components

### 1. **SupplyConfig.cs** - ScriptableObject Configuration
Central configuration for all supply spawners

### 2. **Supply.cs** - Spawner Component
Attach to spawner GameObjects to handle spawning

### 3. **ResourceType Enum**
- Water
- Coffee
- Extras

---

## How It Works

### Click-to-Spawn Flow:

```
Player clicks spawner
  ?
Check money >= cost?
  ?? NO ? Play failed animation + dialogue
  ?? YES ? Deduct money
   ? Play spawn animation
         ? Spawn random amount of items
         ? Launch items from spawn point
```

---

## Setup Instructions

### Step 1: Create SupplyConfig ScriptableObject

```
1. Right-click in Project window
2. Create ? Coffee ? Supply Configuration
3. Name it "SupplyConfig"
4. Configure settings for each resource type
```

---

### Step 2: Configure Resource Types

#### **Water Configuration:**
```
Resource Type Name: "Water"
Debug Color: Blue
Cost Per Spawn: 10 (adjust as needed)
Min Spawn Count: 1
Max Spawn Count: 3
Spawn Delay: 0.1 seconds
Launch Force: 5
Launch Direction: (0, 0.5, 1) forward + up
Add Random Variation: ?
Max Random Angle: 15 degrees

Available Items:
?? Item 0:
?  ?? Prefab: [Your Water Prefab]
?  ?? Item Name: "Water"
?  ?? Is Available: ?
?  ?? Spawn Weight: 50
?? Item 1:
?  ?? Prefab: [Your Milk Prefab]
?  ?? Item Name: "Milk"
?  ?? Is Available: ?
?  ?? Spawn Weight: 30
?? Item 2:
   ?? Prefab: [Your Juice Prefab]
   ?? Item Name: "Juice"
   ?? Is Available: ? (initially locked)
   ?? Spawn Weight: 20
```

#### **Coffee Configuration:**
```
Resource Type Name: "Coffee"
Debug Color: Brown
Cost Per Spawn: 15
Min Spawn Count: 1
Max Spawn Count: 2
Spawn Delay: 0.15 seconds
Launch Force: 4
Launch Direction: (0, 0.5, 1)
Add Random Variation: ?
Max Random Angle: 10 degrees

Available Items:
?? Espresso Capsule (Available: ?, Weight: 50)
?? Decaf Capsule (Available: ?, Weight: 40)
?? Dark Roast (Available: ?, Weight: 30)
```

#### **Extras Configuration:**
```
Resource Type Name: "Extras"
Debug Color: Yellow
Cost Per Spawn: 5
Min Spawn Count: 2
Max Spawn Count: 4
Spawn Delay: 0.08 seconds
Launch Force: 6
Launch Direction: (0, 0.5, 1)
Add Random Variation: ?
Max Random Angle: 20 degrees

Available Items:
?? Sugar (Available: ?, Weight: 60)
?? Cream (Available: ?, Weight: 50)
?? Syrup (Available: ?, Weight: 40)
```

---

### Step 3: Create Spawner GameObjects

For each resource type (Water, Coffee, Extras):

```
1. Create GameObject in scene
2. Name it: "WaterSpawner" / "CoffeeSpawner" / "ExtrasSpawner"
3. Add Collider (for clicking)
4. Add ClickableObject component
   ?? Can Pick Up: ? OFF
   ?? Item Type: Prop
5. Add Supply component
```

---

### Step 4: Configure Supply Component

**On WaterSpawner:**
```
Supply Component:
?? Resource Type: Water
?? Supply Config: [Drag SupplyConfig asset]
?? Inventory Manager: [Drag InventoryManager]
?? System Messages: [Drag SystemMessages]
?? Dialogue Manager: [Drag DialogueManager from scene]
?? Spawn Point: [Create child "SpawnPoint" transform]
?? Animator: [Optional - your animator]
?? Spawn Animation Trigger: "Spawn"
?? Failed Animation Trigger: "Failed"
?? Audio Source: [Optional]
?? Spawn Particles: [Optional]
?? Failed Particles: [Optional]
?? Not Enough Money Dialogue: [Drag DialogueNodeSO]
?? Show Debug Info: ? (for testing)
```

**Repeat for CoffeeSpawner and ExtrasSpawner** with appropriate ResourceType

---

### Step 5: Create Spawn Point Transform

```
For each spawner:
1. Create empty child GameObject
2. Name it "SpawnPoint"
3. Position it where items should spawn
4. Rotate to set launch direction
5. Assign to Supply component's Spawn Point field
```

---

## Features

### 1. **Money System Integration**
```csharp
// Check money before spawning
if (inventoryManager.Money >= cost)
{
    inventoryManager.Money -= cost;
  SpawnItems();
}
else
{
    ShowNotEnoughMoneyDialogue();
}
```

---

### 2. **Weighted Random Spawning**
```csharp
// Items with higher spawn weight appear more often
Sugar: Weight 60 ? 60% chance
Cream: Weight 50 ? 50% chance
Syrup: Weight 40 ? 40% chance (if available)
```

---

### 3. **Dynamic Item Availability**
```csharp
// Enable/disable items via code or events
supplyConfig.waterConfig.SetItemAvailability("Juice", true);
supply.SetItemAvailability("DarkRoast", true);
```

**Use Cases:**
- Unlock items based on player progress
- Enable seasonal items
- Disable out-of-stock items
- Progression system

---

### 4. **Animation Support**
```csharp
Animator triggers:
- "Spawn" ? Successful spawn animation
- "Failed" ? Not enough money animation

Example Animator Setup:
?? Idle
?? Spawn (transition from Any State)
?  ?? Returns to Idle after animation
?? Failed (transition from Any State)
   ?? Returns to Idle after animation
```

---

### 5. **Audio Feedback**
```csharp
SupplyConfig:
?? Spawn Success Sound: "Cha-ching!"
?? Failed Sound: "Buzz/Error"

Automatically plays via Supply component's AudioSource
```

---

### 6. **Visual Feedback**
```csharp
Particle Systems:
?? Spawn Particles: Confetti/sparkles on success
?? Failed Particles: Error effect on failure
```

---

### 7. **Launch Physics**
```csharp
Spawned items:
?? Launched with configurable force
?? Random direction variation
?? Random angular velocity
?? Natural-feeling trajectory
```

---

## Spawn Weight System

### How It Works:
```
Items in list:
?? Water:  Weight 50
?? Milk:   Weight 30
?? Juice:  Weight 20 (if available)

Total weight: 100

Random selection:
0-49:  Spawn Water  (50% chance)
50-79: Spawn Milk   (30% chance)
80-99: Spawn Juice  (20% chance)
```

### Adjusting Probabilities:
```
Higher weight = More likely to spawn
Lower weight = Less likely to spawn

Example for rare items:
?? Common:  Weight 70 ? 70% chance
?? Uncommon: Weight 20 ? 20% chance
?? Rare:    Weight 10 ? 10% chance
```

---

## Item Availability System

### Toggle via Code:
```csharp
// Get reference to supply spawner
Supply waterSpawner = FindObjectOfType<Supply>();

// Enable a specific item
waterSpawner.SetItemAvailability("Juice", true);

// Disable a specific item
waterSpawner.SetItemAvailability("Water", false);
```

### Toggle via Events:
```csharp
// In Unity Event (button click, trigger, etc.):
Supply.SetItemAvailability(string itemName, bool available)

// Or via SupplyConfig directly:
supplyConfig.waterConfig.SetItemAvailability("Juice", true);
```

### Use Cases:
```
1. Progression System:
   ?? Unlock "Dark Roast" after completing tutorial

2. Shop System:
   ?? Purchase new items to add to spawn pool

3. Time-Based:
   ?? Enable "IcedCoffee" during summer season

4. Resource Management:
   ?? Disable items when out of stock

5. Achievement Rewards:
   ?? Unlock "Premium Blend" after achievement
```

---

## Debug Features

### Console Logging (showDebugInfo = true):
```
[Supply] WaterSpawner initialized as Water spawner. Cost: $10
[Supply] Water spawner clicked!
[Supply] Cost: $10, Current money: $50
[Supply] Deducted $10. Remaining: $40
[Supply] Spawning 3 Water items
[Supply] Spawned Water at (1.0, 2.0, 3.0)
[Supply] Launched Water with velocity (2.1, 3.5, 4.2)
[Supply] Finished spawning 3 items
```

### Scene Gizmos:
```
Normal View:
?? Colored sphere at spawn point
?? Launch direction arrow
?? Color-coded by resource type:
   ?? Blue = Water
   ?? Brown = Coffee
   ?? Yellow = Extras

Selected View:
?? Launch cone visualization
?? Shows random variation range
?? Preview of spawn area
```

---

## Example Scenarios

### Scenario 1: Basic Water Spawner
```
Player clicks water dispenser
?? Has $50, cost is $10
?? Money: $50 ? $40
?? Spawn animation plays
?? Spawns 2 water bottles
?? Both launch upward and forward
?? Player can pick them up
```

---

### Scenario 2: Not Enough Money
```
Player clicks coffee dispenser
?? Has $5, cost is $15
?? Failed animation plays
?? Error sound plays
?? Dialogue: "Not enough money!"
?? No spawn occurs
```

---

### Scenario 3: Weighted Random
```
Player buys extras (4 items):
?? Item 1: Sugar (60% weight) ? Spawns Sugar
?? Item 2: Sugar (60% weight) ? Spawns Sugar
?? Item 3: Cream (40% weight) ? Spawns Cream
?? Item 4: Sugar (60% weight) ? Spawns Sugar

Result: 3 Sugar, 1 Cream (based on probability)
```

---

### Scenario 4: Unlocking Items
```csharp
// Player completes quest
void OnQuestComplete()
{
    Supply coffeeSpawner = GetCoffeeSpawner();
    coffeeSpawner.SetItemAvailability("DarkRoast", true);
    
    ShowMessage("Dark Roast coffee unlocked!");
}
```

---

## API Reference

### SupplyConfig Methods:

```csharp
// Get configuration for resource type
ResourceTypeConfig config = supplyConfig.GetConfig(ResourceType.Water);

// Get random available item
GameObject prefab = config.GetRandomAvailableItem();

// Get count of available items
int count = config.GetAvailableItemCount();

// Set item availability
bool success = config.SetItemAvailability("Juice", true);

// Validate configuration
bool valid = supplyConfig.Validate();
```

### Supply Methods:

```csharp
// Manual spawn trigger (called by ClickableObject)
supply.OnSpawnerClicked();

// Set item availability
supply.SetItemAvailability("DarkRoast", true);

// Get spawn cost
int cost = supply.GetSpawnCost();

// Check if player can afford
bool canAfford = supply.CanAffordSpawn();
```

---

## Integration Examples

### With Quest System:
```csharp
public class QuestReward : MonoBehaviour
{
    public Supply targetSpawner;
    public string itemToUnlock;
    
    public void OnQuestComplete()
    {
        targetSpawner.SetItemAvailability(itemToUnlock, true);
    }
}
```

### With Shop System:
```csharp
public class ShopItem : MonoBehaviour
{
    public Supply targetSpawner;
    public string itemName;
    public int purchaseCost;
    
    public void Purchase()
 {
        if (inventoryManager.Money >= purchaseCost)
    {
        inventoryManager.Money -= purchaseCost;
  targetSpawner.SetItemAvailability(itemName, true);
    }
    }
}
```

### With Time System:
```csharp
public class SeasonalItems : MonoBehaviour
{
    public Supply spawner;
    
    public void OnSeasonChange(Season newSeason)
    {
        switch (newSeason)
        {
case Season.Summer:
          spawner.SetItemAvailability("IcedCoffee", true);
            break;
         case Season.Winter:
       spawner.SetItemAvailability("HotChocolate", true);
         break;
        }
    }
}
```

---

## Troubleshooting

### Issue 1: Items Not Spawning
```
Check:
? SupplyConfig assigned?
? Resource type set correctly?
? At least one item available?
? Items have prefabs assigned?
? Player has enough money?
? showDebugInfo enabled for logs?
```

### Issue 2: Items Spawning in Wrong Place
```
Check:
? Spawn Point assigned?
? Spawn Point positioned correctly?
? Launch direction configured?
? Check Scene view gizmos for visualization
```

### Issue 3: No Animation Playing
```
Check:
? Animator component assigned?
? Animation triggers match?
? Animator has "Spawn" and "Failed" triggers?
? Animator controller configured?
```

### Issue 4: Wrong Cost Deducted
```
Check:
? SupplyConfig ? Resource Config ? Cost Per Spawn
? Correct resource type selected?
? InventoryManager reference valid?
```

---

## Performance

### Spawn Count Recommendations:
```
Low-end devices:
?? Min: 1
?? Max: 2
?? Delay: 0.15s

Mid-range devices:
?? Min: 1
?? Max: 3
?? Delay: 0.1s

High-end devices:
?? Min: 2
?? Max: 5
?? Delay: 0.05s
```

### Object Pooling (Future Enhancement):
```
For frequent spawning, consider:
?? Object pooling system
?? Reuse spawned objects
?? Reduce instantiation overhead
```

---

## Summary

### What Was Created:
1. ? **SupplyConfig.cs** - ScriptableObject configuration
2. ? **Supply.cs** - Spawner component
3. ? **ResourceType enum** - Water, Coffee, Extras

### Features:
- ? Money-based purchasing system
- ? Random amount spawning (min/max)
- ? Weighted random item selection
- ? Dynamic item availability toggling
- ? Animation support (spawn/failed)
- ? Audio feedback
- ? Particle effects
- ? Launch physics with variation
- ? Debug visualization
- ? Configurable per resource type

### Setup Steps:
1. Create SupplyConfig asset
2. Configure each resource type
3. Create spawner GameObjects
4. Add Supply components
5. Assign references
6. Configure spawn points
7. Test!

**Complete, flexible supply spawning system ready to use!** ????

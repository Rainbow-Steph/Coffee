# Supply Spawner - Quick Setup Guide

## ? **System Created Successfully!**

Two new scripts ready to use:
- `SupplyConfig.cs` - ScriptableObject for configuration
- `Supply.cs` - Component for spawner GameObjects

---

## 5-Minute Setup

### Step 1: Create SupplyConfig Asset (1 min)

```
1. Right-click in Project
2. Create ? Coffee ? Supply Configuration
3. Name: "SupplyConfig"
```

---

### Step 2: Configure Water Spawner (1 min)

```
Select SupplyConfig asset in Inspector:

Water Config:
?? Cost Per Spawn: 10
?? Min Spawn Count: 1
?? Max Spawn Count: 3
?? Launch Force: 5
?? Available Items:
   ?? Add 3 items
   ?? Assign prefabs (Water, Milk, etc.)
   ?? Set spawn weights (50, 30, 20)
```

---

### Step 3: Configure Coffee Spawner (1 min)

```
Coffee Config:
?? Cost Per Spawn: 15
?? Min Spawn Count: 1
?? Max Spawn Count: 2
?? Launch Force: 4
?? Available Items:
   ?? Espresso Capsule (Weight: 50)
   ?? Decaf Capsule (Weight: 40)
```

---

### Step 4: Configure Extras Spawner (1 min)

```
Extras Config:
?? Cost Per Spawn: 5
?? Min Spawn Count: 2
?? Max Spawn Count: 4
?? Launch Force: 6
?? Available Items:
   ?? Sugar (Weight: 60)
   ?? Cream (Weight: 50)
```

---

### Step 5: Create Spawner GameObjects (1 min)

For each spawner (Water, Coffee, Extras):

```
1. Create GameObject in scene
2. Name: "WaterSpawner" (or CoffeeSpawner, ExtrasSpawner)
3. Add Components:
 ?? Collider (for clicking)
   ?? ClickableObject
   ?? Supply
4. Create child "SpawnPoint" transform
5. Position spawn point where items should appear
```

---

### Step 6: Configure Supply Component

**On each spawner GameObject:**

```
Supply Component:
?? Resource Type: Water / Coffee / Extras
?? Supply Config: [Drag SupplyConfig asset]
?? Inventory Manager: [Auto-found or drag]
?? System Messages: [Auto-found or drag]
?? Dialogue Manager: [Auto-found or drag]
?? Spawn Point: [Drag "SpawnPoint" child]
?? Not Enough Money Dialogue: [Drag DialogueNodeSO]
```

---

## Testing

### Test 1: Basic Spawn
```
1. Play the game
2. Give yourself money (test): inventoryManager.Money = 100
3. Click spawner
4. Items should spawn and launch!
```

### Test 2: Not Enough Money
```
1. Set money to $0
2. Click spawner
3. Should show "not enough money" dialogue
4. No items spawn
```

---

## Visual Aids

### Scene View Gizmos:
```
When spawner selected:
?? Colored sphere = Spawn point
?? Arrow = Launch direction
?? Cone = Random variation range
?? Color:
   ?? Blue = Water
   ?? Brown = Coffee
   ?? Yellow = Extras
```

---

## Quick Configuration

### Typical Values:

**Water (Cheap, Multiple Items):**
```
Cost: $10
Spawn: 1-3 items
Force: 5
Delay: 0.1s
```

**Coffee (Medium Cost, Fewer Items):**
```
Cost: $15
Spawn: 1-2 items
Force: 4
Delay: 0.15s
```

**Extras (Cheap, Many Items):**
```
Cost: $5
Spawn: 2-4 items
Force: 6
Delay: 0.08s
```

---

## Common Spawn Weights

```
Very Common:  70-100
Common:       50-70
Uncommon:     30-50
Rare:      10-30
Very Rare:    1-10
```

---

## Enable/Disable Items

### Via Inspector:
```
SupplyConfig ? Water Config ? Available Items
?? Toggle "Is Available" checkbox
```

### Via Code:
```csharp
Supply spawner = GetComponent<Supply>();
spawner.SetItemAvailability("Juice", true);  // Enable
spawner.SetItemAvailability("Water", false); // Disable
```

---

## Troubleshooting

**Nothing happens when clicking?**
- Check ClickableObject component present
- Check Collider attached
- Enable showDebugInfo for logs

**Wrong resource type spawning?**
- Check Resource Type setting matches spawner

**Items spawn but don't move?**
- Check prefabs have Rigidbody
- Check Launch Force > 0

---

## That's It!

The supply spawner system is now ready to use. Click spawners to purchase and spawn random items! ???

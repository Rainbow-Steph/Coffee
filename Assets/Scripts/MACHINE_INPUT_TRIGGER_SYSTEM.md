# Machine Input Trigger System

## Overview
The Machine Input Trigger System replaces click-based input with a more intuitive trigger-based approach. When held items enter designated trigger zones, they are automatically added to the coffee machine.

## How It Works

### Old System (Click-Based):
```
1. Hold item
2. Click on input object (MachineWaterInput, etc.)
3. Item interaction handler processes click
4. Item added to machine
```

### New System (Trigger-Based):
```
1. Hold item
2. Move item into "Input Trigger" zone
3. Trigger automatically detects and processes item
4. Item added to machine
```

---

## Setup Instructions

### Step 1: Create Trigger Objects

For each machine input object, create a child object named "Input Trigger":

```
Machine
?? MachineWaterInput
?  ?? Input Trigger (with MachineInputTrigger component)
?? MachineCoffeeInput
?  ?? Input Trigger (with MachineInputTrigger component)
?? MachineExtraInput
   ?? Input Trigger (with MachineInputTrigger component)
```

---

### Step 2: Add Colliders

Each "Input Trigger" object needs a collider:

**Box Collider (Recommended):**
```
Component: Box Collider
Is Trigger: ? (automatically set by script)
Size: Adjust to desired input zone
Center: Position relative to parent
```

**Or Sphere Collider:**
```
Component: Sphere Collider
Is Trigger: ? (automatically set by script)
Radius: Adjust to desired input zone
```

---

### Step 3: Add MachineInputTrigger Script

Add the `MachineInputTrigger` component to each "Input Trigger" object:

**For Water Input:**
```
MachineInputTrigger Component:
?? Input Type: Water
?? Action Tracker: PlayerActionTracker
?? System Messages: SystemMessages
?? Dialogue Manager: DialogueManager
?? Display Handler: (Optional) Your display handler
?? Show Debug Info: ? (for testing)
```

**For Coffee Input:**
```
MachineInputTrigger Component:
?? Input Type: Coffee
?? Action Tracker: PlayerActionTracker
?? System Messages: SystemMessages
?? Dialogue Manager: DialogueManager
?? Display Handler: (Optional) Your display handler
?? Show Debug Info: ? (for testing)
```

**For Extra Input:**
```
MachineInputTrigger Component:
?? Input Type: Extra
?? Action Tracker: PlayerActionTracker
?? System Messages: SystemMessages
?? Dialogue Manager: DialogueManager
?? Display Handler: (Optional) Your display handler
?? Show Debug Info: ? (for testing)
```

---

## Component Settings

### Input Type

Determines what items the trigger accepts:

| Input Type | Accepts | Example Items |
|------------|---------|---------------|
| **Water** | ItemType.Liquid | Water, Milk, Coffee |
| **Coffee** | ItemType.Capsule | Espresso Capsule, Decaf Capsule |
| **Extra** | ItemType.Additive | Sugar, Cream, Syrup |

---

### References

#### Required:
- **Action Tracker**: Stores machine state (liquids, capsules, additives)
- **System Messages**: Contains dialogue messages
- **Dialogue Manager**: Shows feedback to player

#### Optional:
- **Display Handler**: Updates visual displays when items are added

**Note:** Script will auto-find references if not assigned!

---

## Visual Feedback

### Gizmo Colors (Editor Only)

Trigger zones are color-coded in the Scene view:

| Input Type | Color | Visual |
|------------|-------|--------|
| Water | Blue | ?? |
| Coffee | Brown | ?? |
| Extra | Yellow | ?? |

This helps with positioning and debugging trigger zones.

---

## How Items Are Processed

### Water Input (Liquid)

```csharp
When liquid enters trigger:
1. Check: Is it ItemType.Liquid?
   ?? YES: Continue
   ?? NO: Show warning, reject
2. Set actionTracker.LiquidName = item name
3. Increment actionTracker.LiquidAmount++
4. Update display (if assigned)
5. Destroy held item
6. Log: "Added [name] to water input. Total liquid: X"
```

**Example:**
```
Player holds "Water"
? Moves into water input trigger
? LiquidName = "Water"
? LiquidAmount = 1
? Display updates
? "Water" object destroyed
```

---

### Coffee Input (Capsule)

```csharp
When capsule enters trigger:
1. Check: Is it ItemType.Capsule?
   ?? YES: Continue
   ?? NO: Show warning, reject
2. Try to add to CapsuleA:
   ?? Empty: Add here, done
   ?? Full: Try CapsuleB
3. Try to add to CapsuleB:
   ?? Empty: Add here, done
 ?? Full: Show "Capsule Full" dialogue
4. Update display (if assigned)
5. Destroy held item (if added)
6. Log result
```

**Example:**
```
Player holds "Espresso"
? Moves into coffee input trigger
? CapsuleA empty ? CapsuleAName = "Espresso"
? Display updates
? "Espresso" object destroyed

Player holds "Decaf"
? Moves into coffee input trigger
? CapsuleA full ? Check CapsuleB
? CapsuleB empty ? CapsuleBName = "Decaf"
? Display updates
? "Decaf" object destroyed

Player holds "Dark Roast"
? Moves into coffee input trigger
? Both slots full ? Show "Capsule Full" message
? Item NOT destroyed (still held)
```

---

### Extra Input (Additive)

```csharp
When additive enters trigger:
1. Check: Is it ItemType.Additive?
   ?? YES: Continue
   ?? NO: Show warning, reject
2. Set actionTracker.AdditiveName = item name
3. Update display (if assigned)
4. Destroy held item
5. Log: "Added [name] to extra input"
```

**Example:**
```
Player holds "Sugar"
? Moves into extra input trigger
? AdditiveName = "Sugar"
? Display updates
? "Sugar" object destroyed
```

---

## Debug Output

### With showDebugInfo = true:

**Successful Water Add:**
```
[MachineInputTrigger] Added Water to water input. Total liquid: 1
```

**Successful Capsule Add:**
```
[MachineInputTrigger] Added Espresso to Capsule A slot
```

**Capsule Slots Full:**
```
[MachineInputTrigger] Cannot add DarkRoast - both capsule slots are full!
```

**Wrong Item Type:**
```
[MachineInputTrigger] Water input only accepts Liquid items! Received: Capsule
```

**Not Being Held:**
```
[MachineInputTrigger] Object Water is not being held
```

---

## Integration with Existing System

### Relationship with ClickableObject

The trigger system works seamlessly with held objects:

```csharp
// Trigger checks if object is held
if (!ClickableObject.IsAnyItemHeld)
    return; // Ignore if not held

if (ClickableObject.GetHeldObject() != clickableObject)
    return; // Ignore if different object held
```

This ensures:
- ? Only held items are processed
- ? Items on the ground are ignored
- ? Only the currently held item triggers

---

### Relationship with ItemInteractionHandler

The old click-based system can coexist:

**Option 1: Keep Both Systems**
- Triggers for intuitive input
- Clicks for backup/alternative

**Option 2: Disable Click Handling**
- Remove ItemInteractionHandler calls
- Use triggers exclusively

---

### Relationship with DisplayHandler

Triggers automatically update displays:

```csharp
if (displayHandler != null)
{
    displayHandler.UpdateDisplay();
}
```

Make sure to assign DisplayHandler in Inspector for visual feedback!

---

## Advantages Over Click System

### 1. More Intuitive
```
Old: Hold + Aim + Click
New: Hold + Move Near
```

### 2. Clearer Feedback
```
Trigger zones visible in editor
Color-coded for each type
Clear spatial boundaries
```

### 3. More Forgiving
```
Old: Must click exact object
New: Just enter zone area
```

### 4. Better for VR/3D
```
Natural physical interaction
No precise clicking needed
Works like real-world insertion
```

---

## Troubleshooting

### Items Not Being Detected

**Check:**
1. Is collider set to trigger? (Auto-set by script)
2. Is object named "Input Trigger"?
3. Is MachineInputTrigger component attached?
4. Is item being held when entering trigger?
5. Check console for debug messages

---

### Wrong Items Accepted

**Check:**
1. Input Type setting matches intent:
   - Water ? Liquid
   - Coffee ? Capsule
   - Extra ? Additive
2. Item's ItemType is correct
3. Check debug logs for type mismatches

---

### Items Disappearing Without Adding

**Check:**
1. Are references assigned?
   - PlayerActionTracker
   - SystemMessages
   - DialogueManager
2. Check console for error messages
3. Enable showDebugInfo to trace execution

---

### Display Not Updating

**Check:**
1. Is DisplayHandler assigned in Inspector?
2. Does DisplayHandler have UpdateDisplay() method?
3. Is display properly connected to action tracker?

---

## Testing Checklist

### Test 1: Water Input
```
? Create trigger zone under MachineWaterInput
? Add MachineInputTrigger (Type: Water)
? Assign references
? Pick up liquid item
? Move into trigger zone
? Verify: Item disappears, liquid added
? Check display updates
```

### Test 2: Coffee Input (Single Capsule)
```
? Create trigger zone under MachineCoffeeInput
? Add MachineInputTrigger (Type: Coffee)
? Assign references
? Pick up capsule
? Move into trigger zone
? Verify: Added to Capsule A
? Check display updates
```

### Test 3: Coffee Input (Two Capsules)
```
? Add first capsule (should go to A)
? Add second capsule (should go to B)
? Try third capsule (should show "full" message)
? Verify third capsule NOT destroyed
```

### Test 4: Extra Input
```
? Create trigger zone under MachineExtraInput
? Add MachineInputTrigger (Type: Extra)
? Assign references
? Pick up additive
? Move into trigger zone
? Verify: Item disappears, additive added
? Check display updates
```

### Test 5: Wrong Item Types
```
? Try capsule in water input ? Should reject
? Try liquid in coffee input ? Should reject
? Try additive in coffee input ? Should reject
? Check console warnings appear
```

### Test 6: Not Held Items
```
? Drop item on ground
? Push it into trigger (not holding it)
? Verify: Item NOT processed
? Check debug log confirms not held
```

---

## Performance

### Trigger Detection Cost:
```
OnTriggerEnter: Called when object enters
Processing: ~0.1-0.5ms per trigger
Impact: Negligible

Only processes:
- ClickableObject components
- Held items only
- Valid item types only
```

### Memory:
```
Per trigger: ~1KB
3 triggers: ~3KB total
Impact: Minimal
```

---

## Migration Guide

### From Old System to New

**Step 1:** Create Trigger Objects
```
For each input (Water, Coffee, Extra):
1. Create child object named "Input Trigger"
2. Add Box Collider
3. Add MachineInputTrigger component
4. Set Input Type
5. Assign references
```

**Step 2:** Test Both Systems
```
Keep ItemInteractionHandler active
Test trigger system
Verify both work correctly
```

**Step 3:** (Optional) Remove Click System
```
If triggers work perfectly:
1. Remove ItemInteractionHandler references
2. Remove input type checks from handler
3. Use triggers exclusively
```

---

## Summary

### What Changed:
1. ? Created MachineInputTrigger.cs
2. ? Trigger-based detection system
3. ? Automatic item processing
4. ? Visual feedback in editor

### How to Use:
1. Create "Input Trigger" child objects
2. Add colliders (trigger)
3. Add MachineInputTrigger component
4. Set input type (Water/Coffee/Extra)
5. Assign references
6. Test!

### Benefits:
- ? More intuitive interaction
- ? Better spatial awareness
- ? Visual trigger zones
- ? Clearer feedback
- ? Works with existing system

### Result:
**Natural, intuitive machine input with clear visual zones and automatic processing!** ???

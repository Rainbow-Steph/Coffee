# Quick Setup Guide: Machine Input Triggers

## ? **System Created Successfully!**

The new trigger-based input system is ready to use. Here's how to set it up in Unity:

---

## Step-by-Step Setup

### 1. Locate Your Machine Inputs

Find these objects in your scene hierarchy:
- `MachineWaterInput`
- `MachineCoffeeInput`
- `MachineExtraInput`

---

### 2. Create Trigger Zones

For **EACH** input object above:

#### A. Create Child Object:
```
1. Right-click the input object
2. Select "Create Empty"
3. Name it exactly: "Input Trigger"
```

#### B. Add Collider:
```
1. Select the "Input Trigger" object
2. Add Component ? Box Collider
3. Adjust size to cover the input area
4. Position as needed (Center property)
```

**Recommended Sizes:**
```
Water Input:   Size (0.5, 0.5, 0.5)
Coffee Input:  Size (0.3, 0.3, 0.3)
Extra Input:   Size (0.3, 0.3, 0.3)
```

---

### 3. Add MachineInputTrigger Component

For each "Input Trigger" object:

```
1. Select "Input Trigger"
2. Add Component ? Machine Input Trigger
3. Configure settings:
```

#### **For MachineWaterInput ? Input Trigger:**
```
Input Type: Water
Action Tracker: [Drag PlayerActionTracker asset]
System Messages: [Drag SystemMessages asset]
Dialogue Manager: [Drag DialogueManager from scene]
Display Handler: [Optional - your display]
Show Debug Info: ? Checked
```

#### **For MachineCoffeeInput ? Input Trigger:**
```
Input Type: Coffee
Action Tracker: [Drag PlayerActionTracker asset]
System Messages: [Drag SystemMessages asset]
Dialogue Manager: [Drag DialogueManager from scene]
Display Handler: [Optional - your display]
Show Debug Info: ? Checked
```

#### **For MachineExtraInput ? Input Trigger:**
```
Input Type: Extra
Action Tracker: [Drag PlayerActionTracker asset]
System Messages: [Drag SystemMessages asset]
Dialogue Manager: [Drag DialogueManager from scene]
Display Handler: [Optional - your display]
Show Debug Info: ? Checked
```

---

## Final Hierarchy

Your machine should look like this:

```
Machine
?? MachineWaterInput
?  ?? Input Trigger
?   ?? Box Collider (Is Trigger: ?)
?     ?? MachineInputTrigger (Type: Water)
?
?? MachineCoffeeInput
?  ?? Input Trigger
? ?? Box Collider (Is Trigger: ?)
?     ?? MachineInputTrigger (Type: Coffee)
?
?? MachineExtraInput
   ?? Input Trigger
      ?? Box Collider (Is Trigger: ?)
   ?? MachineInputTrigger (Type: Extra)
```

---

## Testing

### Test Water Input:
```
1. Play the game
2. Pick up a water/liquid item
3. Move it into the water input trigger zone
4. Object should disappear
5. Check console: "Added [name] to water input"
6. Display should update
```

### Test Coffee Input:
```
1. Pick up a capsule
2. Move into coffee input trigger zone
3. Object should disappear
4. Check console: "Added [name] to Capsule A"
5. Repeat with second capsule
6. Third capsule should show "full" message
```

### Test Extra Input:
```
1. Pick up an additive (sugar, etc.)
2. Move into extra input trigger zone
3. Object should disappear
4. Check console: "Added [name] to extra input"
5. Display should update
```

---

## Visual Feedback

In the **Scene View**, you'll see colored wireframes showing trigger zones:

- ?? **Blue** = Water input trigger
- ?? **Brown** = Coffee input trigger
- ?? **Yellow** = Extra input trigger

This helps you position and size the triggers correctly!

---

## Troubleshooting

### Items Not Detected?

**Check:**
1. Is the object named exactly "Input Trigger"?
2. Is Box Collider marked as "Is Trigger"? (Auto-set by script)
3. Is MachineInputTrigger component attached?
4. Are references assigned (ActionTracker, etc.)?
5. Is item being HELD when entering trigger?

### Wrong Items Accepted?

**Check:**
1. Input Type matches:
   - Water ? Accepts Liquid
   - Coffee ? Accepts Capsule
   - Extra ? Accepts Additive
2. Item has correct ItemType set

### Display Not Updating?

**Check:**
1. Display Handler assigned in Inspector
2. Display properly connected to ActionTracker

---

## Optional: Auto-Find References

The script will automatically find references if not assigned:
- PlayerActionTracker
- SystemMessages
- DialogueManager

But it's recommended to assign them manually for reliability!

---

## Console Output (Debug Mode)

With "Show Debug Info" checked, you'll see:

**On Success:**
```
[MachineInputTrigger] Input Trigger initialized as Water input trigger
[MachineInputTrigger] Added Water to water input. Total liquid: 1
[MachineInputTrigger] Successfully processed Water for Water input
```

**On Failure:**
```
[MachineInputTrigger] Water input only accepts Liquid items! Received: Capsule
[MachineInputTrigger] Object Water is not being held
```

---

## Quick Reference

| Input Type | Accepts | Component Setting |
|------------|---------|-------------------|
| **MachineWaterInput** | Liquid items | Input Type: **Water** |
| **MachineCoffeeInput** | Capsule items | Input Type: **Coffee** |
| **MachineExtraInput** | Additive items | Input Type: **Extra** |

---

## That's It!

The system is now ready. When players hold items and move them into the trigger zones, they'll automatically be added to the machine - no clicking required!

**Natural, intuitive, and physically realistic!** ???

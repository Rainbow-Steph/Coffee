# Machine Input System Implementation Summary

## Overview
The machine input system has been successfully implemented with three specialized input types that validate item compatibility before accepting them.

## Item Types Added
The following new item types were added to `ItemType.cs`:
- `MachineWaterInput` - Accepts only Liquid items
- `MachineCoffeeInput` - Accepts only Capsule items
- `MachineExtraInput` - Accepts only Additive items

## Implementation Details

### 1. Machine Water Input (`MachineWaterInput`)
**Behavior:**
- Only accepts items of type `Liquid`
- Updates `PlayerActionTracker.LiquidName` with the liquid's name
- Destroys the held liquid object after placement
- Shows warning if non-liquid item is used

**Code Location:** `HandleMachineWaterInput()` in `ItemInteractionHandler.cs`

### 2. Machine Coffee Input (`MachineCoffeeInput`)
**Behavior:**
- Only accepts items of type `Capsule`
- Checks `CapsuleAName` first - if empty, places capsule there
- If `CapsuleAName` is full, checks `CapsuleBName` - if empty, places capsule there
- If both slots are full:
  - Triggers "Capsule Full" dialogue from `SystemMessages` SO
  - Does NOT destroy the held capsule
- Destroys the capsule object after successful placement
- Shows warning if non-capsule item is used

**Code Location:** `HandleMachineCoffeeInput()` and `HandleCapsuleInteraction()` in `ItemInteractionHandler.cs`

### 3. Machine Extra Input (`MachineExtraInput`)
**Behavior:**
- Only accepts items of type `Additive`
- Updates `PlayerActionTracker.AdditiveName` with the additive's name
- Destroys the held additive object after placement
- Shows warning if non-additive item is used

**Code Location:** `HandleMachineExtraInput()` in `ItemInteractionHandler.cs`

## System Messages ScriptableObject

### Location
`Assets/Scripts/SystemMessages.cs`

### Structure
```csharp
public class SystemMessages : ScriptableObject
{
    [Header("Error Messages")]
    public DialogueNodeSO capsuleFullMessage;
}
```

### How to Create
1. Right-click in Project window
2. Navigate to: Create ? Coffee ? System Messages
3. Name it "SystemMessages"
4. Assign a DialogueNodeSO to the `capsuleFullMessage` field

### Dialogue Nodes Required
- **Capsule Full** - Triggered when both capsule slots are occupied

## Setup Instructions

### 1. Create System Messages Asset
1. Right-click in Project ? Create ? Coffee ? System Messages
2. Create a DialogueNodeSO for "Capsule Full" message
3. Assign it to the SystemMessages asset

### 2. Create Dialogue for "Capsule Full"
1. Right-click in Project ? Create ? Dialogue ? Dialogue Node
2. Set up the dialogue text (e.g., "Both capsule slots are full!")
3. Configure any additional dialogue settings

### 3. Configure ItemInteractionHandler
1. Create an empty GameObject in your scene (or use existing manager)
2. Add `ItemInteractionHandler` component
3. Assign references:
   - **Player Action Tracker** - Your PlayerActionTracker ScriptableObject
   - **System Messages** - Your SystemMessages ScriptableObject
   - **Dialogue Manager** - Reference to DialogueManager in scene

### 4. Configure Machine Input Objects
For each machine input point:
1. Add `ClickableObject` component
2. Set `Item Type` to appropriate type:
   - `Machine Water Input` for liquid slot
   - `Machine Coffee Input` for capsule slots
   - `Machine Extra Input` for additive slot
3. Assign the `ItemInteractionHandler` reference

### 5. Configure Items
For each interactable item:
1. Add `ClickableObject` component
2. Set `Item Type` to:
   - `Liquid` for water/liquids
   - `Capsule` for coffee capsules
   - `Additive` for salt/sugar/pepper/extras
3. Enable `Float On Click` if you want pick-up behavior
4. Assign `Player Action Tracker` reference

## Error Handling

### Validation Messages
- **Wrong Item Type:** "Water input only accepts liquid items!" (etc.)
- **Capsule Full:** Triggers dialogue instead of console message
- **Missing References:** "System Messages or Capsule Full message not assigned!"

### Debug Logging
Success messages are logged to console:
- "Added [ItemName] to water input"
- "Added [ItemName] to coffee input"
- "Added [ItemName] to extra input"

## Integration with PlayerActionTracker

The system updates the following fields in `PlayerActionTracker`:
- `LiquidName` - Set by Water Input
- `CapsuleAName` - Set by Coffee Input (first slot)
- `CapsuleBName` - Set by Coffee Input (second slot)
- `AdditiveName` - Set by Extra Input

All changes trigger the `onMachineContentsChanged` event, which can be used to:
- Update UI displays
- Trigger game logic
- Check recipe completion
- Enable/disable machine operation

## Testing Checklist

- [ ] Water Input accepts Liquid items
- [ ] Water Input rejects non-Liquid items
- [ ] Coffee Input accepts Capsule items
- [ ] Coffee Input rejects non-Capsule items
- [ ] Coffee Input fills CapsuleA first
- [ ] Coffee Input fills CapsuleB when CapsuleA is full
- [ ] Coffee Input shows "Capsule Full" dialogue when both slots full
- [ ] Coffee Input doesn't destroy capsule when full
- [ ] Extra Input accepts Additive items
- [ ] Extra Input rejects non-Additive items
- [ ] All successful placements destroy the held item
- [ ] PlayerActionTracker updates correctly
- [ ] Debug messages appear in console

## Future Enhancements

Potential improvements to consider:
1. Visual feedback on machine inputs when hovering with correct item
2. Particle effects when placing items
3. Sound effects for placement/rejection
4. Animation for items being placed
5. UI indicators showing which inputs are filled
6. Ability to remove items from inputs
7. Different dialogue messages for each input type rejection
8. Recipe validation system
9. Machine operation button that checks if all inputs are filled

## Files Modified/Created

### Modified Files
- `Assets/Scripts/ItemType.cs` - Added new enum values
- `Assets/Scripts/Interactions/ItemInteractionHandler.cs` - Updated with new handlers

### Existing Files (No Changes Needed)
- `Assets/Scripts/SystemMessages.cs` - Already has correct structure
- `Assets/Scripts/Interactions/PlayerActionTracker.cs` - Already supports required fields
- `Assets/Scripts/Dialogue/DialogueManager.cs` - Already handles dialogue display

## Notes
- The system is fully implemented and ready to use
- Item validation prevents incorrect items from being placed
- The capsule system supports exactly 2 capsules with proper overflow handling
- All item placements are logged for debugging
- The system integrates seamlessly with the existing PlayerActionTracker and Dialogue systems

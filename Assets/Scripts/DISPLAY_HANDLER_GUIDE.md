# Display Handler System Guide

## Overview
The Display Handler system automatically updates visual displays on the coffee machine based on the contents tracked in the PlayerActionTracker. When liquids, capsules, or additives are added, the corresponding display objects change materials to reflect the current state.

## Components Created

### 1. DisplayHandlerConfig.cs (ScriptableObject)
Configuration asset that stores:
- References to all display GameObjects
- Materials for all display states
- Name identifiers for matching items to materials

### 2. DisplayHandler.cs (MonoBehaviour)
Runtime component that:
- Listens to PlayerActionTracker changes
- Updates display materials based on current machine contents
- Handles color matching for capsules and additives

## Setup Instructions

### Step 1: Create DisplayHandlerConfig Asset

1. Right-click in Project window
2. Navigate to: **Create ? Coffee ? Display Handler Config**
3. Name it "DisplayHandlerConfig"

### Step 2: Configure Display GameObjects

In the DisplayHandlerConfig asset, assign these references:

**Water Displays:**
- `Water Display 1` - First water indicator
- `Water Display 2` - Second water indicator
- `Water Display 3` - Third water indicator

**Coffee Displays:**
- `Coffee Display 1` - Shows Capsule A contents
- `Coffee Display 2` - Shows Capsule B contents

**Extra Display:**
- `Extra Display` - Shows additive contents

### Step 3: Assign Materials

Create or assign materials for each state:

#### Water Display Materials
- **Water Empty Material** - Default empty state
- **Water Filled Material** - When liquid is added

#### Coffee Display Materials
- **Coffee Empty Material** - Default empty state
- **Red Capsule Material** - When red capsule is added
- **Blue Capsule Material** - When blue capsule is added
- **Black Capsule Material** - When black capsule is added

#### Extra Display Materials
- **Extra Empty Material** - Default empty state
- **Salt Material** - When salt is added
- **Sugar Material** - When sugar is added
- **Pepper Material** - When pepper is added

### Step 4: Configure Name Identifiers

Set up text identifiers that match your item names:

**Capsule Identifiers:**
- Red Capsule Identifier: "Red" (default)
- Blue Capsule Identifier: "Blue" (default)
- Black Capsule Identifier: "Black" (default)

**Extra Identifiers:**
- Salt Identifier: "Salt" (default)
- Sugar Identifier: "Sugar" (default)
- Pepper Identifier: "Pepper" (default)

> **Note:** These identifiers are case-sensitive and check if the item name **contains** the identifier text.

### Step 5: Setup DisplayHandler in Scene

1. Create an empty GameObject (or use existing manager)
2. Name it "DisplayHandler"
3. Add the `DisplayHandler` component
4. Assign references:
   - **Player Action Tracker** - Your PlayerActionTracker ScriptableObject
   - **Display Config** - The DisplayHandlerConfig asset you created
5. Optionally enable **Show Debug Logs** for testing

## How It Works

### Water Display Updates

When `PlayerActionTracker.LiquidName` changes:
- If liquid name is **not empty**: All 3 water displays use `waterFilledMaterial`
- If liquid name **is empty**: All 3 water displays use `waterEmptyMaterial`

### Coffee Display Updates

When `PlayerActionTracker.CapsuleAName` or `CapsuleBName` changes:

**Coffee Display 1** (shows Capsule A):
- Empty ? Uses `coffeeEmptyMaterial`
- Contains "Red" ? Uses `redCapsuleMaterial`
- Contains "Blue" ? Uses `blueCapsuleMaterial`
- Contains "Black" ? Uses `blackCapsuleMaterial`

**Coffee Display 2** (shows Capsule B):
- Same logic as Display 1

### Extra Display Updates

When `PlayerActionTracker.AdditiveName` changes:
- Empty ? Uses `extraEmptyMaterial`
- Contains "Salt" ? Uses `saltMaterial`
- Contains "Sugar" ? Uses `sugarMaterial`
- Contains "Pepper" ? Uses `pepperMaterial`

## Event Flow

```
1. Player clicks machine input with held item
   ?
2. ItemInteractionHandler processes interaction
   ?
3. PlayerActionTracker properties are updated
   ?
4. PlayerActionTracker.onMachineContentsChanged event fires
   ?
5. DisplayHandler receives event
   ?
6. DisplayHandler updates appropriate display materials
```

## Example Item Names

For the display system to work correctly, name your items appropriately:

### Good Naming Examples:
- **Capsules:** "Red Coffee Capsule", "Blue_Capsule", "BlackCapsule"
- **Additives:** "Salt Shaker", "Sugar_Packet", "Pepper Grinder"
- **Liquids:** "Water Bottle", "Hot Water", "Filtered Water"

### Poor Naming Examples:
- **Capsules:** "Capsule1", "Type_A", "Coffee" (doesn't contain color)
- **Additives:** "Additive1", "Extra", "Seasoning" (doesn't contain type)

## Public Methods

### DisplayHandler.RefreshDisplays()
Manually updates all displays based on current PlayerActionTracker state.

```csharp
displayHandler.RefreshDisplays();
```

### DisplayHandler.ResetDisplays()
Resets all displays to empty state (useful when clearing the machine).

```csharp
displayHandler.ResetDisplays();
```

## Integration with Existing Systems

### PlayerActionTracker Integration
The DisplayHandler automatically subscribes to:
- `onMachineContentsChanged` event

No additional code needed - it just works!

### ItemInteractionHandler Integration
When items are placed in machine inputs:
1. ItemInteractionHandler updates PlayerActionTracker
2. PlayerActionTracker fires onMachineContentsChanged
3. DisplayHandler receives event and updates displays

No changes needed to ItemInteractionHandler!

## Debug Features

### Enable Debug Logs
In the DisplayHandler component, check **Show Debug Logs** to see:
- When machine contents change
- Which displays are being updated
- Material assignments
- Warnings for unmatched item names

### Validation
The DisplayHandlerConfig automatically validates:
- All display renderers are assigned
- All materials are assigned
- Logs warnings if any references are missing

## Customization

### Adding New Capsule Colors

1. Add new material to DisplayHandlerConfig
2. Add new identifier string
3. Update `GetCapsuleMaterial()` method:

```csharp
else if (capsuleName.Contains(displayConfig.greenCapsuleIdentifier))
{
    return displayConfig.greenCapsuleMaterial;
}
```

### Adding New Additives

1. Add new material to DisplayHandlerConfig
2. Add new identifier string
3. Update `GetAdditiveMaterial()` method:

```csharp
else if (additiveName.Contains(displayConfig.cinnamonIdentifier))
{
    return displayConfig.cinnamonMaterial;
}
```

### Custom Display Behavior

Override or extend the update methods:
- `UpdateWaterDisplays()` - Water display logic
- `UpdateCoffeeDisplays()` - Coffee display logic
- `UpdateExtraDisplay()` - Extra display logic

## Testing Checklist

- [ ] DisplayHandlerConfig asset created
- [ ] All 6 display GameObject references assigned
- [ ] All water materials assigned (empty + filled)
- [ ] All coffee materials assigned (empty + 3 colors)
- [ ] All extra materials assigned (empty + 3 types)
- [ ] DisplayHandler component added to scene
- [ ] PlayerActionTracker reference assigned
- [ ] DisplayHandlerConfig reference assigned
- [ ] Test adding liquid - water displays update
- [ ] Test adding red capsule - coffee display shows red
- [ ] Test adding blue capsule - coffee display shows blue
- [ ] Test adding black capsule - coffee display shows black
- [ ] Test adding second capsule - both displays show correctly
- [ ] Test adding salt - extra display shows salt
- [ ] Test adding sugar - extra display shows sugar
- [ ] Test adding pepper - extra display shows pepper
- [ ] Test clearing machine - displays reset to empty

## Performance Notes

- Updates only occur when machine contents change (event-based)
- Material changes are instant (no lerping or animation)
- No Update() loop - zero performance cost when idle
- Automatic subscription/unsubscription prevents memory leaks

## Error Handling

The system handles:
- Missing display references (logs warning)
- Missing material references (logs warning)
- Unmatched item names (uses empty material + logs warning)
- Null PlayerActionTracker (disables component + logs error)
- Null DisplayHandlerConfig (disables component + logs error)

## Future Enhancements

Potential improvements:
1. Material lerping/fading animations
2. Particle effects on material change
3. Sound effects when displays update
4. Multi-material support (change multiple materials per display)
5. Custom shader property updates (colors, values, etc.)
6. Display animations (rotation, scaling, etc.)
7. Sequential display updates (water fills one at a time)
8. Amount-based display states (partial fill levels)

## Files Created

- `Assets/Scripts/DisplayHandlerConfig.cs` - Configuration ScriptableObject
- `Assets/Scripts/DisplayHandler.cs` - Runtime display manager
- `Assets/Scripts/DISPLAY_HANDLER_GUIDE.md` - This guide

## Dependencies

- `PlayerActionTracker.cs` - For machine contents tracking
- `ItemInteractionHandler.cs` - For updating machine contents
- Unity's Renderer component - For material assignment

## Example Workflow

```csharp
// 1. Player picks up red capsule
ClickableObject.floatOnClick = true;
// Red Capsule is now held

// 2. Player clicks Machine - Coffee Input
ItemInteractionHandler.HandleMachineCoffeeInput(heldCapsule);

// 3. PlayerActionTracker updates
actionTracker.CapsuleAName = "Red Coffee Capsule";

// 4. Event fires
actionTracker.onMachineContentsChanged?.Invoke(machineContents);

// 5. DisplayHandler receives event
DisplayHandler.OnMachineContentsChanged(contents);

// 6. Coffee Display 1 updates
coffeeDisplay1.material = redCapsuleMaterial;

// Result: Coffee Display 1 now shows red material!
```

## Troubleshooting

### Displays not updating
- Check DisplayHandler is enabled
- Verify PlayerActionTracker reference is assigned
- Ensure DisplayHandlerConfig is assigned and validated
- Check debug logs to see if events are firing

### Wrong materials showing
- Verify item names contain the correct identifiers
- Check identifier strings in DisplayHandlerConfig
- Enable debug logs to see material matching logic
- Ensure materials are assigned in DisplayHandlerConfig

### All displays show empty
- Check if PlayerActionTracker is actually updating
- Verify onMachineContentsChanged event is firing
- Ensure DisplayHandler is subscribed to event
- Check if DisplayHandler.OnEnable() ran successfully

### Materials not changing
- Verify Renderer components exist on display GameObjects
- Check if materials are properly assigned in config
- Ensure display GameObjects are active in scene
- Verify no other scripts are overriding materials

## Support

For issues or questions:
1. Enable debug logs in DisplayHandler
2. Check Unity Console for warnings/errors
3. Verify all references in Inspector
4. Test with simple item names (e.g., "Red", "Blue", "Salt")
5. Review this guide's setup instructions

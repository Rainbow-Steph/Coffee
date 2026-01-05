# DisplayHandler Update - Display Type Selection

## Changes Made

The DisplayHandler script now allows you to assign display GameObjects directly in the DisplayHandler component, with automatic assignment to the DisplayHandlerConfig when the game starts.

## New Features

### 1. DisplayType Enum
```csharp
public enum DisplayType
{
    WaterDisplay1,
    WaterDisplay2,
    WaterDisplay3,
    CoffeeDisplay1,
    CoffeeDisplay2,
    ExtraDisplay
}
```

### 2. DisplayAssignment Class
```csharp
[System.Serializable]
public class DisplayAssignment
{
    public DisplayType displayType;
    public GameObject displayGameObject;
}
```

### 3. Automatic Assignment
- Assignments happen in `Awake()` before any other initialization
- DisplayHandler automatically assigns GameObjects to DisplayHandlerConfig
- Clears cached renderers before reassigning

## How to Use

### Setup in Unity Inspector

1. **Add DisplayHandler to Scene:**
   - Create empty GameObject or use existing manager
   - Add DisplayHandler component

2. **Assign References:**
   - **Player Action Tracker** - Your PlayerActionTracker SO
   - **Display Config** - Your DisplayHandlerConfig SO

3. **Configure Display Assignments:**
   
   In the "Display Assignments" section, you'll see an array with 6 slots:

   ```
   Display Assignments (Size: 6)
   ????????????????????????????????????????
   ? Element 0          ?
   ?   Display Type: [Water Display 1]   ?
   ?   Display GameObject: [Drag Here]   ?
   ????????????????????????????????????????
   ? Element 1        ?
   ?   Display Type: [Water Display 2]   ?
   ?   Display GameObject: [Drag Here]?
   ????????????????????????????????????????
   ? Element 2      ?
   ?   Display Type: [Water Display 3]   ?
   ?   Display GameObject: [Drag Here]   ?
   ????????????????????????????????????????
   ? Element 3           ?
   ?   Display Type: [Coffee Display 1]  ?
   ?   Display GameObject: [Drag Here]   ?
   ????????????????????????????????????????
   ? Element 4         ?
   ?   Display Type: [Coffee Display 2]  ?
   ?   Display GameObject: [Drag Here]   ?
   ????????????????????????????????????????
   ? Element 5    ?
   ?   Display Type: [Extra Display]     ?
   ?   Display GameObject: [Drag Here]   ?
   ????????????????????????????????????????
   ```

4. **For Each Element:**
   - Select **Display Type** from dropdown
   - Drag **GameObject** from hierarchy

## Workflow Comparison

### Old Workflow (Manual Assignment)
1. Create DisplayHandlerConfig asset
2. Open DisplayHandlerConfig in inspector
3. Drag 6 GameObjects into config
4. Assign all materials
5. Reference config in DisplayHandler

### New Workflow (Automatic Assignment)
1. Create DisplayHandlerConfig asset
2. Assign materials in config
3. Add DisplayHandler to scene
4. Assign 6 GameObjects in DisplayHandler with types
5. GameObjects automatically assigned to config on game start!

## Benefits

? **Centralized Configuration** - All display assignments in one place (DisplayHandler)
? **Type Safety** - Dropdown ensures correct display type selection
? **Visual Clarity** - See exactly which GameObject is assigned to which display type
? **Automatic** - No need to manually configure DisplayHandlerConfig asset
? **Runtime Support** - Can reassign displays at runtime with `ReassignDisplays()`
? **Debug Friendly** - Shows assignment logs when debug is enabled

## Example Setup

```csharp
// In Unity Inspector:

DisplayHandler Component:
?? Player Action Tracker: [PlayerActionTracker]
?? Display Config: [DisplayHandlerConfig]
?? Display Assignments (6):
    ?? [0] Water Display 1 ? Machine_WaterIndicator_1
    ?? [1] Water Display 2 ? Machine_WaterIndicator_2
    ?? [2] Water Display 3 ? Machine_WaterIndicator_3
    ?? [3] Coffee Display 1 ? Machine_CapsuleDisplay_A
    ?? [4] Coffee Display 2 ? Machine_CapsuleDisplay_B
    ?? [5] Extra Display ? Machine_AdditiveDisplay
```

## Code Flow

```
Game Starts
    ?
DisplayHandler.Awake()
    ?
AssignDisplaysToConfig()
    ?
displayConfig.waterDisplay1 = assignedGameObject
displayConfig.waterDisplay2 = assignedGameObject
displayConfig.waterDisplay3 = assignedGameObject
displayConfig.coffeeDisplay1 = assignedGameObject
displayConfig.coffeeDisplay2 = assignedGameObject
displayConfig.extraDisplay = assignedGameObject
    ?
displayConfig.ClearRendererCache()
    ?
Ready for use! ?
```

## Runtime Methods

### ReassignDisplays()
Manually reassign all displays to config (useful if GameObjects change):
```csharp
displayHandler.ReassignDisplays();
```

### RefreshDisplays()
Manually update all display materials:
```csharp
displayHandler.RefreshDisplays();
```

### ResetDisplays()
Reset all displays to empty state:
```csharp
displayHandler.ResetDisplays();
```

## Debug Logging

Enable "Show Debug Logs" to see:
- Which GameObjects are assigned to which display types
- Total number of assignments
- When displays are updated
- When displays are reassigned

Example log output:
```
[DisplayHandler] Assigned Machine_WaterIndicator_1 to Water Display 1
[DisplayHandler] Assigned Machine_WaterIndicator_2 to Water Display 2
[DisplayHandler] Assigned Machine_WaterIndicator_3 to Water Display 3
[DisplayHandler] Assigned Machine_CapsuleDisplay_A to Coffee Display 1
[DisplayHandler] Assigned Machine_CapsuleDisplay_B to Coffee Display 2
[DisplayHandler] Assigned Machine_AdditiveDisplay to Extra Display
[DisplayHandler] All displays assigned to config. Total assignments: 6
```

## Important Notes

?? **Assignment Timing**
- Assignments happen in `Awake()` (before `OnEnable()`)
- Ensures displays are configured before any events fire
- Safe to access in `Start()` or later

?? **DisplayHandlerConfig**
- Still need to create the config asset
- Still need to assign materials in config
- GameObjects are now assigned automatically from DisplayHandler

?? **Renderer Components**
- Each GameObject still needs a Renderer component
- Validation still checks for Renderer components
- Warnings logged if Renderer is missing

## Migration from Old System

If you already have a DisplayHandlerConfig with GameObjects assigned:

1. **Option A - Keep Existing:**
   - Leave GameObjects in config
   - Don't set up Display Assignments in DisplayHandler
   - Works as before

2. **Option B - Switch to New System:**
   - Remove GameObject assignments from config
   - Add Display Assignments to DisplayHandler
   - Assignments will happen automatically on game start

Both options work! The new system just provides more convenience.

## Troubleshooting

### GameObjects not assigned to config
- Check Display Assignments array has 6 elements
- Verify each element has both DisplayType and GameObject set
- Enable debug logs to see assignment process

### Displays not updating
- Ensure DisplayHandlerConfig has all materials assigned
- Verify PlayerActionTracker reference is set
- Check debug logs for validation errors

### Wrong display updating
- Double-check DisplayType matches GameObject purpose
- Verify no duplicate display type assignments
- Enable debug logs to see which displays update

## Testing Checklist

- [ ] Create DisplayHandlerConfig asset
- [ ] Assign all materials in config
- [ ] Add DisplayHandler to scene
- [ ] Assign PlayerActionTracker reference
- [ ] Assign DisplayHandlerConfig reference
- [ ] Configure 6 display assignments with types
- [ ] Enable debug logs
- [ ] Enter Play mode
- [ ] Check Console for assignment logs
- [ ] Test adding liquid - verify water displays update
- [ ] Test adding capsules - verify coffee displays update
- [ ] Test adding additives - verify extra display updates
- [ ] Verify all 6 displays work correctly

## Files Modified

- `Assets/Scripts/DisplayHandler.cs` - Added DisplayType enum and automatic assignment system

## Files Unchanged

- `Assets/Scripts/DisplayHandlerConfig.cs` - Still stores materials and provides Renderer access
- All other display system files remain unchanged

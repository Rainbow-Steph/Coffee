# Coffee Crafting System Guide

## Overview
Complete crafting system for making coffee drinks with automatic tracking, recipe matching, and item spawning.

## Components Created

### 1. CraftingManager (ScriptableObject)
- Manages all craftable coffee recipes
- Matches ingredients to recipes
- Provides result prefabs

### 2. CoffeeMaker (MonoBehaviour)
- Handles the crafting logic
- Checks liquid availability
- Spawns crafted items
- Shows dialogue messages

### 3. Updated Components
- **PlayerActionTracker**: Added liquid amount tracking and auto-clear on game start
- **SystemMessages**: Added dialogue nodes for liquid needed and craft success
- **ClickableObject**: Can now trigger coffee making
- **ItemInteractionHandler**: Tracks liquid amount when added

## Setup Instructions

### Step 1: Create CraftingManager Asset

1. Right-click in Project window
2. Navigate to: **Create ? Coffee ? Crafting Manager**
3. Name it "CraftingManager"

### Step 2: Configure Recipes

In the CraftingManager asset, add recipes:

```
Recipes (List)
?? Element 0: Simple Coffee
?  ?? Recipe Name: "Simple Coffee"
?  ?? Required Liquid: "Water"
?  ?? Required Coffee 1: "Black"
?  ?? Required Coffee 2: (empty)
?  ?? Required Extra: (empty)
?  ?? Result Prefab: [Your coffee prefab]
?
?? Element 1: Double Shot Coffee
?  ?? Recipe Name: "Double Shot"
?  ?? Required Liquid: "Water"
?  ?? Required Coffee 1: "Black"
?  ?? Required Coffee 2: "Black"
?  ?? Required Extra: (empty)
?  ?? Result Prefab: [Your double shot prefab]
?
?? Element 2: Salted Caramel Coffee
   ?? Recipe Name: "Salted Caramel"
   ?? Required Liquid: "Water"
   ?? Required Coffee 1: "Red"
   ?? Required Coffee 2: (empty)
   ?? Required Extra: "Salt"
   ?? Result Prefab: [Your salted caramel prefab]
```

### Step 3: Update SystemMessages

Open your SystemMessages asset and assign:

**Error Messages:**
- **Capsule Full Message**: Existing dialogue
- **Liquid Needed Message**: NEW - Create dialogue saying "Add water to the machine!"

**Success Messages:**
- **Craft Success Message**: NEW - Create dialogue saying "Coffee ready!"

### Step 4: Setup CoffeeMaker Component

1. Create or select GameObject for coffee machine
2. Add **CoffeeMaker** component
3. Assign references:
   - **Player Action Tracker**: Your PlayerActionTracker SO
   - **Crafting Manager**: The CraftingManager asset you created
   - **System Messages**: Your SystemMessages SO
   - **Dialogue Manager**: DialogueManager from scene
   - **Spawn Point** (optional): Transform where coffee spawns
   - **Default Spawn Offset**: Offset if no spawn point (default: 0, 1, 0)
4. Optionally enable **Show Debug Logs** for testing

### Step 5: Setup Clickable Machine Button

On the GameObject that triggers coffee making (e.g., "Make Coffee Button"):

1. Add/Select **ClickableObject** component
2. In the "Coffee Making" section:
   - **Coffee Maker**: Assign the CoffeeMaker component from Step 4

Now clicking this object will attempt to make coffee!

## How It Works

### Liquid Tracking

```
Player adds liquid to machine
    ?
ItemInteractionHandler.HandleMachineWaterInput()
?
actionTracker.LiquidAmount++ (increments)
    ?
Liquid stored for crafting
```

### Coffee Making Process

```
Player clicks "Make Coffee" button
    ?
ClickableObject.OnClicked()
    ?
coffeeMaker.MakeCoffee()
    ?
Check liquid amount >= 1
    ?
YES: Find matching recipe
?    ?? Recipe found?
?  ?   YES: ?? Spawn prefab
?    ?        ?? Reduce liquid by 1
?    ?  ?? Clear capsules & additive
? ?        ?? Show success dialogue
?    ?   NO:  ?? (Silent fail / could add dialogue)
?    
NO:  Show "liquid needed" dialogue
```

### Recipe Matching

The system checks if machine contents match a recipe:

1. **Liquid**: Must contain required liquid name
2. **Coffee 1**: Must have required capsule in slot A or B
3. **Coffee 2** (optional): Must have second required capsule
4. **Extra** (optional): Must have required additive

**Example Match:**
```
Recipe Requires: Water + Black + Salt
Machine Has: "Water Bottle" + "Black Capsule" + "Salt Shaker"
Result: ? MATCH (spawns result prefab)
```

### Auto-Clear on Game Start

When the game starts, PlayerActionTracker automatically clears:
- Held item name
- Liquid amount (resets to 0)
- All machine contents
- Fires all change events

This happens in `OnEnable()`, so every time you enter Play mode starts fresh.

## Recipe Configuration

### Required Component Fields

All fields support partial string matching:

- **Required Liquid**: e.g., "Water" matches "Water Bottle", "Hot Water", etc.
- **Required Coffee 1**: e.g., "Black" matches "Black Capsule", "Black_Coffee", etc.
- **Required Coffee 2**: Optional second capsule (can be same or different color)
- **Required Extra**: e.g., "Salt" matches "Salt Shaker", "Salt_Item", etc.

### Optional Components

Leave a field **empty** to make it optional:
- Empty Coffee 2 = single capsule recipe
- Empty Extra = no additive needed

### Example Recipes

**Simple Black Coffee:**
```
Liquid: Water
Coffee 1: Black
Coffee 2: (empty)
Extra: (empty)
```

**Double Shot:**
```
Liquid: Water
Coffee 1: Black
Coffee 2: Black
Extra: (empty)
```

**Mocha with Sugar:**
```
Liquid: Water
Coffee 1: Black
Coffee 2: Red
Extra: Sugar
```

**Experimental Blend:**
```
Liquid: Water
Coffee 1: Red
Coffee 2: Blue
Extra: Pepper
```

## Public Methods

### CoffeeMaker.MakeCoffee()
Main crafting function - checks requirements and crafts if possible.

```csharp
coffeeMaker.MakeCoffee();
```

### CoffeeMaker.CanMakeCoffee()
Check if coffee can be made without actually making it.

```csharp
if (coffeeMaker.CanMakeCoffee())
{
  // Enable "Make Coffee" button
}
```

### PlayerActionTracker.ClearAll()
Manually clear all tracker data (called automatically on game start).

```csharp
actionTracker.ClearAll();
```

### CraftingManager.FindMatchingRecipe()
Find recipe matching given ingredients.

```csharp
var recipe = craftingManager.FindMatchingRecipe(
    liquid, capsuleA, capsuleB, additive
);
```

## Integration with Existing Systems

### With ItemInteractionHandler
? Already integrated - liquid amount increments when liquid added

### With DisplayHandler
? Works automatically - machine contents updates trigger display changes

### With DialogueManager
? Integrated - shows dialogue for:
- Liquid needed
- Craft success
- (Can add more: no recipe found, etc.)

### With InventoryManager (Future)
Could integrate to:
- Check if player has ingredients in inventory
- Remove ingredients from inventory when crafting
- Add crafted item to inventory instead of spawning

## Testing Checklist

- [ ] CraftingManager asset created
- [ ] At least one recipe configured with prefab
- [ ] SystemMessages has liquidNeededMessage assigned
- [ ] SystemMessages has craftSuccessMessage assigned
- [ ] CoffeeMaker component added to scene
- [ ] All CoffeeMaker references assigned
- [ ] Clickable button has CoffeeMaker reference
- [ ] Enter Play mode - tracker clears
- [ ] Add liquid - liquid amount increases
- [ ] Add capsule(s) - machine contents update
- [ ] Add additive (optional) - machine contents update
- [ ] Click make coffee WITHOUT liquid - shows "liquid needed"
- [ ] Add liquid, click make coffee - spawns prefab
- [ ] Liquid amount decreases by 1
- [ ] Capsules and additive clear from machine
- [ ] Success dialogue appears
- [ ] Can make multiple coffees if multiple liquids added

## Debug Logging

Enable "Show Debug Logs" in CoffeeMaker to see:
- When MakeCoffee is called
- Current liquid amount
- Machine contents
- Recipe matching process
- Crafting results
- Spawn locations

Example output:
```
[CoffeeMaker] MakeCoffee called
[CoffeeMaker] Current liquid amount: 2
[CoffeeMaker] Machine contents: Water Bottle, Black Capsule, , Salt
[CoffeeMaker] Crafting: Salted Black Coffee
[CoffeeMaker] Spawned Salted Black Coffee at (0, 1, 0)
[CoffeeMaker] Crafting complete! Remaining liquid: 1
```

## Error Handling

### No Liquid
Shows "liquid needed" dialogue, crafting cancelled.

### No Matching Recipe
Currently silent - crafting cancelled.
*Could add dialogue: "These ingredients don't make anything!"*

### Missing Prefab
Error logged, crafting fails gracefully.

### Missing References
Error logged on first craft attempt, component disabled.

## Advanced Usage

### Multiple Recipes with Same Ingredients

First matching recipe wins. Order recipes in CraftingManager by priority:
```
1. Most specific (all components required)
2. Less specific (some components optional)
3. Basic recipes (minimum components)
```

### Custom Spawn Behavior

Override spawn location per recipe by:
1. Adding spawn transform field to recipe
2. Modifying SpawnCraftedItem() to check recipe's spawn point first

### Recipe Variations

Create similar recipes with different amounts:
- "Small Coffee" (1 liquid, 1 capsule)
- "Large Coffee" (2 liquid, 1 capsule)
*(Would need to modify liquid check from >= 1 to >= required amount)*

### Prefab Customization

The spawned GameObject can have:
- ClickableObject (to pick up crafted coffee)
- Custom scripts (coffee effects, stats, etc.)
- Particle effects
- Audio sources

## Files Created

- `Assets/Scripts/CraftingManager.cs` - Recipe management SO
- `Assets/Scripts/CoffeeMaker.cs` - Crafting logic component

## Files Modified

- `Assets/Scripts/Interactions/PlayerActionTracker.cs` - Added liquid amount and auto-clear
- `Assets/Scripts/SystemMessages.cs` - Added new dialogue nodes
- `Assets/Scripts/ClickableObject.cs` - Added coffee maker integration
- `Assets/Scripts/Interactions/ItemInteractionHandler.cs` - Tracks liquid amount

## Future Enhancements

Potential improvements:
1. Recipe discovery system (unlock recipes as you experiment)
2. Recipe book UI showing known recipes
3. Quality system (perfect/good/bad coffee based on timing)
4. Temperature tracking (hot/cold beverages)
5. Ingredient quantities (2 sugars vs 1 sugar)
6. Brewing time/animation
7. Failed recipe items (burnt coffee, etc.)
8. Recipe hints when hovering over "Make Coffee"
9. Visual feedback during crafting process
10. Coffee rating/scoring system

---

## Quick Start Summary

1. Create **CraftingManager** asset
2. Add recipes with prefabs
3. Update **SystemMessages** with new dialogues
4. Add **CoffeeMaker** component to machine
5. Assign all references
6. Link **CoffeeMaker** to clickable button
7. Play and test!

**That's it! The coffee crafting system is ready to use!**

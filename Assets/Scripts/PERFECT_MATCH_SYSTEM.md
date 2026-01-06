# Perfect Match Recipe System

## ?? Overview

The crafting system has been updated to require **PERFECT MATCHES** only. No partial matches are allowed. Every component must match exactly.

## ?? Matching Rules

### Rule 1: Empty = None
- **Empty in recipe** = "This component is not required"
- **Empty in machine** = "This component is not provided"
- Both must align perfectly

### Rule 2: All Components Must Match
If a component is specified in the recipe, it must be present in the machine (and match using `.Contains()`).
If a component is NOT specified in the recipe, it must NOT be present in the machine.

### Rule 3: Capsule Count Must Match
- Recipe requires 0 capsules ? Machine must have 0 capsules
- Recipe requires 1 capsule ? Machine must have exactly 1 capsule
- Recipe requires 2 capsules ? Machine must have exactly 2 capsules

### Rule 4: No Extra Components Allowed
If the machine has components that the recipe doesn't require, it's NOT a match.

## ?? Examples

### ? Example 1: Perfect Match
**Recipe:**
- Liquid: "Water"
- Coffee 1: "Black"
- Coffee 2: (empty)
- Extra: (empty)

**Machine:**
- Liquid: "Water Bottle" ? Contains "Water"
- Capsule A: "Black Capsule" ? Contains "Black"
- Capsule B: (empty) ? Matches empty
- Additive: (empty) ? Matches empty

**Result:** ? **PERFECT MATCH** - Creates item!

---

### ? Example 2: Extra Component (NO MATCH)
**Recipe:**
- Liquid: "Water"
- Coffee 1: "Black"
- Coffee 2: (empty)
- Extra: (empty) ? Wants NO extra

**Machine:**
- Liquid: "Water Bottle" ?
- Capsule A: "Black Capsule" ?
- Capsule B: (empty) ?
- Additive: "Salt" ? Recipe wants none, but machine has salt

**Result:** ? **NO MATCH** - Recipe needs NO extra but machine has 'Salt'

---

### ? Example 3: Missing Component (NO MATCH)
**Recipe:**
- Liquid: "Water"
- Coffee 1: "Black"
- Coffee 2: "Red" ? Requires 2 capsules
- Extra: (empty)

**Machine:**
- Liquid: "Water Bottle" ?
- Capsule A: "Black Capsule" ?
- Capsule B: (empty) ? Missing second capsule
- Additive: (empty) ?

**Result:** ? **NO MATCH** - Recipe needs 2 capsule(s) but machine has 1

---

### ? Example 4: Double Capsule Perfect Match
**Recipe:**
- Liquid: "Water"
- Coffee 1: "Black"
- Coffee 2: "Red"
- Extra: "Salt"

**Machine:**
- Liquid: "Water Bottle" ? Contains "Water"
- Capsule A: "Black Capsule" ? Contains "Black"
- Capsule B: "Red Capsule" ? Contains "Red"
- Additive: "Salt Shaker" ? Contains "Salt"

**Result:** ? **PERFECT MATCH** - Creates item!

---

### ? Example 5: Too Many Capsules (NO MATCH)
**Recipe:**
- Liquid: "Water"
- Coffee 1: "Black"
- Coffee 2: (empty) ? Wants only 1 capsule
- Extra: (empty)

**Machine:**
- Liquid: "Water Bottle" ?
- Capsule A: "Black Capsule" ?
- Capsule B: "Red Capsule" ? Recipe doesn't want 2 capsules
- Additive: (empty) ?

**Result:** ? **NO MATCH** - Recipe needs 1 capsule(s) but machine has 2

---

### ? Example 6: Wrong Liquid (NO MATCH)
**Recipe:**
- Liquid: "Water"
- Coffee 1: "Black"
- Coffee 2: (empty)
- Extra: (empty)

**Machine:**
- Liquid: "Milk" ? Doesn't contain "Water"
- Capsule A: "Black Capsule" ?
- Capsule B: (empty) ?
- Additive: (empty) ?

**Result:** ? **NO MATCH** - Needs liquid 'Water' but machine has 'Milk'

---

### ? Example 7: No Liquid When Required (NO MATCH)
**Recipe:**
- Liquid: "Water" ? Requires liquid
- Coffee 1: "Black"
- Coffee 2: (empty)
- Extra: (empty)

**Machine:**
- Liquid: (empty) ? Missing required liquid
- Capsule A: "Black Capsule" ?
- Capsule B: (empty) ?
- Additive: (empty) ?

**Result:** ? **NO MATCH** - Needs liquid 'Water' but machine has none

---

### ? Example 8: Liquid When Not Required (NO MATCH)
**Recipe:**
- Liquid: (empty) ? Wants NO liquid
- Coffee 1: "Black"
- Coffee 2: (empty)
- Extra: (empty)

**Machine:**
- Liquid: "Water Bottle" ? Recipe wants none
- Capsule A: "Black Capsule" ?
- Capsule B: (empty) ?
- Additive: (empty) ?

**Result:** ? **NO MATCH** - Recipe needs NO liquid but machine has 'Water Bottle'

## ?? Debug Output

When a recipe doesn't match, you'll see detailed reasons:

```
??? RECIPE MATCHING ???
  Matching Recipe: NONE FOUND
  Available Recipes: 3

  Why no match? Checking all recipes:
  Recipe 1: Simple Black Coffee
    Requires: Liquid=Water, Coffee1=Black, Coffee2=None, Extra=None
    ? Recipe needs NO extra but machine has 'Salt'
  
  Recipe 2: Double Shot
    Requires: Liquid=Water, Coffee1=Black, Coffee2=Black, Extra=None
? Recipe needs 2 capsule(s) but machine has 1
  
  Recipe 3: Salted Coffee
    Requires: Liquid=Water, Coffee1=Black, Coffee2=None, Extra=Salt
    ? Recipe needs 1 capsule(s) but machine has 2

??? RESULT ???
  Status: ? FAILED
  Reason: No recipe matches current ingredients (PERFECT MATCH required)
  Note: All components must match exactly - no partial matches allowed
  Tip: Empty components in recipe = none required, Empty in machine = none provided
```

## ??? How to Create Recipes

### Recipe 1: Simple Coffee (Water + 1 Capsule)
```
Recipe Name: Simple Black Coffee
Required Liquid: Water
Required Coffee 1: Black
Required Coffee 2: (leave empty)
Required Extra: (leave empty)
Result Prefab: SimpleBlackCoffee_Prefab
```

**Will match:**
- Water + Black capsule in A slot + nothing else
- Water + Black capsule in B slot + nothing else

**Will NOT match:**
- Water + Black + Red (too many capsules)
- Water + Black + Salt (extra not required)
- Water only (missing capsule)

### Recipe 2: Double Shot (Water + 2 Same Capsules)
```
Recipe Name: Double Shot Black
Required Liquid: Water
Required Coffee 1: Black
Required Coffee 2: Black
Required Extra: (leave empty)
Result Prefab: DoubleShot_Prefab
```

**Will match:**
- Water + Black in A + Black in B + nothing else

**Will NOT match:**
- Water + Black only (needs 2 capsules)
- Water + Black + Red (needs both to be Black)
- Water + Black + Black + Salt (extra not required)

### Recipe 3: Mixed Coffee (Water + 2 Different Capsules)
```
Recipe Name: Red & Black Mix
Required Liquid: Water
Required Coffee 1: Black
Required Coffee 2: Red
Required Extra: (leave empty)
Result Prefab: RedBlackMix_Prefab
```

**Will match:**
- Water + Black in A + Red in B
- Water + Red in A + Black in B (order doesn't matter)

**Will NOT match:**
- Water + Black + Black (needs Red)
- Water + Black + Blue (needs Red not Blue)

### Recipe 4: Flavored Coffee (Water + Capsule + Extra)
```
Recipe Name: Salted Black Coffee
Required Liquid: Water
Required Coffee 1: Black
Required Coffee 2: (leave empty)
Required Extra: Salt
Result Prefab: SaltedCoffee_Prefab
```

**Will match:**
- Water + Black capsule + Salt + nothing else

**Will NOT match:**
- Water + Black only (missing salt)
- Water + Black + Sugar (needs Salt not Sugar)
- Water + Black + Red + Salt (too many capsules)

### Recipe 5: No Liquid Recipe (Dry Mix)
```
Recipe Name: Dry Coffee Powder
Required Liquid: (leave empty)
Required Coffee 1: Black
Required Coffee 2: Red
Required Extra: (leave empty)
Result Prefab: CoffeePowder_Prefab
```

**Will match:**
- Black + Red capsules only, NO water, NO extra

**Will NOT match:**
- Water + Black + Red (liquid not required)
- Black + Red + Salt (extra not required)

## ?? Testing Scenarios

### Test 1: Basic Recipe
1. Create recipe: Water + Black
2. Add water to machine
3. Add black capsule
4. Click Make Coffee
5. **Expected:** ? SUCCESS

### Test 2: Extra Component Rejection
1. Create recipe: Water + Black (no extra)
2. Add water to machine
3. Add black capsule
4. Add salt
5. Click Make Coffee
6. **Expected:** ? FAILED - "Recipe needs NO extra but machine has 'Salt'"

### Test 3: Missing Component
1. Create recipe: Water + Black + Red
2. Add water to machine
3. Add black capsule only
4. Click Make Coffee
5. **Expected:** ? FAILED - "Recipe needs 2 capsule(s) but machine has 1"

### Test 4: Wrong Component
1. Create recipe: Water + Black + Salt
2. Add water to machine
3. Add black capsule
4. Add sugar (not salt)
5. Click Make Coffee
6. **Expected:** ? FAILED - "Needs extra 'Salt' but machine has 'Sugar'"

### Test 5: Multiple Recipe Priority
1. Create Recipe A: Water + Black (simple)
2. Create Recipe B: Water + Black + Salt (complex)
3. Add water + black to machine
4. Click Make Coffee
5. **Expected:** ? Creates Recipe A (matches first)
6. Now add salt
7. Click Make Coffee
8. **Expected:** ? Creates Recipe B (now matches this one instead)

## ?? Technical Details

### String Matching
Uses `.Contains()` for flexibility:
- Item: "Black Coffee Capsule" matches identifier: "Black" ?
- Item: "BlackCapsule_01" matches identifier: "Black" ?
- Item: "Capsule_Black_Premium" matches identifier: "Black" ?

### Capsule Order Independence
Recipe requires Black + Red, machine can have:
- Black in A, Red in B ?
- Red in A, Black in B ?

Both are valid! The system doesn't care about slot order.

### Empty vs Null
Both treated the same:
- `""` (empty string) = None
- `null` = None

## ?? Common Mistakes

### Mistake 1: Leaving Components in Machine
```
? WRONG: Add water, black, make coffee ?, forget to clear
  Then add red, make coffee ? (still has black + red = 2)

? CORRECT: MakeCoffee() automatically clears capsules and extra
           So this shouldn't happen if using the system correctly
```

### Mistake 2: Assuming Partial Matches Work
```
? WRONG: Recipe wants Black + Red
        Think: "I have Black, that's partial match!"
          
? CORRECT: Must have BOTH Black AND Red, nothing more, nothing less
```

### Mistake 3: Not Accounting for Extras
```
? WRONG: Create recipe for Water + Black
       Player adds Water + Black + Salt
          Expect it to work (ignoring salt)
       
? CORRECT: Recipe must explicitly allow Salt
           OR player must not add Salt
```

## ?? Recipe Complexity Levels

### Level 1: Simple (1-2 components)
- Water + Black
- Black only
- Water only (if that's even useful)

### Level 2: Standard (2-3 components)
- Water + Black + Red
- Water + Black + Salt
- Water + Red + Sugar

### Level 3: Complex (4 components)
- Water + Black + Red + Salt
- Water + Blue + Black + Sugar
- Water + Red + Red + Pepper

### Level 4: Exact Requirements
- No Water + Black + Red (dry mix)
- Black only (single capsule item)
- Salt only (pure ingredient)

## ?? Best Practices

1. **Start Simple:** Create basic recipes first (Water + 1 capsule)
2. **Test Each Recipe:** Verify it matches correctly
3. **Add Variations:** Create recipes with different component counts
4. **Document Recipes:** Keep a list of what each recipe requires
5. **Name Clearly:** Recipe names should indicate ingredients
6. **Order Matters:** Put simpler recipes first in list (they match faster)

## ?? Summary

**Key Changes:**
- ? PERFECT match required
- ? NO partial matches
- ? Empty = None (must align)
- ? Capsule count must match exactly
- ? Extra components prevent match
- ? Detailed debug output shows why recipes don't match

**Result:**
Players must provide EXACTLY what the recipe requires - no more, no less!

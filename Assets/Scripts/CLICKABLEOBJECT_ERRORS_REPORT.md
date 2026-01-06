# ClickableObject.cs - Compilation Errors Report

## ?? Critical Errors Found

### Error 1: Invalid Method Name (Line 751)
```csharp
? WRONG:
public void ForceReturn toOriginal()

? CORRECT:
public void ForceReturnToOriginal()
```
**Issue:** Space in method name breaks syntax

---

### Error 2: Duplicate Field Declarations

**Old Float System Fields (REMOVE THESE):**
```csharp
? REMOVE:
[Header("Float Behavior")]
public bool floatOnClick = false;
public float floatDistance = 2f;
public float floatSpeed = 5f;
public Vector3 floatOffset = Vector3.zero;
private bool isFloating = false;
private Quaternion floatingRotationOffset;
```

**New Pickup System Fields (KEEP THESE):**
```csharp
? KEEP:
[Header("Pick Up Behavior")]
public bool canPickUp = false;
public float holdDistance = 2f;
public float pickupSpeed = 5f;
public Vector3 holdOffset = Vector3.zero;
private bool isBeingHeld = false;
private bool isHoldingMouseButton = false;
private Quaternion holdingRotationOffset;
```

---

### Error 3: Duplicate Tooltip Attributes

**Lines with multiple tooltips on same field:**
```csharp
? WRONG:
[Tooltip("Speed at which object rotates while floating (degrees per second)")]
[Tooltip("Speed at which object rotates while being held (degrees per second)")]
public float rotationSpeed = 30f;

? CORRECT:
[Tooltip("Speed at which object rotates while being held (degrees per second)")]
public float rotationSpeed = 30f;
```

---

### Error 4: Conflicting Update() Method

**Current (BROKEN):**
```csharp
void Update()
{
    // Check for right-click to return floating object
    if (floatOnClick && isFloating && Input.GetMouseButtonDown(1))
    // Handle pick up behavior with hold-to-lift mechanic
    if (canPickUp)
    {
        StopFloating();  // ? Method doesn't exist!
        // Check if mouse button is being released...
```

**Should be:**
```csharp
void Update()
{
    // Handle pick up behavior with hold-to-lift mechanic
    if (canPickUp)
    {
  // Check if mouse button is being released while holding this object
        if (isBeingHeld && !Input.GetMouseButton(0))
        {
 ReleaseObject();
   
  if (showDebugInfo)
{
       Debug.Log($"ClickableObject: {gameObject.name} - Left mouse released, dropping object");
         }
            return;
  }

// Update object position while being held
        if (isBeingHeld && mainCamera != null)
     {
    Vector3 targetPosition = mainCamera.transform.position +
                mainCamera.transform.forward * holdDistance +
        mainCamera.transform.TransformDirection(holdOffset);

         transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * pickupSpeed);

          Quaternion worldYRotation = Quaternion.Euler(0f, rotationSpeed * Time.time, 0f);
 Quaternion targetRotation = worldYRotation * holdingRotationOffset;
     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * pickupSpeed);
      }
    }
}
```

---

### Error 5: Undefined Method Calls

**Methods being called that don't exist:**
- `StopFloating()` ?
- `StartFloating()` ?
- `ToggleFloat()` ?

**Should call instead:**
- `ReleaseObject()` ?
- `StartHoldingObject()` ?
- `TogglePickUp()` ?

---

### Error 6: Duplicate Variable Declarations in ReturnToOriginalPosition()

```csharp
? WRONG:
float returnSpeed = floatSpeed;
float rotationReturnSpeed = floatSpeed * 2f;
float returnSpeed = pickupSpeed;  // ? Duplicate!
float rotationReturnSpeed = pickupSpeed * 2f;  // ? Duplicate!

? CORRECT:
float returnSpeed = pickupSpeed;
float rotationReturnSpeed = pickupSpeed * 2f;
```

---

### Error 7: Conflicting OnClicked() Logic

```csharp
? WRONG:
// Handle float behavior
if (floatOnClick)
// Handle pick-up behavior
if (canPickUp)
{
    ToggleFloat();  // ? Method doesn't exist!
    TogglePickUp();
}

? CORRECT:
// Handle pick-up behavior - start holding when clicked
if (canPickUp && !isBeingHeld)
{
    StartHoldingObject();
}
```

---

### Error 8: Duplicate Start() Camera Check

```csharp
? WRONG:
if (mainCamera == null && floatOnClick)
if (mainCamera == null && canPickUp)
{
    Debug.LogWarning($"...: floatOnClick is enabled...");
    Debug.LogWarning($"...: canPickUp is enabled...");
}

? CORRECT:
if (mainCamera == null && canPickUp)
{
    Debug.LogWarning($"ClickableObject on {gameObject.name}: canPickUp is enabled but no Main Camera found!");
}
```

---

### Error 9: Conflicting Method Definitions

**TogglePickUp() has mixed old/new code:**
```csharp
? WRONG:
private void TogglePickUp()
{
    // ...
    if (!isFloating)  // ? Wrong variable!
  if (!isBeingHeld)
    {
        // ...
  StartFloating();  // ? Wrong method!
        StartHoldingObject();
    }
    else
    {
        StopFloating();  // ? Wrong method!
        ReleaseObject();
    }
}

? CORRECT:
private void TogglePickUp()
{
    if (mainCamera == null)
    {
  Debug.LogWarning($"ClickableObject on {gameObject.name}: Cannot pick up - no Main Camera found!");
     return;
    }

    if (!isBeingHeld)
    {
   StartHoldingObject();
        
        if (actionTracker != null)
        {
          actionTracker.HeldItemName = gameObject.name;
        }
    }
    else
    {
    ReleaseObject();
    
        if (actionTracker != null && actionTracker.HeldItemName == gameObject.name)
        {
            actionTracker.HeldItemName = "";
        }
    }
}
```

---

### Error 10: Duplicate/Conflicting ReleaseObject()

Has duplicate debug logs and variable assignments:
```csharp
? WRONG:
private void ReleaseObject()
{
    isFloating = false;  // ? Wrong variable!
    isBeingHeld = false;
    // ...
    Debug.Log($"...returning to original position...");
    Debug.Log($"...released, returning to original position...");  // ? Duplicate!
}

? CORRECT:
private void ReleaseObject()
{
isBeingHeld = false;
    isHoldingMouseButton = false;

    if (currentlyHeldObject == this)
    {
        currentlyHeldObject = null;

     if (actionTracker != null && actionTracker.HeldItemName == gameObject.name)
        {
            actionTracker.HeldItemName = "";
  }
    }

    StartCoroutine(ReturnToOriginalPosition());

    if (showDebugInfo)
    {
        Debug.Log($"ClickableObject: {gameObject.name} released, returning to original position. Currently held: {HeldItemName}");
}
}
```

---

### Error 11: Duplicate StartHoldingObject()

Method sets both old and new variables:
```csharp
? WRONG:
private void StartHoldingObject()
{
    // ...
    floatingRotationOffset = Quaternion.Euler(rotationOffset);  // ? Old system!
    holdingRotationOffset = Quaternion.Euler(rotationOffset);
    
    isFloating = true;// ? Old system!
 isBeingHeld = true;
    // ...
}

? CORRECT:
private void StartHoldingObject()
{
    if (mainCamera == null)
    {
        Debug.LogWarning($"ClickableObject on {gameObject.name}: Cannot pick up - no Main Camera found!");
        return;
    }

    if (currentlyHeldObject != null && currentlyHeldObject != this)
    {
        if (showDebugInfo)
     {
   Debug.Log($"ClickableObject: Cannot pick up {gameObject.name} - {currentlyHeldObject.gameObject.name} is already being held!");
    }
   return;
    }

    pickupPosition = transform.position;
    pickupRotation = transform.rotation;
    originalParent = transform.parent;
    holdingRotationOffset = Quaternion.Euler(rotationOffset);
    
    isBeingHeld = true;
    isHoldingMouseButton = true;
    currentlyHeldObject = this;

    if (objectCollider != null)
    {
   objectCollider.enabled = false;
    }

    if (actionTracker != null)
    {
        actionTracker.HeldItemName = gameObject.name;
    }

    if (showDebugInfo)
  {
        Debug.Log($"ClickableObject: {gameObject.name} picked up from position {pickupPosition}. Currently held: {HeldItemName}");
    }
}
```

---

## ?? Complete Fix Checklist

### Step 1: Remove Old Float System
- [ ] Remove `[Header("Float Behavior")]`
- [ ] Remove `public bool floatOnClick`
- [ ] Remove `public float floatDistance`
- [ ] Remove `public float floatSpeed`
- [ ] Remove `public Vector3 floatOffset`
- [ ] Remove `private bool isFloating`
- [ ] Remove `private Quaternion floatingRotationOffset`

### Step 2: Clean Up Tooltips
- [ ] Remove duplicate tooltip for `rotationSpeed`
- [ ] Remove duplicate tooltip for `rotationOffset`
- [ ] Keep only the "held object" versions

### Step 3: Fix Update() Method
- [ ] Remove all `floatOnClick` references
- [ ] Remove all `isFloating` references
- [ ] Remove `StopFloating()` call
- [ ] Keep only `canPickUp` logic with `ReleaseObject()`

### Step 4: Fix OnClicked() Method
- [ ] Remove `if (floatOnClick)` block
- [ ] Remove `ToggleFloat()` call
- [ ] Remove duplicate `if (canPickUp && !isBeingHeld)` block
- [ ] Keep only one call to `StartHoldingObject()`

### Step 5: Remove Old Methods
- [ ] Delete `ToggleFloat()` method completely
- [ ] Delete `StartFloating()` method completely
- [ ] Delete `StopFloating()` method completely

### Step 6: Clean TogglePickUp()
- [ ] Remove `isFloating` check
- [ ] Remove `StartFloating()` call
- [ ] Remove `StopFloating()` call
- [ ] Keep only `isBeingHeld`, `StartHoldingObject()`, `ReleaseObject()`

### Step 7: Clean StartHoldingObject()
- [ ] Remove `floatingRotationOffset` assignment
- [ ] Remove `isFloating = true`
- [ ] Remove duplicate debug log
- [ ] Keep only new pickup system variables

### Step 8: Clean ReleaseObject()
- [ ] Remove `isFloating = false`
- [ ] Remove duplicate debug log
- [ ] Keep only `isBeingHeld` and `isHoldingMouseButton`

### Step 9: Clean ReturnToOriginalPosition()
- [ ] Remove duplicate `float returnSpeed` declaration
- [ ] Remove duplicate `float rotationReturnSpeed` declaration
- [ ] Use `pickupSpeed` not `floatSpeed`

### Step 10: Fix ForceReturnToOriginal()
- [ ] Fix method name: `ForceReturnToOriginal()` (remove space)
- [ ] Remove `if (isFloating)` check
- [ ] Remove `StopFloating()` call
- [ ] Keep only `if (isBeingHeld)` with `ReleaseObject()`

### Step 11: Clean Start() Method
- [ ] Remove `if (mainCamera == null && floatOnClick)` check
- [ ] Remove "floatOnClick is enabled" warning
- [ ] Keep only `canPickUp` check

---

## ?? Quick Fix Script

Here's a search-and-replace guide:

### Replace These Lines:
```
floatOnClick ? (DELETE)
floatDistance ? (DELETE)
floatSpeed ? (DELETE - replace with pickupSpeed where used)
floatOffset ? (DELETE - replace with holdOffset where used)
isFloating ? isBeingHeld
floatingRotationOffset ? holdingRotationOffset
StartFloating() ? StartHoldingObject()
StopFloating() ? ReleaseObject()
ToggleFloat() ? TogglePickUp() (but simplify logic)
ForceReturn toOriginal() ? ForceReturnToOriginal()
```

---

## ? Expected Final State

After all fixes:

**Inspector Fields:**
```csharp
[Header("Pick Up Behavior")]
public bool canPickUp = false;
public float holdDistance = 2f;
public float pickupSpeed = 5f;
public float rotationSpeed = 30f;
public Vector3 rotationOffset = Vector3.zero;
public Vector3 holdOffset = Vector3.zero;
```

**Private Variables:**
```csharp
private bool isBeingHeld = false;
private bool isHoldingMouseButton = false;
private Quaternion holdingRotationOffset;
```

**Methods:**
```csharp
void Update() // Monitors mouse release
void OnClicked() // Starts pickup
void TogglePickUp() // Optional toggle logic
void StartHoldingObject() // Begin holding
void ReleaseObject() // Drop object
IEnumerator ReturnToOriginalPosition() // Smooth return
void ForceReturnToOriginal() // Public force release
```

**No Float System References Anywhere!**

---

## ?? After Fixing

The file should compile with:
- ? Zero errors
- ? Zero warnings
- ? Only pickup system (no float system)
- ? Clean, consistent code
- ? Hold-to-lift mechanic working

---

## Manual Edit Required

Due to the number of intertwined issues, **manual editing is recommended**:

1. Open `Assets\Scripts\ClickableObject.cs` in your code editor
2. Use Find & Replace for the patterns above
3. Follow the checklist step by step
4. Test compilation after each major step
5. Verify in Unity Inspector that old fields are gone

Alternatively, I can provide a completely rewritten clean version of the entire file if needed.

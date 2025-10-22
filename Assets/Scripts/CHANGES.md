# ClickableObject Update - Rotation Offset & Pickup Position Memory

## Changes Made

### 1. Rotation Offset Parameter
Added a new `rotationOffset` parameter in the Float Behavior section:
- **Type**: Vector3 (Euler angles)
- **Default**: (0, 0, 0)
- **Purpose**: Applies a base rotation to the floating object (e.g., tilt it 45° on X-axis)

**Usage Examples:**
- `(45, 0, 0)` - Tilts object forward 45 degrees
- `(0, 90, 0)` - Rotates object 90 degrees around Y-axis
- `(30, 45, 0)` - Tilts forward 30° and rotates 45°

### 2. Pickup Position Memory
The object now remembers its position **when it was picked up**, not where it spawned.

**Previous Behavior:**
- Object always returned to spawn position

**New Behavior:**
- Object returns to the position it was at when you clicked it

**Benefits:**
- If object moved (physics, player moved it, etc.), it returns to that moved position
- More realistic for physics-based games
- Better for moveable objects

### 3. Technical Changes

**New Variables:**
- `pickupPosition` - Position when object was clicked
- `pickupRotation` - Rotation when object was clicked
- `floatingRotationOffset` - Calculated quaternion from rotationOffset parameter
- Still keeps `originalPosition`/`originalRotation` for reference

**Updated Methods:**
- `StartFloating()` - Now captures pickup position/rotation and calculates rotation offset
- `Update()` - Uses quaternion-based rotation with offset applied, **rotates on world Y axis**, checks for **right-click to return object**
- `ReturnToOriginalPosition()` - Returns to pickup position instead of spawn position

**Rotation Behavior:**
- Rotation always occurs on the **world Y axis** (vertical axis)
- This ensures objects spin upright regardless of camera orientation
- Rotation offset is applied on top of the world Y rotation

**Input Handling:**
- **Left-click on object** - Toggles float state (pick up / put down)
- **Right-click (anywhere)** - Returns floating object to pickup position immediately

## How It Works

1. **First Click:**
   - Saves current position/rotation as "pickup point"
   - Applies rotation offset to floating state
   - Object floats to camera with rotation offset + continuous spin

2. **Second Click (Left-Click) or Right-Click:**
   - **Left-click on object**: Smoothly returns to pickup position/rotation (toggle behavior)
   - **Right-click anywhere**: Instantly returns object to pickup position
   - Restores exact transform from when it was picked up

## Inspector Settings

```
Float Behavior Section:
??? Float On Click: ?
??? Float Distance: 2.0
??? Float Speed: 5.0
??? Rotation Speed: 30.0
??? Rotation Offset: (30, 0, 0)  ? NEW PARAMETER
??? Float Offset: (0, 0, 0)
```

## Example Use Cases

### Tilted Item Inspection
```
Rotation Offset: (45, 0, 0)
Rotation Speed: 20
```
Object floats with a 45° forward tilt and slowly rotates

### Sideways Book Examination
```
Rotation Offset: (0, 0, 90)
Rotation Speed: 15
```
Book floats standing upright and rotates slowly

### Dynamic Object Pickup
```
Rotation Offset: (0, 0, 0)
Float Distance: 1.5
```
Object picked up from wherever it currently is (not spawn point) and floats naturally

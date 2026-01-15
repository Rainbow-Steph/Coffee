# Delivery System - Quick Setup Guide ??

## ? **System Ready!**

Three new scripts created for coffee delivery system!

---

## Quick Setup (5 Minutes)

### Step 1: Create Configuration Asset
```
Right-click in Project
? Create
? Coffee
? Deliverable Manager
Name: "DeliverableManager"
```

---

### Step 2: Configure Targets

**In DeliverableManager Inspector:**

#### **Add 3 Targets:**

**1. Black Target:**
```
Target Name: "Black Target"
High Satisfaction: [Drag coffee prefabs]
Medium Satisfaction: [Drag coffee prefabs]
Low Satisfaction: [Drag coffee prefabs]
```

**2. Red Target:**
```
Target Name: "Red Target"
High Satisfaction: [Drag coffee prefabs]
Medium Satisfaction: [Drag coffee prefabs]
Low Satisfaction: [Drag coffee prefabs]
```

**3. Blue Target:**
```
Target Name: "Blue Target"
High Satisfaction: [Drag coffee prefabs]
Medium Satisfaction: [Drag coffee prefabs]
Low Satisfaction: [Drag coffee prefabs]
```

---

### Step 3: Configure Rewards

**For Each Satisfaction Level:**

```
High Satisfaction Rewards:
?? Rock Prefab: [Drag prefab]
?? Coin Prefab: [Drag prefab]
?? Bill Prefab: [Drag prefab]
?? Rocks Range: Min 2, Max 5
?? Coins Range: Min 3, Max 7
?? Bills Range: Min 1, Max 3

Medium Satisfaction Rewards:
?? Rocks Range: Min 1, Max 3
?? Coins Range: Min 2, Max 5
?? Bills Range: Min 0, Max 1

Low Satisfaction Rewards:
?? Rocks Range: Min 0, Max 2
?? Coins Range: Min 1, Max 3
?? Bills Range: Min 0, Max 0
```

---

### Step 4: Create Delivery Targets in Scene

**For Each Target (Black/Red/Blue):**

```
1. Create Empty GameObject
   Name: "Black Target"

2. Add DeliveryTarget component

3. Create Child GameObject:
   Name: "Delivery Collider"
   Add: Box Collider
   Set: Is Trigger ?
   Size: Make big enough for coffee

4. Configure DeliveryTarget:
   ?? Target Name: "Black Target"
   ?? Deliverable Manager: [Drag asset]
   ?? Delivery Collider: [Auto-found]
   ?? Reward Spawn Point: [Self or set custom]
```

---

### Step 5: Mark Coffee as Deliverable

**On Coffee Prefabs:**

```
ClickableObject Component:
?? Item Type: Delivery ? Change this!
```

---

## Test

1. **Start Game**
2. **Craft Coffee** (sets ItemType.Delivery)
3. **Pick Up Coffee**
4. **Bring to Target**
5. **Drop in Delivery Collider**
6. **See Rewards Spawn!** ??

---

## Visual Guide

### Target Setup:
```
Black Target (GameObject)
?? DeliveryTarget (Component)
?  ?? Target Name: "Black Target"
?  ?? Deliverable Manager: [Asset]
?? Delivery Collider (Child)
   ?? Box Collider (Is Trigger ?)
```

### Satisfaction Flow:
```
High List Items   ? High Rewards   ??????
Medium List Items ? Medium Rewards ????
Low List Items    ? Low Rewards    ??
Not in Lists      ? Failed ?
```

---

## Quick Tips

? **Target Names Must Match** - DeliverableManager and scene  
? **Delivery Collider** - Must be child named exactly  
? **ItemType.Delivery** - Required for coffee prefabs  
? **Lists** - Add prefabs to satisfaction lists  

---

**Ready to deliver!** ????

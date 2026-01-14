# Rich Text Quick Reference ??

## ? Implementation Complete!

Rich text is now enabled in your dialogue system!

---

## Basic Syntax

### Colors
```
<color=red>Red Text</color>
<color=#FF0000>Red Text (Hex)</color>
```

### Formatting
```
<b>Bold</b>
<i>Italic</i>
<u>Underline</u>
<s>Strikethrough</s>
```

### Size
```
<size=20>Sized Text</size>
```

---

## Common Colors

| Name | Example | Hex |
|------|---------|-----|
| Red | `<color=red>text</color>` | `#FF0000` |
| Blue | `<color=blue>text</color>` | `#0000FF` |
| Green | `<color=green>text</color>` | `#00FF00` |
| Yellow | `<color=yellow>text</color>` | `#FFFF00` |
| Orange | `<color=orange>text</color>` | `#FFA500` |
| Purple | `<color=purple>text</color>` | `#800080` |
| Pink | `<color=pink>text</color>` | `#FFC0CB` |
| Brown | `<color=brown>text</color>` | `#A52A2A` |
| White | `<color=white>text</color>` | `#FFFFFF` |
| Black | `<color=black>text</color>` | `#000000` |
| Gray | `<color=gray>text</color>` | `#808080` |
| Cyan | `<color=cyan>text</color>` | `#00FFFF` |
| Magenta | `<color=magenta>text</color>` | `#FF00FF` |

---

## Game-Specific Color Coding

### Money
```
<color=green>$10</color>
```

### Items
```
<color=blue>Water</color>
<color=brown>Coffee</color>
<color=yellow>Sugar</color>
```

### Status
```
<color=green>? Success</color>
<color=red>? Error</color>
```

---

## Example Templates

### Purchase Message:
```
You bought <color=blue>water</color> for <color=green>$10</color>.
```

### Error Message:
```
<color=red><b>ERROR:</b></color> Not enough money!
```

### Success Message:
```
<color=green>? Success!</color> You crafted <color=brown><b>Espresso</b></color>!
```

### Instructions:
```
<b>Step 1:</b> Click the <color=blue>water dispenser</color>
<b>Step 2:</b> Insert <color=brown>coffee capsule</color>
```

---

## Testing

1. Create DialogueNodeSO
2. Add rich text tags
3. Play game
4. Pick up object (dialogue shows)
5. Release object (dialogue clears) ?

---

## Features

? **Rich Text Support** - Colors, bold, italic, etc.  
? **Auto-Clear on Release** - Dialogue clears when dropping objects  
? **All TMP Tags Supported** - Full TextMeshPro compatibility  
? **Character-by-Character Typing** - Smooth animation preserved  

---

**Ready to use!** ???

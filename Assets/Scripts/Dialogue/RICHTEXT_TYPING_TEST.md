# Rich Text Typing Test Guide ?

## ? **System Ready!**

Your DialogueManager now properly handles rich text tags and unicode during typing animation!

---

## Quick Tests

### Test 1: Simple Unicode
```
Text: "? Success! ???"

Expected Result:
? types out (with delay)
Success! types out
??? types out
All symbols visible
```

---

### Test 2: Colored Text
```
Text: "<color=red>Hello World</color>"

Expected Result:
- NO visible "<color=red>" during typing
- "Hello World" types in RED
- Smooth character-by-character animation
```

---

### Test 3: Multiple Colors
```
Text: "Buy <color=blue>water</color> for <color=green>$10</color>"

Expected Result:
- "Buy " types normally
- "water" types in BLUE
- " for " types normally
- "$10" types in GREEN
- No visible tags
```

---

### Test 4: Bold + Unicode
```
Text: "<b>? Warning:</b> Error!"

Expected Result:
- "? Warning:" types in BOLD
- " Error!" types normally
- Unicode symbol shows correctly
```

---

## How to Test

1. **Create DialogueNodeSO:**
   - Right-click ? Create ? Dialogue ? Dialogue Node
   - Name it "TestRichText"

2. **Add Test Text:**
   ```
   <color=green>? Success!</color>

   You earned <color=yellow>???</color>
   ```

3. **Attach to ClickableObject:**
   - Add DialogueTrigger component
   - Assign TestRichText dialogue

4. **Play and Click:**
   - Watch text type out
   - Colors should work
   - Symbols should show
   - Tags should be invisible

---

## Common Unicode Symbols

### Ready to Use:
```
? ? ? ? ? ? ? ?
$ € £ ¥
? ? ? ?
? ? ? ?
? ? ?
```

### Usage in Dialogue:
```
<color=green>? Success!</color>
<color=red>? Failed!</color>
<color=yellow>? ? ?</color>
Buy <color=blue>water</color> ? <color=green>$10</color>
```

---

## Verification Checklist

During typing animation:
- [ ] Rich text tags are NOT visible
- [ ] Colors apply immediately
- [ ] Only text characters type out
- [ ] Unicode symbols display correctly
- [ ] Animation is smooth
- [ ] No lag or stuttering

---

## If Something Doesn't Work

### Tags Visible During Typing?
- Check `enableRichText` is **checked** in DialogueManager
- Verify TMP_Text has "Rich Text" enabled

### Unicode Not Showing?
- Font must include unicode characters
- Check TMP font asset settings
- Generate font atlas with unicode ranges

### Colors Not Working?
- Use correct syntax: `<color=red>text</color>`
- Not `<color:red>` or other variations
- Check TMP_Text has Rich Text enabled

---

**Test with the examples above - everything should work perfectly!** ?

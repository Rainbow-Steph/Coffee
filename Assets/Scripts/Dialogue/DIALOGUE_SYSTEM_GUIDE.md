# Dialogue System Setup Guide

## Overview
This dialogue system provides a flexible way to display text-based dialogue in your UI with the following features:
- Text appears letter by letter
- Click to advance/skip
- Optional typing sounds
- Optional voice clips
- Cancellable dialogues
- Hide UI when complete
- Reusable dialogue nodes

## Setup Instructions

### 1. Create the Dialogue UI
```
1. In your Canvas, create a new Panel (this will be your dialogue panel)
2. Inside the Panel, add a TextMeshPro - Text (UI) component
3. (Optional) Add an Image component for the "continue" icon
```

### 2. Setup DialogueManager
```
1. Create an empty GameObject in your scene
2. Name it "DialogueManager"
3. Add the DialogueManager component
4. Assign references in the inspector:
   - Dialogue Text: Your TextMeshPro component
   - Dialogue Panel: Your Panel object
   - Continue Icon: Your "continue" icon (optional)
   - Audio Source: For typing/voice sounds (optional)
```

### 3. Create Dialogue Nodes
```
1. In Project window: Right-click ? Create ? Dialogue ? Dialogue Node
2. In the inspector:
   - Add dialogue lines in the array
   - Set Can Be Cancelled
   - Adjust Text Speed
   - Add voice clips (optional)
```

### 4. Setup Clickable Objects
```
1. Select your ClickableObject GameObject
2. Add the DialogueTrigger component
3. Assign your Dialogue Node
4. Configure Play Once if needed
```

## Component Settings

### DialogueManager
```
UI References:
- Dialogue Text: TMP component to display text
- Dialogue Panel: Panel to show/hide
- Continue Icon: Icon shown when line is complete

Settings:
- Hide When Complete: Auto-hide panel after dialogue
- Advance Key: Key to progress dialogue (default: left mouse)
- Cancel Key: Key to cancel dialogue (default: escape)

Audio:
- Audio Source: For sound effects
- Typing Sound: Sound played while typing
- Typing Sound Interval: Delay between sounds
```

### DialogueNodeSO
```
Dialogue Content:
- Dialogue Lines: Array of text lines
- Can Be Cancelled: Allow ESC to exit
- Text Speed: Characters per second
- Voice Clips: Optional audio for each line
```

### DialogueTrigger
```
Settings:
- Dialogue: The DialogueNode to play
- Play Once: Only trigger once per game
```

## Usage Examples

### Basic Click Trigger
```csharp
// Add DialogueTrigger to any ClickableObject
dialogueTrigger.TriggerDialogue();
```

### Change Dialogue at Runtime
```csharp
DialogueTrigger trigger = GetComponent<DialogueTrigger>();
trigger.SetDialogue(newDialogueNode);
trigger.ResetDialogue(); // Allow replaying
```

### Check Dialogue State
```csharp
DialogueManager manager = FindObjectOfType<DialogueManager>();
if (!manager.IsDialogueActive())
{
  // Start new dialogue
}
```

## Features

### Text Animation
- Text appears character by character
- Click to complete current line instantly
- Continue icon shows when line is complete

### Audio Support
- Typing sounds at configurable intervals
- Voice clip support for each dialogue line
- Audio source auto-setup

### UI Control
- Auto-hide dialogue panel when complete
- Cancel dialogue with ESC (if enabled)
- Configurable input keys

### Reusability
- Dialogue nodes are ScriptableObjects
- Can be shared between multiple triggers
- Easy to modify at runtime

## Tips

1. **Performance**
   - Create dialogue nodes in a Resources folder for dynamic loading
   - Use object pooling for frequent dialogue triggers
   - Cache DialogueManager reference in Start()

2. **Best Practices**
   - Keep dialogue lines short and readable
   - Use consistent text speed across dialogues
   - Test with both mouse and keyboard controls

3. **Common Issues**
   - Missing DialogueManager in scene
   - Unassigned UI references
   - Dialogue node not assigned to trigger

4. **Customization**
   - Modify text speed per dialogue
   - Add custom sound effects
   - Change input controls
   - Add animations or effects

## Example Scene Setup

```
Hierarchy:
- Canvas
  - DialoguePanel (Panel)
    - DialogueText (TextMeshPro)
    - ContinueIcon (Image)
- DialogueManager
- ClickableObject
  - DialogueTrigger
```

Remember to:
- Set appropriate Canvas settings
- Configure TextMeshPro font and style
- Test dialogue with various text lengths
- Set up proper UI scaling
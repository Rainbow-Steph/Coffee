using UnityEngine;

/// <summary>
/// Add this component to any GameObject with a ClickableObject to trigger dialogue.
/// </summary>
[RequireComponent(typeof(ClickableObject))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueNodeSO dialogue;
    [SerializeField] private bool playOnce = true;
    
    private bool hasPlayed = false;
    private DialogueManager dialogueManager;

    private void Start()
    {
        // Find DialogueManager in scene
     dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
    {
    Debug.LogError("DialogueTrigger requires a DialogueManager in the scene!");
      enabled = false;
   return;
        }

        // Subscribe to ClickableObject's click event
    var clickable = GetComponent<ClickableObject>();
   if (clickable != null)
        {
            clickable.onClickEvent.AddListener(TriggerDialogue);
        }
    }

    /// <summary>
    /// Start the dialogue sequence
    /// </summary>
    public void TriggerDialogue()
    {
        if (dialogue == null || (playOnce && hasPlayed))
      return;

        if (!dialogueManager.IsDialogueActive())
      {
            dialogueManager.StartDialogue(dialogue);
   hasPlayed = true;
        }
    }

    /// <summary>
    /// Reset the played state
    /// </summary>
    public void ResetDialogue()
    {
  hasPlayed = false;
    }

    /// <summary>
    /// Change the dialogue node at runtime
    /// </summary>
    public void SetDialogue(DialogueNodeSO newDialogue)
    {
        dialogue = newDialogue;
        hasPlayed = false;
    }
}
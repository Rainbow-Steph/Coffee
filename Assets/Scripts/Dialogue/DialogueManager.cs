using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image continueIcon; // Optional icon to show when text is complete

    [Header("Settings")]
    [SerializeField] private bool hideWhenComplete = true;
    [SerializeField] private bool enableRichText = true; // Enable rich text formatting (colors, bold, italic, etc.)
    [SerializeField] private KeyCode advanceKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private float typingSoundInterval = 0.1f;

    private DialogueNodeSO currentDialogue;
    private int currentLineIndex = -1;
    private bool isTyping = false;
    private bool dialogueComplete = false;
    private Coroutine typeCoroutine;
    private float lastTypingSoundTime;

    private void Awake()
    {
        // Get AudioSource if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Ensure dialogue panel is hidden at start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueIcon != null)
            continueIcon.enabled = false;

        // Enable rich text on TMP_Text component
        if (dialogueText != null && enableRichText)
        {
            dialogueText.richText = true;
        }
    }

    private void Update()
    {
        if (currentDialogue == null) return;

        // Check for advance input
        if (Input.GetKeyDown(advanceKey))
        {
            if (isTyping)
            {
                // Complete current line instantly
                if (typeCoroutine != null)
                    StopCoroutine(typeCoroutine);
                CompleteLine();
            }
            else if (!dialogueComplete)
            {
                NextLine();
            }
        }

        // Check for cancel input
        if (currentDialogue.canBeCancelled && Input.GetKeyDown(cancelKey))
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Start displaying a dialogue sequence
    /// </summary>
    public void StartDialogue(DialogueNodeSO dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines == null || dialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("Attempted to start dialogue with null or empty DialogueNodeSO");
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = -1;
        dialogueComplete = false;

        // Show dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Start first line
        NextLine();
    }

    /// <summary>
    /// Advance to next line or end dialogue if at last line
    /// </summary>
    private void NextLine()
    {
        currentLineIndex++;

        // Check if we've reached the end
        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        // Start typing the next line
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypeLine(currentDialogue.dialogueLines[currentLineIndex]));

        // Play voice clip if available
        if (audioSource != null && currentDialogue.voiceClips != null && 
            currentLineIndex < currentDialogue.voiceClips.Length && 
            currentDialogue.voiceClips[currentLineIndex] != null)
        {
            audioSource.clip = currentDialogue.voiceClips[currentLineIndex];
            audioSource.Play();
        }
    }

    /// <summary>
    /// Display text one character at a time (with rich text support)
    /// </summary>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        if (continueIcon != null)
            continueIcon.enabled = false;

        if (enableRichText)
        {
            // Parse the text to separate visible characters from rich text tags
            int visibleCharIndex = 0;
            int currentIndex = 0;
            string displayText = "";
            bool insideTag = false;
            string currentTag = "";

            while (currentIndex < line.Length)
            {
                char c = line[currentIndex];

                // Check if we're entering a tag
                if (c == '<')
                {
                    insideTag = true;
                    currentTag = "<";
                    currentIndex++;
                    continue;
                }

                // Check if we're exiting a tag
                if (insideTag && c == '>')
                {
                    currentTag += ">";
                    insideTag = false;
                    displayText += currentTag;
                    dialogueText.text = displayText;
                    currentTag = "";
                    currentIndex++;
                    continue;
                }

                // If inside a tag, accumulate tag characters
                if (insideTag)
                {
                    currentTag += c;
                    currentIndex++;
                    continue;
                }

                // Regular visible character - type it out
                displayText += c;
                dialogueText.text = displayText;
                visibleCharIndex++;

                // Play typing sound at intervals (only for visible characters)
                if (audioSource != null && typingSound != null && 
                    Time.time - lastTypingSoundTime >= typingSoundInterval)
                {
                    audioSource.PlayOneShot(typingSound);
                    lastTypingSoundTime = Time.time;
                }

                yield return new WaitForSeconds(1f / currentDialogue.textSpeed);
                currentIndex++;
            }
        }
        else
        {
            // Simple character-by-character for non-rich text
            foreach (char c in line.ToCharArray())
            {
                dialogueText.text += c;

                // Play typing sound at intervals
                if (audioSource != null && typingSound != null && 
                    Time.time - lastTypingSoundTime >= typingSoundInterval)
                {
                    audioSource.PlayOneShot(typingSound);
                    lastTypingSoundTime = Time.time;
                }

                yield return new WaitForSeconds(1f / currentDialogue.textSpeed);
            }
        }

        CompleteLine();
    }

    /// <summary>
    /// Complete current line instantly
    /// </summary>
    private void CompleteLine()
    {
        if (currentLineIndex >= 0 && currentLineIndex < currentDialogue.dialogueLines.Length)
        {
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
        }

        isTyping = false;
        if (continueIcon != null)
            continueIcon.enabled = true;
    }

    /// <summary>
    /// End the current dialogue sequence
    /// </summary>
    public void EndDialogue()
    {
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);

        dialogueComplete = true;

        // Hide dialogue panel if configured
        if (hideWhenComplete && dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Clear dialogue text
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        // Clear current dialogue
        currentDialogue = null;
        currentLineIndex = -1;
        isTyping = false;

        if (continueIcon != null)
            continueIcon.enabled = false;
    }

    /// <summary>
    /// Public method to clear dialogue (can be called from ClickableObject on release)
    /// </summary>
    public void ClearDialogue()
    {
        EndDialogue();
    }

    /// <summary>
    /// Check if dialogue is currently active
    /// </summary>
    public bool IsDialogueActive()
    {
        return currentDialogue != null;
    }

    /// <summary>
    /// Check if dialogue is in progress but complete
    /// </summary>
    public bool IsDialogueComplete()
    {
        return dialogueComplete;
    }
}
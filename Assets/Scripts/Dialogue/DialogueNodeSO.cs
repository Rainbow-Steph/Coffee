using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Node")]
public class DialogueNodeSO : ScriptableObject
{
    [Header("Dialogue Content")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    [Header("Settings")]
    public bool canBeCancelled = true;
    public float textSpeed = 30f; // Characters per second
    
    [Header("Audio (Optional)")]
    public AudioClip[] voiceClips; // One per line, optional
}
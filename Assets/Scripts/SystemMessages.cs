using UnityEngine;

[CreateAssetMenu(fileName = "SystemMessages", menuName = "Coffee/System Messages")]
public class SystemMessages : ScriptableObject
{
    [Header("Error Messages")]
    public DialogueNodeSO capsuleFullMessage;

    private void OnEnable()
    {
        // Initialize if needed
        if (capsuleFullMessage == null)
        {
      Debug.LogWarning("System Messages: Capsule Full message not assigned!");
        }
    }
}
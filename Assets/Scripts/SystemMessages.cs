using UnityEngine;

[CreateAssetMenu(fileName = "SystemMessages", menuName = "Coffee/System Messages")]
public class SystemMessages : ScriptableObject
{
    [Header("Error Messages")]
    public DialogueNodeSO capsuleFullMessage;
    public DialogueNodeSO liquidNeededMessage;

    [Header("Success Messages")]
    public DialogueNodeSO craftSuccessMessage;

    private void OnEnable()
    {
        // Initialize if needed
        if (capsuleFullMessage == null)
        {
      Debug.LogWarning("System Messages: Capsule Full message not assigned!");
        }

        if (liquidNeededMessage == null)
      {
Debug.LogWarning("System Messages: Liquid Needed message not assigned!");
        }

        if (craftSuccessMessage == null)
  {
  Debug.LogWarning("System Messages: Craft Success message not assigned!");
   }
    }
}
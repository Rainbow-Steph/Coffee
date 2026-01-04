using UnityEngine;

public class ItemInteractionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerActionTracker actionTracker;
    [SerializeField] private SystemMessages systemMessages;
    [SerializeField] private DialogueManager dialogueManager;

    public void HandleInteraction(ClickableObject clickedObject, ClickableObject heldObject)
 {
        if (clickedObject == null || heldObject == null) return;

 // Machine interactions
   if (clickedObject.itemType == ItemType.Machine)
    {
      HandleMachineInteraction(heldObject);
 }
    }

    private void HandleMachineInteraction(ClickableObject heldObject)
    {
        switch (heldObject.itemType)
        {
            case ItemType.Liquid:
   actionTracker.LiquidName = heldObject.gameObject.name;
      
          break;

            case ItemType.Capsule:
   HandleCapsuleInteraction(heldObject);
      break;

            case ItemType.Additive:
        actionTracker.AdditiveName = heldObject.gameObject.name;
       Destroy(heldObject.gameObject);
             break;
        }
    }

    private void HandleCapsuleInteraction(ClickableObject capsule)
    {
        // Check Capsule A first
        if (string.IsNullOrEmpty(actionTracker.CapsuleAName))
      {
     actionTracker.CapsuleAName = capsule.gameObject.name;
  Destroy(capsule.gameObject);
    }
        // Then check Capsule B
   else if (string.IsNullOrEmpty(actionTracker.CapsuleBName))
        {
            actionTracker.CapsuleBName = capsule.gameObject.name;
        Destroy(capsule.gameObject);
      }
        // Both slots full
    else
        {
      if (systemMessages != null && systemMessages.capsuleFullMessage != null)
      {
dialogueManager.StartDialogue(systemMessages.capsuleFullMessage);
}
      else
            {
    Debug.LogWarning("System Messages or Capsule Full message not assigned!");
        }
   }
    }
}
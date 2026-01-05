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
     // Machine Water Input interactions
        else if (clickedObject.itemType == ItemType.MachineWaterInput)
        {
          HandleMachineWaterInput(heldObject);
     }
        // Machine Coffee Input interactions
        else if (clickedObject.itemType == ItemType.MachineCoffeeInput)
        {
 HandleMachineCoffeeInput(heldObject);
        }
   // Machine Extra Input interactions
        else if (clickedObject.itemType == ItemType.MachineExtraInput)
        {
       HandleMachineExtraInput(heldObject);
    }
    }

 private void HandleMachineInteraction(ClickableObject heldObject)
    {
        switch (heldObject.itemType)
    {
      case ItemType.Liquid:
          actionTracker.LiquidName = heldObject.gameObject.name;
                Destroy(heldObject.gameObject);
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

    private void HandleMachineWaterInput(ClickableObject heldObject)
    {
     // Only accept liquid items
        if (heldObject.itemType == ItemType.Liquid)
        {
      actionTracker.LiquidName = heldObject.gameObject.name;
 actionTracker.LiquidAmount++; // Increment liquid amount
  Destroy(heldObject.gameObject);
 Debug.Log($"Added {heldObject.gameObject.name} to water input. Total liquid: {actionTracker.LiquidAmount}");
        }
        else
   {
    Debug.LogWarning("Water input only accepts liquid items!");
        }
    }

    private void HandleMachineCoffeeInput(ClickableObject heldObject)
    {
        // Only accept capsule items
        if (heldObject.itemType == ItemType.Capsule)
        {
       HandleCapsuleInteraction(heldObject);
            Debug.Log($"Added {heldObject.gameObject.name} to coffee input");
        }
        else
        {
            Debug.LogWarning("Coffee input only accepts capsule items!");
        }
    }

    private void HandleMachineExtraInput(ClickableObject heldObject)
{
        // Only accept additive items
        if (heldObject.itemType == ItemType.Additive)
{
            actionTracker.AdditiveName = heldObject.gameObject.name;
            Destroy(heldObject.gameObject);
            Debug.Log($"Added {heldObject.gameObject.name} to extra input");
        }
   else
        {
       Debug.LogWarning("Extra input only accepts additive items!");
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
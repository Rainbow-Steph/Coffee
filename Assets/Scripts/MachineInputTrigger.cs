using UnityEngine;

/// <summary>
/// Trigger-based input system for coffee machine components.
/// Place this on child objects named "Input Trigger" under MachineWaterInput, MachineCoffeeInput, or MachineExtraInput objects.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MachineInputTrigger : MonoBehaviour
{
    [Header("Input Type")]
    [Tooltip("What type of items this input accepts")]
    [SerializeField] private InputType inputType = InputType.Water;

    [Header("References")]
    [Tooltip("Reference to the PlayerActionTracker")]
    [SerializeField] private PlayerActionTracker actionTracker;
    
    [Tooltip("Reference to SystemMessages for dialogue")]
 [SerializeField] private SystemMessages systemMessages;
    
    [Tooltip("Reference to DialogueManager for showing messages")]
    [SerializeField] private DialogueManager dialogueManager;

    [Header("Visual Feedback (Optional)")]
    [Tooltip("Display handler to update when items are added")]
    [SerializeField] private DisplayHandler displayHandler;

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugInfo = true;

    public enum InputType
    {
        Water,   // Accepts Liquid items
        Coffee,  // Accepts Capsule items
        Extra    // Accepts Additive items
    }

    private Collider triggerCollider;

    void Start()
    {
        // Ensure collider is set to trigger
   triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
      {
         triggerCollider.isTrigger = true;
      
     if (showDebugInfo)
         {
     Debug.Log($"[MachineInputTrigger] {gameObject.name} initialized as {inputType} input trigger");
      }
}
        else
        {
       Debug.LogError($"[MachineInputTrigger] No collider found on {gameObject.name}!");
   }

   // Auto-find references if not assigned
    if (actionTracker == null)
        {
   actionTracker = FindObjectOfType<PlayerActionTracker>();
     }

        if (systemMessages == null)
        {
     systemMessages = FindObjectOfType<SystemMessages>();
    }

   if (dialogueManager == null)
        {
     dialogueManager = FindObjectOfType<DialogueManager>();
        }

        ValidateReferences();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is a held ClickableObject
        ClickableObject clickableObject = other.GetComponent<ClickableObject>();
     
    if (clickableObject == null)
        {
     if (showDebugInfo)
        {
                Debug.Log($"[MachineInputTrigger] Object {other.gameObject.name} has no ClickableObject component");
    }
            return;
        }

        // Only process if the object is currently being held
        if (!ClickableObject.IsAnyItemHeld || ClickableObject.GetHeldObject() != clickableObject)
  {
            if (showDebugInfo)
       {
 Debug.Log($"[MachineInputTrigger] Object {other.gameObject.name} is not being held");
 }
            return;
   }

   // Process based on input type
 bool processed = false;
   switch (inputType)
        {
            case InputType.Water:
                processed = ProcessWaterInput(clickableObject);
  break;

     case InputType.Coffee:
  processed = ProcessCoffeeInput(clickableObject);
       break;

         case InputType.Extra:
            processed = ProcessExtraInput(clickableObject);
                break;
 }

        if (processed && showDebugInfo)
        {
            Debug.Log($"[MachineInputTrigger] Successfully processed {clickableObject.gameObject.name} for {inputType} input");
        }
    }

    private bool ProcessWaterInput(ClickableObject heldObject)
    {
        // Only accept liquid items
      if (heldObject.itemType != ItemType.Liquid)
        {
   if (showDebugInfo)
{
         Debug.LogWarning($"[MachineInputTrigger] Water input only accepts Liquid items! Received: {heldObject.itemType}");
    }
          return false;
        }

        if (actionTracker == null)
        {
            Debug.LogError("[MachineInputTrigger] PlayerActionTracker not assigned!");
     return false;
    }

 // Add water to machine
        string liquidName = heldObject.gameObject.name;
        actionTracker.LiquidName = liquidName;
        actionTracker.LiquidAmount++;

        if (showDebugInfo)
     {
            Debug.Log($"[MachineInputTrigger] Added {liquidName} to water input. Total liquid: {actionTracker.LiquidAmount}");
        }

        // Update display if assigned
   if (displayHandler != null)
        {
            displayHandler.UpdateDisplay();
        }

        // Destroy the held object
        Destroy(heldObject.gameObject);

        return true;
    }

    private bool ProcessCoffeeInput(ClickableObject heldObject)
    {
        // Only accept capsule items
        if (heldObject.itemType != ItemType.Capsule)
        {
         if (showDebugInfo)
       {
        Debug.LogWarning($"[MachineInputTrigger] Coffee input only accepts Capsule items! Received: {heldObject.itemType}");
       }
            return false;
     }

        if (actionTracker == null)
  {
    Debug.LogError("[MachineInputTrigger] PlayerActionTracker not assigned!");
  return false;
        }

        // Try to add capsule to available slot
        string capsuleName = heldObject.gameObject.name;

    // Check Capsule A first
        if (string.IsNullOrEmpty(actionTracker.CapsuleAName))
        {
actionTracker.CapsuleAName = capsuleName;
         
    if (showDebugInfo)
     {
              Debug.Log($"[MachineInputTrigger] Added {capsuleName} to Capsule A slot");
            }

            // Update display if assigned
         if (displayHandler != null)
       {
       displayHandler.UpdateDisplay();
      }

            // Destroy the held object
       Destroy(heldObject.gameObject);
            return true;
}
        // Then check Capsule B
    else if (string.IsNullOrEmpty(actionTracker.CapsuleBName))
        {
  actionTracker.CapsuleBName = capsuleName;

            if (showDebugInfo)
            {
        Debug.Log($"[MachineInputTrigger] Added {capsuleName} to Capsule B slot");
            }

  // Update display if assigned
            if (displayHandler != null)
            {
  displayHandler.UpdateDisplay();
            }

      // Destroy the held object
 Destroy(heldObject.gameObject);
            return true;
        }
   // Both slots full
else
        {
     if (showDebugInfo)
  {
         Debug.LogWarning($"[MachineInputTrigger] Cannot add {capsuleName} - both capsule slots are full!");
     }

            // Show "capsule full" message
   if (systemMessages != null && systemMessages.capsuleFullMessage != null && dialogueManager != null)
        {
      dialogueManager.StartDialogue(systemMessages.capsuleFullMessage);
            }

  return false;
   }
  }

    private bool ProcessExtraInput(ClickableObject heldObject)
    {
        // Only accept additive items
        if (heldObject.itemType != ItemType.Additive)
   {
  if (showDebugInfo)
            {
  Debug.LogWarning($"[MachineInputTrigger] Extra input only accepts Additive items! Received: {heldObject.itemType}");
            }
      return false;
        }

      if (actionTracker == null)
        {
    Debug.LogError("[MachineInputTrigger] PlayerActionTracker not assigned!");
 return false;
   }

        // Add additive to machine
        string additiveName = heldObject.gameObject.name;
        actionTracker.AdditiveName = additiveName;

        if (showDebugInfo)
        {
            Debug.Log($"[MachineInputTrigger] Added {additiveName} to extra input");
        }

// Update display if assigned
        if (displayHandler != null)
     {
   displayHandler.UpdateDisplay();
        }

// Destroy the held object
        Destroy(heldObject.gameObject);

        return true;
    }

    private bool ValidateReferences()
    {
        bool valid = true;

if (actionTracker == null)
        {
    Debug.LogError($"[MachineInputTrigger] PlayerActionTracker not assigned on {gameObject.name}!");
     valid = false;
        }

 if (systemMessages == null)
      {
      Debug.LogWarning($"[MachineInputTrigger] SystemMessages not assigned on {gameObject.name} - dialogue messages won't work!");
        }

 if (dialogueManager == null)
        {
            Debug.LogWarning($"[MachineInputTrigger] DialogueManager not assigned on {gameObject.name} - dialogue messages won't work!");
      }

        return valid;
 }

    // Visual helper in editor
    void OnDrawGizmos()
    {
        // Draw a colored wireframe to show the trigger area
        Collider col = GetComponent<Collider>();
      if (col != null)
        {
    Color gizmoColor = Color.green;
      switch (inputType)
    {
         case InputType.Water:
        gizmoColor = new Color(0.3f, 0.6f, 1f, 0.5f); // Blue
   break;
      case InputType.Coffee:
  gizmoColor = new Color(0.6f, 0.3f, 0f, 0.5f); // Brown
    break;
  case InputType.Extra:
                  gizmoColor = new Color(1f, 0.8f, 0.3f, 0.5f); // Yellow
          break;
    }

 Gizmos.color = gizmoColor;
      
        if (col is BoxCollider)
       {
      BoxCollider box = col as BoxCollider;
      Gizmos.matrix = transform.localToWorldMatrix;
     Gizmos.DrawWireCube(box.center, box.size);
       }
            else if (col is SphereCollider)
        {
            SphereCollider sphere = col as SphereCollider;
     Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
    }
        }
    }
}

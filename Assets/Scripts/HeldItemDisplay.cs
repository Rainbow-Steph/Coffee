using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script to display the currently held item name in the UI.
/// Supports both Unity UI Text and TextMeshPro components.
/// Attach this to a UI Text or TextMeshPro component.
/// </summary>
public class HeldItemDisplay : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Unity UI Text component to display the held item name")]
    public Text displayText;
    
    [Tooltip("TextMeshPro component to display the held item name (alternative to displayText)")]
    public TMP_Text displayTextTMP;
    
    [Tooltip("Prefix text to show before the item name")]
    public string prefix = "Holding: ";
    
    [Tooltip("Text to show when no item is held")]
    public string emptyText = "No item held";
    
    [Header("Optional Features")]
    [Tooltip("Hide the entire UI element when no item is held")]
    public bool hideWhenEmpty = false;
    
    private bool useTextMeshPro = false;
    private CanvasGroup parentCanvasGroup;
    
    void Start()
    {
    // Get or add CanvasGroup on parent for smooth fade transitions
        parentCanvasGroup = GetComponentInParent<CanvasGroup>();
        if (hideWhenEmpty && parentCanvasGroup == null)
        {
            parentCanvasGroup = transform.parent?.gameObject.AddComponent<CanvasGroup>();
          if (parentCanvasGroup == null)
            {
          Debug.LogWarning("HeldItemDisplay: No parent object found for hiding UI element. Using text component visibility instead.");
        }
        }
        
        // Determine which text component to use
        if (displayTextTMP != null)
        {
            useTextMeshPro = true;
        if (displayText != null)
 {
       Debug.LogWarning("HeldItemDisplay: Both Text and TextMeshPro components assigned. Using TextMeshPro.");
      }
        }
        else if (displayText == null)
        {
            // Try to get Text component if not assigned
        displayText = GetComponent<Text>();
 
            if (displayText == null)
      {
   // Try to get TextMeshPro component if Text not found
  displayTextTMP = GetComponent<TMP_Text>();

           if (displayTextTMP != null)
 {
                useTextMeshPro = true;
                }
     else
          {
      Debug.LogError("HeldItemDisplay: No Text or TextMeshPro component found! Please assign a Text or TextMeshPro component.");
 }
            }
        }
        
        if (useTextMeshPro && displayTextTMP != null)
        {
       Debug.Log("HeldItemDisplay: Using TextMeshPro component.");
        }
        else if (displayText != null)
        {
     Debug.Log("HeldItemDisplay: Using Unity UI Text component.");
 }
    }
    
    void Update()
    {
    // Check if we have a valid text component
        if (!useTextMeshPro && displayText == null) return;
        if (useTextMeshPro && displayTextTMP == null) return;
        
        // Check if any item is being held
   if (ClickableObject.IsAnyItemHeld)
        {
            // Display the held item name
       string textToDisplay = prefix + ClickableObject.HeldItemName;
 
            if (useTextMeshPro)
            {
           displayTextTMP.text = textToDisplay;
      }
            else
         {
             displayText.text = textToDisplay;
     }
   
        // Show UI element
      if (hideWhenEmpty)
      {
              if (parentCanvasGroup != null)
         {
         parentCanvasGroup.alpha = 1f;
          parentCanvasGroup.interactable = true;
              parentCanvasGroup.blocksRaycasts = true;
                }
  else
    {
           // Fallback to text visibility if no CanvasGroup
        if (useTextMeshPro)
   displayTextTMP.enabled = true;
      else
       displayText.enabled = true;
    }
         }
 }
        else
        {
         // No item held
          if (hideWhenEmpty)
     {
        // Hide UI element
           if (parentCanvasGroup != null)
                {
          parentCanvasGroup.alpha = 0f;
parentCanvasGroup.interactable = false;
          parentCanvasGroup.blocksRaycasts = false;
        }
           else
            {
  // Fallback to text visibility if no CanvasGroup
   if (useTextMeshPro)
     displayTextTMP.enabled = false;
  else
          displayText.enabled = false;
                }
 }
   else
 {
    // Just update text
         if (useTextMeshPro)
       {
   displayTextTMP.text = emptyText;
        }
                else
          {
        displayText.text = emptyText;
     }
   }
        }
    }
    
    /// <summary>
  /// Example method to check held item and perform actions
 /// </summary>
    public void CheckHeldItem()
    {
  if (ClickableObject.IsAnyItemHeld)
        {
     string itemName = ClickableObject.HeldItemName;
 Debug.Log($"Player is holding: {itemName}");
        
  // Example: Check for specific items
   if (itemName.Contains("Key"))
            {
        Debug.Log("Player is holding a key!");
            }
  
            // Get reference to the actual object
            ClickableObject heldObject = ClickableObject.GetHeldObject();
            if (heldObject != null)
   {
           // You can access any properties or methods on the held object
                Debug.Log($"Held object position: {heldObject.transform.position}");
            }
        }
        else
        {
            Debug.Log("Player is not holding anything");
        }
    }
}

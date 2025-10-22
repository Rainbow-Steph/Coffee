using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example script showing how to display the currently held item name in the UI.
/// Attach this to a UI Text or TextMeshPro component.
/// </summary>
public class HeldItemDisplay : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Text component to display the held item name")]
public Text displayText;
    
    [Tooltip("Prefix text to show before the item name")]
    public string prefix = "Holding: ";
    
    [Tooltip("Text to show when no item is held")]
    public string emptyText = "No item held";
    
    [Header("Optional Features")]
    [Tooltip("Hide the text when no item is held")]
    public bool hideWhenEmpty = false;
    
    void Start()
    {
        // Try to get Text component if not assigned
        if (displayText == null)
        {
        displayText = GetComponent<Text>();
        }
        
        if (displayText == null)
        {
            Debug.LogError("HeldItemDisplay: No Text component found! Please assign a Text component.");
     }
    }
 
    void Update()
  {
      if (displayText == null) return;
 
   // Check if any item is being held
        if (ClickableObject.IsAnyItemHeld)
        {
   // Display the held item name
            displayText.text = prefix + ClickableObject.HeldItemName;
  
    // Make sure text is visible
       if (hideWhenEmpty)
      {
         displayText.enabled = true;
  }
   }
     else
   {
          // No item held
            if (hideWhenEmpty)
            {
    displayText.enabled = false;
  }
         else
   {
 displayText.text = emptyText;
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

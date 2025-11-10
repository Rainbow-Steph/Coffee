using UnityEngine;
using TMPro;

/// <summary>
/// Component to display and debug PlayerActionTracker contents using TextMeshPro
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerActionDebugDisplay : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the PlayerActionTracker scriptable object")]
public PlayerActionTracker actionTracker;

    [Header("Display Settings")]
    [Tooltip("Format string for the display text. Use default for standard layout")]
    [TextArea(3, 10)]
    public string displayFormat = "Player Actions Debug:\n" +
 "----------------\n" +
        "Held Item: {0}\n" +
           "----------------\n" +
 "Machine Contents:\n" +
    "Liquid: {1}\n" +
  "Capsule A: {2}\n" +
        "Capsule B: {3}\n" +
         "Additive: {4}\n" +
           "Expected: {5}";

    private TextMeshProUGUI debugText;

    private void OnEnable()
    {
        if (!SetupReferences())
        return;

        // Subscribe to events
        actionTracker.onHeldItemChanged += OnHeldItemChanged;
        actionTracker.onMachineContentsChanged += OnMachineContentsChanged;

        // Initial update
     UpdateDisplay();
    }

    private void OnDisable()
    {
  if (actionTracker != null)
    {
         actionTracker.onHeldItemChanged -= OnHeldItemChanged;
     actionTracker.onMachineContentsChanged -= OnMachineContentsChanged;
        }
    }

    private bool SetupReferences()
 {
        debugText = GetComponent<TextMeshProUGUI>();
    if (debugText == null)
        {
            Debug.LogError($"[{GetType().Name}] No TextMeshProUGUI component found!", this);
       enabled = false;
    return false;
        }

        if (actionTracker == null)
      {
    Debug.LogError($"[{GetType().Name}] No PlayerActionTracker assigned!", this);
      enabled = false;
            return false;
        }

  return true;
    }

    private void OnHeldItemChanged(string newItem)
  {
        Debug.Log($"[Player Actions] Held item changed to: {(string.IsNullOrEmpty(newItem) ? "None" : newItem)}");
        UpdateDisplay();
    }

    private void OnMachineContentsChanged(PlayerActionTracker.MachineContents contents)
    {
     Debug.Log($"[Player Actions] Machine contents changed:\n{contents}");
        UpdateDisplay();
    }

    private void UpdateDisplay()
  {
        if (debugText == null || actionTracker == null) return;

debugText.text = string.Format(displayFormat,
            string.IsNullOrEmpty(actionTracker.HeldItemName) ? "None" : actionTracker.HeldItemName,
    string.IsNullOrEmpty(actionTracker.LiquidName) ? "Empty" : actionTracker.LiquidName,
string.IsNullOrEmpty(actionTracker.CapsuleAName) ? "Empty" : actionTracker.CapsuleAName,
   string.IsNullOrEmpty(actionTracker.CapsuleBName) ? "Empty" : actionTracker.CapsuleBName,
   string.IsNullOrEmpty(actionTracker.AdditiveName) ? "Empty" : actionTracker.AdditiveName,
string.IsNullOrEmpty(actionTracker.ExpectedResult) ? "None" : actionTracker.ExpectedResult
        );
    }
}
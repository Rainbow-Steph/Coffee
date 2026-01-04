using UnityEngine;

/// <summary>
/// Scriptable Object to track player's actions and machine combinations
/// </summary>
[CreateAssetMenu(fileName = "PlayerActionTracker", menuName = "Coffee/Player Action Tracker")]
public class PlayerActionTracker : ScriptableObject
{
    [System.Serializable]
    public class MachineContents
    {
   public string liquidName;
  public string capsuleAName;
        public string capsuleBName;
        public string additiveName;
        public string expectedResult;

        public void Clear()
    {
     liquidName = "";
            capsuleAName = "";
    capsuleBName = "";
    additiveName = "";
    expectedResult = "";
        }

        public override string ToString()
 {
            return $"Machine Contents:\n" +
         $"Liquid: {(string.IsNullOrEmpty(liquidName) ? "Empty" : liquidName)}\n" +
         $"Capsule A: {(string.IsNullOrEmpty(capsuleAName) ? "Empty" : capsuleAName)}\n" +
            $"Capsule B: {(string.IsNullOrEmpty(capsuleBName) ? "Empty" : capsuleBName)}\n" +
          $"Additive: {(string.IsNullOrEmpty(additiveName) ? "Empty" : additiveName)}\n" +
                 $"Expected Result: {(string.IsNullOrEmpty(expectedResult) ? "None" : expectedResult)}";
        }
    }

    [Header("Current State")]
    [SerializeField]
    private string heldItemName;

    [SerializeField]
    private MachineContents machineContents = new MachineContents();

    // Events for state changes
    public System.Action<string> onHeldItemChanged;
    public System.Action<MachineContents> onMachineContentsChanged;

    // Properties with change notification
    public string HeldItemName
    {
        get => heldItemName;
        set
        {
 if (heldItemName != value)
  {
    heldItemName = value;
            onHeldItemChanged?.Invoke(heldItemName);
      }
        }
 }

    public string LiquidName
    {
  get => machineContents.liquidName;
        set
        {
            if (machineContents.liquidName != value)
  {
     machineContents.liquidName = value;
      onMachineContentsChanged?.Invoke(machineContents);
 }
      }
    }

    public string CapsuleAName
    {
        get => machineContents.capsuleAName;
 set
        {
            if (machineContents.capsuleAName != value)
       {
                machineContents.capsuleAName = value;
   onMachineContentsChanged?.Invoke(machineContents);
  }
  }
    }

    public string CapsuleBName
    {
        get => machineContents.capsuleBName;
        set
    {
        if (machineContents.capsuleBName != value)
            {
        machineContents.capsuleBName = value;
    onMachineContentsChanged?.Invoke(machineContents);
         }
        }
    }

 public string AdditiveName
 {
get => machineContents.additiveName;
    set
  {
            if (machineContents.additiveName != value)
        {
   machineContents.additiveName = value;
    onMachineContentsChanged?.Invoke(machineContents);
  }
        }
    }

    public string ExpectedResult
    {
      get => machineContents.expectedResult;
      set
     {
            if (machineContents.expectedResult != value)
            {
         machineContents.expectedResult = value;
      onMachineContentsChanged?.Invoke(machineContents);
 }
        }
    }

    /// <summary>
    /// Clears all machine contents
    /// </summary>
    public void ClearMachineContents()
{
        machineContents.Clear();
        onMachineContentsChanged?.Invoke(machineContents);
    }

    private void OnEnable()
    {
        // Initialize with empty values when the SO is enabled
        if (machineContents == null)
   machineContents = new MachineContents();
    }
}
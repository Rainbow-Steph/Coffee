using UnityEngine;

/// <summary>
/// Handles visual updates for machine displays based on PlayerActionTracker changes
/// </summary>
public class DisplayHandler : MonoBehaviour
{
    public enum DisplayType
    {
        WaterDisplay1,
        WaterDisplay2,
        WaterDisplay3,
        CoffeeDisplay1,
        CoffeeDisplay2,
        ExtraDisplay
    }

    [System.Serializable]
    public class DisplayAssignment
    {
        public DisplayType displayType;
        public GameObject displayGameObject;
        [HideInInspector] public Renderer cachedRenderer;
    }

    [Header("References")]
    [SerializeField] private PlayerActionTracker actionTracker;
    [SerializeField] private DisplayHandlerConfig displayConfig;

    [Header("Display Assignments")]
    [Tooltip("Assign GameObjects to their display types here")]
    [SerializeField] private DisplayAssignment[] displayAssignments = new DisplayAssignment[6];

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    // Quick lookup dictionaries for display assignments
    private DisplayAssignment waterDisplay1Assignment;
    private DisplayAssignment waterDisplay2Assignment;
    private DisplayAssignment waterDisplay3Assignment;
    private DisplayAssignment coffeeDisplay1Assignment;
    private DisplayAssignment coffeeDisplay2Assignment;
    private DisplayAssignment extraDisplayAssignment;

    private void Awake()
    {
        // Cache renderer components and organize assignments
        CacheRenderersAndOrganize();
    }

    private void OnEnable()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        // Subscribe to machine contents changes
        actionTracker.onMachineContentsChanged += OnMachineContentsChanged;
        
        // Subscribe to liquid amount changes (for water display updates)
        actionTracker.onLiquidAmountChanged += OnLiquidAmountChanged;

        // Initial update
        UpdateAllDisplays(actionTracker.LiquidName, actionTracker.CapsuleAName, 
            actionTracker.CapsuleBName, actionTracker.AdditiveName);
    }

    private void OnDisable()
    {
        if (actionTracker != null)
        {
            actionTracker.onMachineContentsChanged -= OnMachineContentsChanged;
            actionTracker.onLiquidAmountChanged -= OnLiquidAmountChanged;
        }
    }

    private void CacheRenderersAndOrganize()
    {
        System.Text.StringBuilder setupLog = new System.Text.StringBuilder();
        setupLog.AppendLine("?????????????????????????????????????????????????????????????");
        setupLog.AppendLine("?       DISPLAY HANDLER - SETUP REPORT         ?");
        setupLog.AppendLine("?????????????????????????????????????????????????????????????");
        
     if (displayAssignments == null || displayAssignments.Length == 0)
    {
            setupLog.AppendLine("? ERROR: No display assignments configured!");
            Debug.LogWarning(setupLog.ToString(), this);
            return;
     }

        setupLog.AppendLine($"\n??? PROCESSING {displayAssignments.Length} ASSIGNMENTS ???\n");

  int successCount = 0;
  int errorCount = 0;

        foreach (var assignment in displayAssignments)
        {
  if (assignment == null)
         {
        setupLog.AppendLine("  ??  NULL assignment found in array");
     errorCount++;
          continue;
         }

        setupLog.AppendLine($"  [{assignment.displayType}]");
    
 if (assignment.displayGameObject == null)
            {
         setupLog.AppendLine($"    ? No GameObject assigned");
   errorCount++;
   continue;
            }

            setupLog.AppendLine($"    GameObject: {assignment.displayGameObject.name}");

      // Cache the Renderer component
          assignment.cachedRenderer = assignment.displayGameObject.GetComponent<Renderer>();

     if (assignment.cachedRenderer == null)
    {
         setupLog.AppendLine($"    ? Missing Renderer component!");
     errorCount++;
         continue;
            }

     setupLog.AppendLine($"    Renderer: {assignment.cachedRenderer.GetType().Name}");

      // Organize into quick lookup references
            switch (assignment.displayType)
            {
  case DisplayType.WaterDisplay1:
               waterDisplay1Assignment = assignment;
         setupLog.AppendLine($"    ? Water Display 1 configured");
     successCount++;
           break;

       case DisplayType.WaterDisplay2:
      waterDisplay2Assignment = assignment;
       setupLog.AppendLine($"    ? Water Display 2 configured");
    successCount++;
         break;

 case DisplayType.WaterDisplay3:
          waterDisplay3Assignment = assignment;
  setupLog.AppendLine($"    ? Water Display 3 configured");
       successCount++;
      break;

       case DisplayType.CoffeeDisplay1:
      coffeeDisplay1Assignment = assignment;
       setupLog.AppendLine($"    ? Coffee Display 1 configured");
      successCount++;
      break;

            case DisplayType.CoffeeDisplay2:
     coffeeDisplay2Assignment = assignment;
  setupLog.AppendLine($"    ? Coffee Display 2 configured");
      successCount++;
   break;

                case DisplayType.ExtraDisplay:
extraDisplayAssignment = assignment;
          setupLog.AppendLine($"    ? Extra Display configured");
              successCount++;
         break;
            }
            setupLog.AppendLine();
        }

        setupLog.AppendLine("??? FINAL STATUS ???");
        setupLog.AppendLine($"  Water Display 1:  {(waterDisplay1Assignment != null ? "? OK" : "? MISSING")}");
        setupLog.AppendLine($"  Water Display 2:  {(waterDisplay2Assignment != null ? "? OK" : "? MISSING")}");
        setupLog.AppendLine($"  Water Display 3:  {(waterDisplay3Assignment != null ? "? OK" : "? MISSING")}");
        setupLog.AppendLine($"  Coffee Display 1: {(coffeeDisplay1Assignment != null ? "? OK" : "? MISSING")}");
        setupLog.AppendLine($"  Coffee Display 2: {(coffeeDisplay2Assignment != null ? "? OK" : "? MISSING")}");
 setupLog.AppendLine($"  Extra Display:    {(extraDisplayAssignment != null ? "? OK" : "? MISSING")}");
        setupLog.AppendLine($"\nSummary: {successCount} configured, {errorCount} errors");
        setupLog.AppendLine("?????????????????????????????????????????????????????????????");

        Debug.Log(setupLog.ToString());
    }

    private bool ValidateReferences()
    {
        if (actionTracker == null)
        {
            Debug.LogError($"[{GetType().Name}] PlayerActionTracker not assigned!", this);
            return false;
        }

        if (displayConfig == null)
        {
            Debug.LogError($"[{GetType().Name}] DisplayHandlerConfig not assigned!", this);
            return false;
        }

        if (!displayConfig.ValidateConfig())
        {
            Debug.LogError($"[{GetType().Name}] DisplayHandlerConfig is not properly configured!", this);
            return false;
        }

        // Validate display assignments
        if (waterDisplay1Assignment == null || waterDisplay2Assignment == null || 
            waterDisplay3Assignment == null)
        {
            Debug.LogError($"[{GetType().Name}] Water display assignments incomplete!", this);
            return false;
        }

        if (coffeeDisplay1Assignment == null || coffeeDisplay2Assignment == null)
        {
            Debug.LogError($"[{GetType().Name}] Coffee display assignments incomplete!", this);
            return false;
        }

        if (extraDisplayAssignment == null)
        {
            Debug.LogError($"[{GetType().Name}] Extra display assignment not configured!", this);
            return false;
        }

        return true;
    }

    private void OnMachineContentsChanged(PlayerActionTracker.MachineContents contents)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[DisplayHandler] Machine contents changed: {contents}");
        }

        UpdateAllDisplays(contents.liquidName, contents.capsuleAName, 
            contents.capsuleBName, contents.additiveName);
    }

    /// <summary>
    /// Called when liquid amount changes - updates water displays
    /// </summary>
    private void OnLiquidAmountChanged(int newAmount)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[DisplayHandler] Liquid amount changed to: {newAmount}");
        }

        // Update water displays based on new amount
        UpdateWaterDisplays(actionTracker.LiquidName);
    }

    private void UpdateAllDisplays(string liquidName, string capsuleAName, 
        string capsuleBName, string additiveName)
    {
        UpdateWaterDisplays(liquidName);
        UpdateCoffeeDisplays(capsuleAName, capsuleBName);
        UpdateExtraDisplay(additiveName); // ? FIXED: Added missing call
    }

    #region Water Display Updates

    private void UpdateWaterDisplays(string liquidName)
    {
        System.Text.StringBuilder waterLog = new System.Text.StringBuilder();
        waterLog.AppendLine("???????????????????????????????????????????????????????");
        waterLog.AppendLine("?          UPDATE WATER DISPLAYS    ?");
        waterLog.AppendLine("???????????????????????????????????????????????????????");
        
        // Get current liquid amount from tracker
        int liquidAmount = actionTracker != null ? actionTracker.LiquidAmount : 0;
        bool hasWater = !string.IsNullOrEmpty(liquidName);

        waterLog.AppendLine($"\n??? STATE ???");
        waterLog.AppendLine($"  Liquid Name:   {(string.IsNullOrEmpty(liquidName) ? "EMPTY" : liquidName)}");
        waterLog.AppendLine($"  Liquid Amount: {liquidAmount}");
        waterLog.AppendLine($"  Has Water:     {hasWater}");

        waterLog.AppendLine($"\n??? DISPLAY LOGIC ???");
        waterLog.AppendLine($"  Display 1: {(liquidAmount >= 1 ? "FILLED" : "EMPTY")} (requires 1+)");
        waterLog.AppendLine($"  Display 2: {(liquidAmount >= 2 ? "FILLED" : "EMPTY")} (requires 2+)");
        waterLog.AppendLine($"  Display 3: {(liquidAmount >= 3 ? "FILLED" : "EMPTY")} (requires 3+)");

        waterLog.AppendLine($"\n??? APPLYING TO DISPLAYS ???");

        int successCount = 0;

        // Display 1: Shows filled if liquidAmount >= 1
        if (waterDisplay1Assignment?.cachedRenderer != null)
        {
            Material display1Material = liquidAmount >= 1 ? displayConfig.waterFilledMaterial : displayConfig.waterEmptyMaterial;
            waterDisplay1Assignment.cachedRenderer.material = display1Material;
            waterLog.AppendLine($"  Display 1: ? Updated to {(liquidAmount >= 1 ? "FILLED" : "EMPTY")}");
            successCount++;
        }
        else
        {
            waterLog.AppendLine($"  Display 1: ? Renderer null");
        }

        // Display 2: Shows filled if liquidAmount >= 2
        if (waterDisplay2Assignment?.cachedRenderer != null)
        {
            Material display2Material = liquidAmount >= 2 ? displayConfig.waterFilledMaterial : displayConfig.waterEmptyMaterial;
            waterDisplay2Assignment.cachedRenderer.material = display2Material;
            waterLog.AppendLine($"  Display 2: ? Updated to {(liquidAmount >= 2 ? "FILLED" : "EMPTY")}");
            successCount++;
        }
        else
        {
            waterLog.AppendLine($"  Display 2: ? Renderer null");
        }

        // Display 3: Shows filled if liquidAmount >= 3
        if (waterDisplay3Assignment?.cachedRenderer != null)
        {
            Material display3Material = liquidAmount >= 3 ? displayConfig.waterFilledMaterial : displayConfig.waterEmptyMaterial;
            waterDisplay3Assignment.cachedRenderer.material = display3Material;
            waterLog.AppendLine($"  Display 3: ? Updated to {(liquidAmount >= 3 ? "FILLED" : "EMPTY")}");
            successCount++;
        }
        else
        {
            waterLog.AppendLine($"  Display 3: ? Renderer null");
        }

        waterLog.AppendLine($"\n??? RESULT ???");
        waterLog.AppendLine($"  Status: {successCount}/3 displays updated");
        waterLog.AppendLine($"  Liquid Amount: {liquidAmount}");
        waterLog.AppendLine($"  Visual State: {GetWaterDisplayState(liquidAmount)}");
        waterLog.AppendLine($"???????????????????????????????????????????????????????");

        Debug.Log(waterLog.ToString());
    }

    /// <summary>
    /// Get a visual representation of water display state
    /// </summary>
    private string GetWaterDisplayState(int liquidAmount)
    {
        if (liquidAmount >= 3)
            return "??? (All Full)";
        else if (liquidAmount == 2)
            return "??? (2/3 Full)";
        else if (liquidAmount == 1)
            return "??? (1/3 Full)";
        else
            return "??? (Empty)";
    }

    #endregion

    #region Coffee Display Updates

    private void UpdateCoffeeDisplays(string capsuleAName, string capsuleBName)
    {
        // Update Coffee Display 1 with Capsule A
        UpdateCoffeeDisplay(coffeeDisplay1Assignment, capsuleAName, "Coffee Display 1");

        // Update Coffee Display 2 with Capsule B
        UpdateCoffeeDisplay(coffeeDisplay2Assignment, capsuleBName, "Coffee Display 2");
    }

    private void UpdateCoffeeDisplay(DisplayAssignment assignment, string capsuleName, string displayName)
    {
      System.Text.StringBuilder updateLog = new System.Text.StringBuilder();
        updateLog.AppendLine($"?????????????????????????????????????????????????????????????");
        updateLog.AppendLine($"?       UPDATE {displayName.ToUpper()} ?");
        updateLog.AppendLine($"?????????????????????????????????????????????????????????????");
        
        updateLog.AppendLine($"\n??? CHECK ???");
   updateLog.AppendLine($"  Assignment:      {(assignment != null ? "Valid" : "NULL")}");
        updateLog.AppendLine($"  Renderer:        {(assignment?.cachedRenderer != null ? "Valid" : "NULL")}");
        updateLog.AppendLine($"  Capsule Name:    '{capsuleName}'");
        
  if (assignment?.cachedRenderer == null)
        {
     updateLog.AppendLine($"\n??? RESULT ???");
      updateLog.AppendLine($"  Status: ? FAILED - Renderer is null");
  updateLog.AppendLine($"?????????????????????????????????????????????????????????????");
    Debug.LogWarning(updateLog.ToString());
        return;
        }

        Material targetMaterial;
   string materialSource;

   if (string.IsNullOrEmpty(capsuleName))
   {
            targetMaterial = displayConfig.coffeeEmptyMaterial;
       materialSource = "Empty (no capsule)";
        }
     else
      {
   targetMaterial = GetCapsuleMaterial(capsuleName, out string colorMatch);
       materialSource = colorMatch;
     }

        Material currentMaterial = assignment.cachedRenderer.material;
        
        updateLog.AppendLine($"\n??? MATERIALS ???");
 updateLog.AppendLine($"  Current:  {(currentMaterial != null ? currentMaterial.name : "NULL")}");
        updateLog.AppendLine($"  Target:   {(targetMaterial != null ? targetMaterial.name : "NULL")}");
        updateLog.AppendLine($"  Source:   {materialSource}");
  
  assignment.cachedRenderer.material = targetMaterial;
      
   updateLog.AppendLine($"\n??? RESULT ???");
 updateLog.AppendLine($"  Status: ? SUCCESS - Material applied");
        updateLog.AppendLine($"  Display now shows: {(string.IsNullOrEmpty(capsuleName) ? "Empty" : capsuleName)}");
  updateLog.AppendLine($"?????????????????????????????????????????????????????????????");

        Debug.Log(updateLog.ToString());
    }

    private Material GetCapsuleMaterial(string capsuleName)
    {
        return GetCapsuleMaterial(capsuleName, out _);
    }

    private Material GetCapsuleMaterial(string capsuleName, out string matchType)
  {
   // Check for red capsule
        if (capsuleName.Contains(displayConfig.redCapsuleIdentifier))
   {
    matchType = $"Red capsule (matches '{displayConfig.redCapsuleIdentifier}')";
       return displayConfig.redCapsuleMaterial;
        }
  // Check for blue capsule
        else if (capsuleName.Contains(displayConfig.blueCapsuleIdentifier))
        {
        matchType = $"Blue capsule (matches '{displayConfig.blueCapsuleIdentifier}')";
  return displayConfig.blueCapsuleMaterial;
     }
        // Check for black capsule
  else if (capsuleName.Contains(displayConfig.blackCapsuleIdentifier))
        {
       matchType = $"Black capsule (matches '{displayConfig.blackCapsuleIdentifier}')";
    return displayConfig.blackCapsuleMaterial;
     }

        // Default to empty if no match found
        matchType = $"No match found - using empty";
        if (showDebugLogs)
  {
            Debug.LogWarning($"[DisplayHandler] Capsule '{capsuleName}' did not match identifiers: '{displayConfig.redCapsuleIdentifier}', '{displayConfig.blueCapsuleIdentifier}', '{displayConfig.blackCapsuleIdentifier}'");
        }

        return displayConfig.coffeeEmptyMaterial;
    }

    #endregion

    #region Extra Display Updates

    private void UpdateExtraDisplay(string additiveName)
    {
   System.Text.StringBuilder extraLog = new System.Text.StringBuilder();
   extraLog.AppendLine("?????????????????????????????????????????????????????????????");
   extraLog.AppendLine("?          UPDATE EXTRA DISPLAY       ?");
        extraLog.AppendLine("?????????????????????????????????????????????????????????????");
        
 extraLog.AppendLine($"\n??? CHECK ???");
        extraLog.AppendLine($"  Assignment:      {(extraDisplayAssignment != null ? "Valid" : "NULL")}");
        extraLog.AppendLine($"  Renderer:   {(extraDisplayAssignment?.cachedRenderer != null ? "Valid" : "NULL")}");
  extraLog.AppendLine($"  Additive Name:   '{additiveName}'");

    if (extraDisplayAssignment?.cachedRenderer == null)
   {
   extraLog.AppendLine($"\n??? RESULT ???");
    extraLog.AppendLine($"  Status: ? FAILED - Renderer is null");
          extraLog.AppendLine($"?????????????????????????????????????????????????????????????");
  Debug.LogWarning(extraLog.ToString());
  return;
  }

        Material targetMaterial;
  string materialSource;

        if (string.IsNullOrEmpty(additiveName))
        {
     targetMaterial = displayConfig.extraEmptyMaterial;
    materialSource = "Empty (no additive)";
     }
        else
 {
            targetMaterial = GetAdditiveMaterial(additiveName, out string additiveMatch);
      materialSource = additiveMatch;
  }

   Material currentMaterial = extraDisplayAssignment.cachedRenderer.material;

        extraLog.AppendLine($"\n??? MATERIALS ???");
  extraLog.AppendLine($"  Current:  {(currentMaterial != null ? currentMaterial.name : "NULL")}");
    extraLog.AppendLine($"  Target:   {(targetMaterial != null ? targetMaterial.name : "NULL")}");
        extraLog.AppendLine($"  Source:   {materialSource}");

        extraDisplayAssignment.cachedRenderer.material = targetMaterial;

 extraLog.AppendLine($"\n??? RESULT ???");
   extraLog.AppendLine($"  Status: ? SUCCESS - Material applied");
   extraLog.AppendLine($"  Display now shows: {(string.IsNullOrEmpty(additiveName) ? "Empty" : additiveName)}");
   extraLog.AppendLine($"?????????????????????????????????????????????????????????????");

        Debug.Log(extraLog.ToString());
    }

    private Material GetAdditiveMaterial(string additiveName)
    {
        return GetAdditiveMaterial(additiveName, out _);
    }

    private Material GetAdditiveMaterial(string additiveName, out string matchType)
    {
        // Check for salt
        if (additiveName.Contains(displayConfig.saltIdentifier))
  {
            matchType = $"Salt (matches '{displayConfig.saltIdentifier}')";
      return displayConfig.saltMaterial;
     }
   // Check for sugar
  else if (additiveName.Contains(displayConfig.sugarIdentifier))
        {
  matchType = $"Sugar (matches '{displayConfig.sugarIdentifier}')";
      return displayConfig.sugarMaterial;
 }
   // Check for pepper
        else if (additiveName.Contains(displayConfig.pepperIdentifier))
 {
       matchType = $"Pepper (matches '{displayConfig.pepperIdentifier}')";
            return displayConfig.pepperMaterial;
  }

   // Default to empty if no match found
        matchType = $"No match found - using empty";
 if (showDebugLogs)
        {
  Debug.LogWarning($"[DisplayHandler] Additive '{additiveName}' did not match identifiers: '{displayConfig.saltIdentifier}', '{displayConfig.sugarIdentifier}', '{displayConfig.pepperIdentifier}'");
    }

        return displayConfig.extraEmptyMaterial;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Alias for RefreshDisplays() - for compatibility with MachineInputTrigger
    /// </summary>
    public void UpdateDisplay()
    {
        RefreshDisplays();
    }

    /// <summary>
    /// Manually refresh all displays based on current tracker state
    /// </summary>
    public void RefreshDisplays()
    {
        if (!ValidateReferences()) return;

        UpdateAllDisplays(actionTracker.LiquidName, actionTracker.CapsuleAName,
            actionTracker.CapsuleBName, actionTracker.AdditiveName);

        if (showDebugLogs)
        {
            Debug.Log("[DisplayHandler] Displays manually refreshed");
        }
    }

    /// <summary>
    /// Reset all displays to empty state
    /// </summary>
    public void ResetDisplays()
    {
        UpdateAllDisplays("", "", "", "");

        if (showDebugLogs)
        {
            Debug.Log("[DisplayHandler] All displays reset to empty");
        }
    }

    /// <summary>
    /// Recache renderers (useful if GameObjects change at runtime)
    /// </summary>
    public void RecacheRenderers()
    {
        CacheRenderersAndOrganize();

        if (showDebugLogs)
        {
            Debug.Log("[DisplayHandler] Renderers recached");
        }
    }

    #endregion
}

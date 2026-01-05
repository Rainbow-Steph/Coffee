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

        // Initial update
        UpdateAllDisplays(actionTracker.LiquidName, actionTracker.CapsuleAName, 
            actionTracker.CapsuleBName, actionTracker.AdditiveName);
    }

    private void OnDisable()
    {
        if (actionTracker != null)
        {
            actionTracker.onMachineContentsChanged -= OnMachineContentsChanged;
        }
    }

    private void CacheRenderersAndOrganize()
    {
        if (displayAssignments == null || displayAssignments.Length == 0)
        {
            Debug.LogWarning($"[{GetType().Name}] No display assignments configured!", this);
            return;
        }

        foreach (var assignment in displayAssignments)
        {
            if (assignment == null || assignment.displayGameObject == null)
                continue;

            // Cache the Renderer component
            assignment.cachedRenderer = assignment.displayGameObject.GetComponent<Renderer>();

            if (assignment.cachedRenderer == null)
            {
                Debug.LogWarning($"[{GetType().Name}] GameObject '{assignment.displayGameObject.name}' is missing a Renderer component!", this);
                continue;
            }

            // Organize into quick lookup references
            switch (assignment.displayType)
            {
                case DisplayType.WaterDisplay1:
                    waterDisplay1Assignment = assignment;
                    if (showDebugLogs)
                        Debug.Log($"[DisplayHandler] Configured Water Display 1: {assignment.displayGameObject.name}");
                    break;

                case DisplayType.WaterDisplay2:
                    waterDisplay2Assignment = assignment;
                    if (showDebugLogs)
                        Debug.Log($"[DisplayHandler] Configured Water Display 2: {assignment.displayGameObject.name}");
                    break;

                case DisplayType.WaterDisplay3:
                    waterDisplay3Assignment = assignment;
                    if (showDebugLogs)
                        Debug.Log($"[DisplayHandler] Configured Water Display 3: {assignment.displayGameObject.name}");
                    break;

                case DisplayType.CoffeeDisplay1:
                    coffeeDisplay1Assignment = assignment;
                    if (showDebugLogs)
                        Debug.Log($"[DisplayHandler] Configured Coffee Display 1: {assignment.displayGameObject.name}");
                    break;

                case DisplayType.CoffeeDisplay2:
                    coffeeDisplay2Assignment = assignment;
                    if (showDebugLogs)
                        Debug.Log($"[DisplayHandler] Configured Coffee Display 2: {assignment.displayGameObject.name}");
                    break;

                case DisplayType.ExtraDisplay:
                    extraDisplayAssignment = assignment;
                    if (showDebugLogs)
                        Debug.Log($"[DisplayHandler] Configured Extra Display: {assignment.displayGameObject.name}");
                    break;
            }
        }

        if (showDebugLogs)
        {
            Debug.Log($"[DisplayHandler] All displays configured. Total assignments: {displayAssignments.Length}");
        }
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

    private void UpdateAllDisplays(string liquidName, string capsuleAName, 
        string capsuleBName, string additiveName)
    {
        UpdateWaterDisplays(liquidName);
        UpdateCoffeeDisplays(capsuleAName, capsuleBName);
    }

    #region Water Display Updates

    private void UpdateWaterDisplays(string liquidName)
    {
        bool hasWater = !string.IsNullOrEmpty(liquidName);
        Material targetMaterial = hasWater ? displayConfig.waterFilledMaterial : displayConfig.waterEmptyMaterial;

        if (waterDisplay1Assignment?.cachedRenderer != null)
        {
            waterDisplay1Assignment.cachedRenderer.material = targetMaterial;
        }

        if (waterDisplay2Assignment?.cachedRenderer != null)
        {
            waterDisplay2Assignment.cachedRenderer.material = targetMaterial;
        }

        if (waterDisplay3Assignment?.cachedRenderer != null)
        {
            waterDisplay3Assignment.cachedRenderer.material = targetMaterial;
        }

        if (showDebugLogs)
        {
            Debug.Log($"[DisplayHandler] Water displays updated: {(hasWater ? "Filled" : "Empty")}");
        }
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
        if (assignment?.cachedRenderer == null) return;

        Material targetMaterial;

        if (string.IsNullOrEmpty(capsuleName))
        {
            targetMaterial = displayConfig.coffeeEmptyMaterial;
        }
        else
        {
            targetMaterial = GetCapsuleMaterial(capsuleName);
        }

        assignment.cachedRenderer.material = targetMaterial;

        if (showDebugLogs)
        {
            Debug.Log($"[DisplayHandler] {displayName} updated with capsule: {(string.IsNullOrEmpty(capsuleName) ? "Empty" : capsuleName)}");
        }
    }

    private Material GetCapsuleMaterial(string capsuleName)
    {
        // Check for red capsule
        if (capsuleName.Contains(displayConfig.redCapsuleIdentifier))
        {
            return displayConfig.redCapsuleMaterial;
        }
        // Check for blue capsule
        else if (capsuleName.Contains(displayConfig.blueCapsuleIdentifier))
        {
            return displayConfig.blueCapsuleMaterial;
        }
        // Check for black capsule
        else if (capsuleName.Contains(displayConfig.blackCapsuleIdentifier))
        {
            return displayConfig.blackCapsuleMaterial;
        }

        // Default to empty if no match found
        if (showDebugLogs)
        {
            Debug.LogWarning($"[DisplayHandler] Capsule '{capsuleName}' did not match any color identifier. Using empty material.");
        }

        return displayConfig.coffeeEmptyMaterial;
    }

    #endregion

    #region Extra Display Updates

    private void UpdateExtraDisplay(string additiveName)
    {
        if (extraDisplayAssignment?.cachedRenderer == null) return;

        Material targetMaterial;

        if (string.IsNullOrEmpty(additiveName))
        {
            targetMaterial = displayConfig.extraEmptyMaterial;
        }
        else
        {
            targetMaterial = GetAdditiveMaterial(additiveName);
        }

        extraDisplayAssignment.cachedRenderer.material = targetMaterial;

        if (showDebugLogs)
        {
            Debug.Log($"[DisplayHandler] Extra display updated with additive: {(string.IsNullOrEmpty(additiveName) ? "Empty" : additiveName)}");
        }
    }

    private Material GetAdditiveMaterial(string additiveName)
    {
        // Check for salt
        if (additiveName.Contains(displayConfig.saltIdentifier))
        {
            return displayConfig.saltMaterial;
        }
        // Check for sugar
        else if (additiveName.Contains(displayConfig.sugarIdentifier))
        {
            return displayConfig.sugarMaterial;
        }
        // Check for pepper
        else if (additiveName.Contains(displayConfig.pepperIdentifier))
        {
            return displayConfig.pepperMaterial;
        }

        // Default to empty if no match found
        if (showDebugLogs)
        {
            Debug.LogWarning($"[DisplayHandler] Additive '{additiveName}' did not match any identifier. Using empty material.");
        }

        return displayConfig.extraEmptyMaterial;
    }

    #endregion

    #region Public Methods

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

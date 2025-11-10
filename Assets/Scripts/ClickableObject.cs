using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component to add to objects that should react to being clicked by the CameraRaycaster.
/// Attach this to any GameObject with a Collider that you want to be clickable.
/// Supports standard color highlighting and outline-based highlighting.
/// Includes support for child object renderers.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ClickableObject : MonoBehaviour
{
    // Static variables to track currently held item across all instances
    private static ClickableObject currentlyHeldObject = null;

    /// <summary>
    /// Public static property to get the name of the currently held item (empty string if none)
    /// </summary>
    public static string HeldItemName
    {
        get
        {
            if (currentlyHeldObject != null)
            {
                return currentlyHeldObject.gameObject.name;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Public static property to check if any item is currently being held
    /// </summary>
    public static bool IsAnyItemHeld
    {
        get { return currentlyHeldObject != null; }
    }

    /// <summary>
    /// Public static method to get reference to the currently held object
    /// </summary>
    public static ClickableObject GetHeldObject()
    {
        return currentlyHeldObject;
    }

    // Highlight mode enum
    public enum HighlightMode
    {
        ColorChange,  // Standard color/emission change
        OutlineMaterial,    // Swap to outline material
        OutlineOverlay,     // Add outline material as overlay (keeps original material)
        Disabled       // No highlighting
    }

    [Header("Click Response")]
    [Tooltip("Event triggered when this object is clicked")]
    public UnityEvent onClickEvent;

    [Header("Visual Feedback")]
    [Tooltip("Change color on click (requires Renderer component)")]
    public bool changeColorOnClick = false;

    [Tooltip("Color to change to when clicked")]
    public Color clickColor = Color.yellow;

    [Tooltip("Duration to show the color change")]
    public float colorChangeDuration = 0.5f;

    [Header("Hover Highlight")]
    [Tooltip("Highlight mode: ColorChange (standard), OutlineMaterial (swap material), OutlineOverlay (add outline), or Disabled")]
    public HighlightMode highlightMode = HighlightMode.ColorChange;

    [Tooltip("Color to use for hover highlight (ColorChange mode only)")]
    public Color hoverColor = new Color(1f, 0.8f, 0.4f, 1f); // Light orange

    [Tooltip("Emission intensity for hover highlight (ColorChange mode only)")]
    [Range(0f, 5f)]
    public float hoverEmissionIntensity = 0.5f;

    [Tooltip("Use emission for hover effect (ColorChange mode only)")]
    public bool useEmission = true;

    [Header("Outline Highlight Settings")]
    [Tooltip("Material to use when hovering (OutlineMaterial/OutlineOverlay mode)")]
    public Material outlineMaterial;

    [Tooltip("Include child objects when applying outline")]
    public bool includeChildren = true;

    [Header("Float Behavior")]
    [Tooltip("Make object float in front of camera when clicked")]
    public bool floatOnClick = false;

    [Tooltip("Distance from camera to float at")]
    public float floatDistance = 2f;

    [Tooltip("Speed at which object moves to float position")]
    public float floatSpeed = 5f;

    [Tooltip("Speed at which object rotates while floating (degrees per second)")]
    public float rotationSpeed = 30f;

    [Tooltip("Rotation offset applied to the floating object (Euler angles)")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Offset from center of camera view (up/down/left/right)")]
    public Vector3 floatOffset = Vector3.zero;

    [Header("Audio Feedback")]
    [Tooltip("Play a sound on click (requires AudioSource component)")]
    public bool playSoundOnClick = false;

    [Tooltip("AudioClip to play when clicked")]
    public AudioClip clickSound;

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    public bool showDebugInfo = false;

    private Renderer[] allRenderers;  // All renderers including children
    private Material materialInstance;
    private Material originalMaterial;
    private Material[] originalMaterials;
    private Material[][] allOriginalMaterials;  // Store original materials for all renderers
    private Color originalColor;
    private Color originalEmissionColor;
    private AudioSource audioSource;
    private bool isChangingColor = false;
    private bool isHovering = false;
    private bool hasEmission = false;

    // Float behavior variables
    private bool isFloating = false;
    private Vector3 pickupPosition;
    private Quaternion pickupRotation;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Camera mainCamera;
    private Collider objectCollider;
    private Quaternion floatingRotationOffset;

    [Header("Action Tracking")]
    [Tooltip("Reference to the PlayerActionTracker scriptable object")]
    public PlayerActionTracker actionTracker;

    void Start()
    {
        // Get all renderers (this object + children if includeChildren is true)
        if (includeChildren)
        {
            allRenderers = GetComponentsInChildren<Renderer>();
        }
        else
        {
            Renderer singleRenderer = GetComponent<Renderer>();
            allRenderers = singleRenderer != null ? new Renderer[] { singleRenderer } : new Renderer[0];
        }

        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject on {gameObject.name}: Found {allRenderers.Length} renderer(s)");
        }

        // Setup for ColorChange mode
        if (highlightMode == HighlightMode.ColorChange && (changeColorOnClick || highlightMode != HighlightMode.Disabled))
        {
            if (allRenderers.Length > 0 && allRenderers[0] != null && allRenderers[0].material != null)
            {
                // Create a material instance to avoid modifying the shared material
                materialInstance = allRenderers[0].material;
                originalColor = materialInstance.color;

                // Check if material supports emission
                if (materialInstance.HasProperty("_EmissionColor"))
                {
                    hasEmission = true;
                    originalEmissionColor = materialInstance.GetColor("_EmissionColor");
                }

                // Enable emission keyword if using emission
                if (useEmission && hasEmission && highlightMode == HighlightMode.ColorChange)
                {
                    materialInstance.EnableKeyword("_EMISSION");
                }
            }
            else if (showDebugInfo)
            {
                Debug.LogWarning($"ClickableObject on {gameObject.name}: Visual feedback enabled but no Renderer found.");
            }
        }

        // Setup for OutlineMaterial/OutlineOverlay mode
        if (highlightMode == HighlightMode.OutlineMaterial || highlightMode == HighlightMode.OutlineOverlay)
        {
            if (allRenderers.Length > 0)
            {
                // Store original materials for all renderers
                allOriginalMaterials = new Material[allRenderers.Length][];

                for (int i = 0; i < allRenderers.Length; i++)
                {
                    if (allRenderers[i] != null)
                    {
                        allOriginalMaterials[i] = allRenderers[i].sharedMaterials;
                    }
                }

                // Store first renderer's materials for backward compatibility
                if (allRenderers[0] != null)
                {
                    originalMaterial = allRenderers[0].sharedMaterial;
                    originalMaterials = allRenderers[0].sharedMaterials;
                }

                if (showDebugInfo)
                {
                    Debug.Log($"ClickableObject on {gameObject.name}: Stored materials for {allRenderers.Length} renderer(s)");
                }
            }
            else
            {
                Debug.LogWarning($"ClickableObject on {gameObject.name}: Outline mode enabled but no Renderer found!");
            }

            if (outlineMaterial == null && showDebugInfo)
            {
                Debug.LogWarning($"ClickableObject on {gameObject.name}: Outline mode enabled but no outline material assigned!");
            }
        }

        // Get or add AudioSource if we need to play sounds
        if (playSoundOnClick)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
        }

        // Ensure the object has a collider
        objectCollider = GetComponent<Collider>();
        if (objectCollider == null)
        {
            Debug.LogError($"ClickableObject on {gameObject.name}: No Collider component found! This object cannot be clicked.");
        }

        // Store original transform information
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        // Find main camera
        mainCamera = Camera.main;
        if (mainCamera == null && floatOnClick)
        {
            Debug.LogWarning($"ClickableObject on {gameObject.name}: floatOnClick is enabled but no Main Camera found!");
        }
    }

    void Update()
    {
        // Check for right-click to return floating object
        if (floatOnClick && isFloating && Input.GetMouseButtonDown(1))
        {
            StopFloating();

            if (showDebugInfo)
            {
                Debug.Log($"ClickableObject: {gameObject.name} - Right-click detected, returning to pickup position");
            }
            return;
        }

        if (floatOnClick && isFloating && mainCamera != null)
        {
            // Calculate target position in front of camera
            Vector3 targetPosition = mainCamera.transform.position +
          mainCamera.transform.forward * floatDistance +
                    mainCamera.transform.TransformDirection(floatOffset);

            // Smoothly move to target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * floatSpeed);

            // Apply rotation on world Y axis (not local)
            Quaternion worldYRotation = Quaternion.Euler(0f, rotationSpeed * Time.time, 0f);
            Quaternion targetRotation = worldYRotation * floatingRotationOffset;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * floatSpeed);
        }
    }

    /// <summary>
    /// Called by CameraRaycaster when mouse enters this object
    /// </summary>
    public virtual void OnHoverEnter()
    {
        if (highlightMode == HighlightMode.Disabled || isHovering) return;

        isHovering = true;

        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject: Mouse entered {gameObject.name}");
        }

        // Handle ColorChange mode
        if (highlightMode == HighlightMode.ColorChange)
        {
            if (materialInstance != null && !isChangingColor)
            {
                // Apply hover color
                materialInstance.color = hoverColor;

                // Apply emission if enabled
                if (useEmission && hasEmission)
                {
                    Color emissionColor = hoverColor * hoverEmissionIntensity;
                    materialInstance.SetColor("_EmissionColor", emissionColor);
                }
            }
        }
        // Handle OutlineMaterial mode
        else if (highlightMode == HighlightMode.OutlineMaterial)
        {
            if (allRenderers != null && outlineMaterial != null)
            {
                // Apply outline material to all renderers
                for (int i = 0; i < allRenderers.Length; i++)
                {
                    if (allRenderers[i] != null)
                    {
                        allRenderers[i].material = outlineMaterial;
                    }
                }

                if (showDebugInfo)
                {
                    Debug.Log($"ClickableObject: {gameObject.name} - Outline material applied to {allRenderers.Length} renderer(s)");
                }
            }
        }
        // Handle OutlineOverlay mode
        else if (highlightMode == HighlightMode.OutlineOverlay)
        {
            if (allRenderers != null && outlineMaterial != null && allOriginalMaterials != null)
            {
                // Add outline material to all renderers
                for (int i = 0; i < allRenderers.Length; i++)
                {
                    if (allRenderers[i] != null && allOriginalMaterials[i] != null)
                    {
                        Material[] materials = new Material[allOriginalMaterials[i].Length + 1];
                        for (int j = 0; j < allOriginalMaterials[i].Length; j++)
                        {
                            materials[j] = allOriginalMaterials[i][j];
                        }
                        materials[materials.Length - 1] = outlineMaterial;

                        allRenderers[i].materials = materials;
                    }
                }

                if (showDebugInfo)
                {
                    Debug.Log($"ClickableObject: {gameObject.name} - Outline overlay applied to {allRenderers.Length} renderer(s)");
                }
            }
        }
    }

    /// <summary>
    /// Called by CameraRaycaster when mouse exits this object
    /// </summary>
    public virtual void OnHoverExit()
    {
        if (!isHovering) return;

        isHovering = false;

        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject: Mouse exited {gameObject.name}");
        }

        // Handle ColorChange mode
        if (highlightMode == HighlightMode.ColorChange)
        {
            if (materialInstance != null && !isChangingColor)
            {
                // Restore original color
                materialInstance.color = originalColor;

                // Restore original emission
                if (useEmission && hasEmission)
                {
                    materialInstance.SetColor("_EmissionColor", originalEmissionColor);
                }
            }
        }
        // Handle OutlineMaterial mode
        else if (highlightMode == HighlightMode.OutlineMaterial)
        {
            if (allRenderers != null && allOriginalMaterials != null)
            {
                // Restore original materials to all renderers
                for (int i = 0; i < allRenderers.Length; i++)
                {
                    if (allRenderers[i] != null && allOriginalMaterials[i] != null && allOriginalMaterials[i].Length > 0)
                    {
                        allRenderers[i].material = allOriginalMaterials[i][0];
                    }
                }

                if (showDebugInfo)
                {
                    Debug.Log($"ClickableObject: {gameObject.name} - Original material restored to {allRenderers.Length} renderer(s)");
                }
            }
        }
        // Handle OutlineOverlay mode
        else if (highlightMode == HighlightMode.OutlineOverlay)
        {
            if (allRenderers != null && allOriginalMaterials != null)
            {
                // Restore original materials arrays to all renderers
                for (int i = 0; i < allRenderers.Length; i++)
                {
                    if (allRenderers[i] != null && allOriginalMaterials[i] != null)
                    {
                        allRenderers[i].materials = allOriginalMaterials[i];
                    }
                }

                if (showDebugInfo)
                {
                    Debug.Log($"ClickableObject: {gameObject.name} - Original materials restored to {allRenderers.Length} renderer(s)");
                }
            }
        }
    }

    /// <summary>
    /// Called by CameraRaycaster when this object is clicked
    /// </summary>
    /// <param name="hit">The RaycastHit information from the raycast</param>
    public void OnClicked(RaycastHit hit)
    {
        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject: {gameObject.name} was clicked at position {hit.point}");
        }

        // Trigger the Unity Event
        onClickEvent?.Invoke();

        // Handle float behavior
        if (floatOnClick)
        {
            ToggleFloat();
        }

        // Visual feedback
        if (changeColorOnClick && materialInstance != null)
        {
            if (!isChangingColor)
            {
                StartCoroutine(ChangeColorTemporarily());
            }
        }

        // Audio feedback
        if (playSoundOnClick && audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Call the virtual method for override in derived classes
        OnClickedCustom(hit);
    }

    /// <summary>
    /// Toggles the floating state of the object
    /// </summary>
    private void ToggleFloat()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning($"ClickableObject on {gameObject.name}: Cannot float - no Main Camera found!");
            return;
        }

        if (!isFloating)
        {
            // Check if another item is already being held
            if (currentlyHeldObject != null && currentlyHeldObject != this)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"ClickableObject: Cannot pick up {gameObject.name} - {currentlyHeldObject.gameObject.name} is already being held!");
                }
                return;
            }

            // Start floating
            StartFloating();

            // Update action tracker if assigned
            if (actionTracker != null)
            {
                actionTracker.HeldItemName = gameObject.name;
            }
        }
        else
        {
            // Return to original position
            StopFloating();

            // Clear held item in action tracker if assigned
            if (actionTracker != null && actionTracker.HeldItemName == gameObject.name)
            {
                actionTracker.HeldItemName = "";
            }
        }
    }

    /// <summary>
    /// Starts the floating behavior
    /// </summary>
    private void StartFloating()
    {
        // Store current transform info as the "pickup point"
        pickupPosition = transform.position;
        pickupRotation = transform.rotation;
        originalParent = transform.parent;

        // Calculate the rotation offset to apply during floating
        floatingRotationOffset = Quaternion.Euler(rotationOffset);

        isFloating = true;

        // Register this object as the currently held item
        currentlyHeldObject = this;

        // Optionally disable collider while floating
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }

        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject: {gameObject.name} started floating from position {pickupPosition}. Currently held: {HeldItemName}");
        }
    }

    private void StopFloating()
    {
        isFloating = false;

        // Unregister this object as the currently held item
        if (currentlyHeldObject == this)
        {
            currentlyHeldObject = null;

            // Clear held item in action tracker if assigned
            if (actionTracker != null && actionTracker.HeldItemName == gameObject.name)
            {
                actionTracker.HeldItemName = "";
            }
        }

        // Start coroutine to smoothly return to original position
        StartCoroutine(ReturnToOriginalPosition());

        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject: {gameObject.name} returning to original position. Currently held: {HeldItemName}");
        }
    }

    /// <summary>
    /// Coroutine to smoothly return object to its original position
    /// </summary>
    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        // Restore parent
        transform.SetParent(originalParent);

        float returnSpeed = floatSpeed;
        float rotationReturnSpeed = floatSpeed * 2f;

        // Smoothly move back to pickup position and rotation
        while (Vector3.Distance(transform.position, pickupPosition) > 0.01f ||
Quaternion.Angle(transform.rotation, pickupRotation) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, pickupPosition, Time.deltaTime * returnSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, pickupRotation, Time.deltaTime * rotationReturnSpeed);
            yield return null;
        }

        // Snap to exact pickup transform
        transform.position = pickupPosition;
        transform.rotation = pickupRotation;

        // Re-enable collider
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }

        if (showDebugInfo)
        {
            Debug.Log($"ClickableObject: {gameObject.name} returned to pickup position {pickupPosition}");
        }
    }

    /// <summary>
    /// Virtual method that can be overridden in derived classes for custom behavior
    /// </summary>
    protected virtual void OnClickedCustom(RaycastHit hit)
    {
        // Override this method in derived classes to add custom behavior
    }

    /// <summary>
    /// Coroutine to temporarily change the object's color
    /// </summary>
    private System.Collections.IEnumerator ChangeColorTemporarily()
    {
        isChangingColor = true;

        if (materialInstance != null)
        {
            Color previousColor = materialInstance.color;
            materialInstance.color = clickColor;
            yield return new WaitForSeconds(colorChangeDuration);

            // Restore to hover color if still hovering, otherwise original color
            if (isHovering && highlightMode != HighlightMode.Disabled)
            {
                materialInstance.color = hoverColor;
                if (useEmission && hasEmission)
                {
                    Color emissionColor = hoverColor * hoverEmissionIntensity;
                    materialInstance.SetColor("_EmissionColor", emissionColor);
                }
            }
            else
            {
                materialInstance.color = originalColor;
                if (useEmission && hasEmission)
                {
                    materialInstance.SetColor("_EmissionColor", originalEmissionColor);
                }
            }
        }

        isChangingColor = false;
    }

    /// <summary>
    /// Public method to manually set outline state
    /// </summary>
    public void SetOutlineActive(bool active)
    {
        if (highlightMode != HighlightMode.OutlineMaterial && highlightMode != HighlightMode.OutlineOverlay) return;

        if (active)
        {
            OnHoverEnter();
        }
        else
        {
            OnHoverExit();
        }
    }

    /// <summary>
    /// Public method to simulate a click programmatically
    /// </summary>
    public void SimulateClick()
    {
        RaycastHit simulatedHit = new RaycastHit();
        OnClicked(simulatedHit);
    }

    /// <summary>
    /// Public method to force return to original position
    /// </summary>
    public void ForceReturnToOriginal()
    {
        if (isFloating)
        {
            StopFloating();
        }
    }

    /// <summary>
    /// Clean up resources when object is destroyed
    /// </summary>
    protected virtual void OnDestroy()
    {
        // Clean up material instance (ColorChange mode)
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }

        // Clean up any extra material instances (OutlineMaterial/OutlineOverlay mode)
        if ((highlightMode == HighlightMode.OutlineMaterial || highlightMode == HighlightMode.OutlineOverlay) && allRenderers != null)
        {
            foreach (Renderer rend in allRenderers)
            {
                if (rend != null)
                {
                    Material[] currentMaterials = rend.materials;
                    foreach (Material mat in currentMaterials)
                    {
                        bool isOriginal = false;

                        // Check against all stored original materials
                        if (allOriginalMaterials != null)
                        {
                            foreach (Material[] origMats in allOriginalMaterials)
                            {
                                if (origMats != null)
                                {
                                    foreach (Material origMat in origMats)
                                    {
                                        if (mat == origMat)
                                        {
                                            isOriginal = true;
                                            break;
                                        }
                                    }
                                }
                                if (isOriginal) break;
                            }
                        }

                        if (!isOriginal && mat != outlineMaterial)
                        {
                            Destroy(mat);
                        }
                    }
                }
            }

            // Clear the held object reference if this object is being destroyed while held
            if (currentlyHeldObject == this)
            {
                currentlyHeldObject = null;
            }
        }
    }
}


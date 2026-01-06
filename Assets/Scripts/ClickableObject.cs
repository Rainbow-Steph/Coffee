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

    [Header("Pick Up Behavior")]
    [Tooltip("Allow picking up this object by holding left mouse button")]
    public bool canPickUp = false;

    [Tooltip("Distance from camera to hold object at")]
    public float holdDistance = 2f;

    [Tooltip("Speed at which object moves to hold position")]
    public float pickupSpeed = 5f;

    [Tooltip("Speed at which object rotates while being held (degrees per second)")]
    public float rotationSpeed = 30f;

    [Tooltip("Rotation offset applied to the held object (Euler angles)")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Offset from center of camera view (up/down/left/right)")]
    public Vector3 holdOffset = Vector3.zero;

    [Tooltip("Percentage of velocity to retain when released (0 = no momentum, 1 = full momentum)")]
    [Range(0f, 1f)]
    public float momentumRetention = 0.7f;

    [Tooltip("Keep collisions enabled while holding (allows pushing other objects)")]
    public bool keepCollisionsWhileHeld = true;

    [Tooltip("Use physics-based holding (collisions can push object away from target position)")]
    public bool usePhysicsHolding = true;

    [Tooltip("Force applied to pull object toward hold position (higher = stronger pull)")]
    [Range(1f, 100f)]
    public float holdForce = 20f;

    [Tooltip("Smoothing factor for held object movement (higher = smoother but slightly more lag)")]
    [Range(1f, 30f)]
    public float holdSmoothing = 15f;

    [Tooltip("Use kinematic mode while held to eliminate physics jitter")]
    public bool useKinematicWhileHeld = true;

    [Tooltip("Use continuous collision detection to prevent tunneling through objects")]
    public bool useContinuousCollision = true;

    [Tooltip("Use ContinuousSpeculative collision for kinematic objects (allows better collision response)")]
    public bool useContinuousSpeculativeForKinematic = true;

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

    // Pick up behavior variables
    private bool isBeingHeld = false;
    private bool isHoldingMouseButton = false;
    private Vector3 pickupPosition;
    private Quaternion pickupRotation;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Camera mainCamera;
    private Collider objectCollider;
    private Quaternion holdingRotationOffset;
    private Rigidbody objectRigidbody;
    private bool originalUseGravity;
    private Vector3 lastPosition;
    private Vector3 currentVelocity;
  private float actualHoldDistance; // Stores the distance when picked up
    private bool wasKinematic; // Store original kinematic state
    private Vector3 smoothedPosition; // For smooth interpolation
private Quaternion smoothedRotation; // For smooth rotation
 private CollisionDetectionMode originalCollisionMode; // Store original collision detection mode
 private RigidbodyInterpolation originalInterpolation; // Store original interpolation mode
    private Vector3 lastValidPosition; // Store last position before collision
    private bool hitObstacle; // Track if we hit something

    [Header("Action Tracking")]
    [Tooltip("Reference to the PlayerActionTracker scriptable object")]
    public PlayerActionTracker actionTracker;

    [Header("Item Settings")]
    [Tooltip("Type of this item")]
    public ItemType itemType = ItemType.Prop;

    [Header("Interaction")]
    [Tooltip("Handler responsible for cross-object interactions (optional)")]
    public ItemInteractionHandler interactionHandler;

    [Header("Coffee Making")]
    [Tooltip("Optional CoffeeMaker component to call when clicked")]
    public CoffeeMaker coffeeMaker;

    [Header("Billboard")]
    [Tooltip("Enable billboard label for this object")]
    public bool enableBillboard = false;

    [Tooltip("Billboard prefab (must have a BillboardController component)")]
    public BillboardController billboardPrefab;

    // Runtime instance (created from prefab)
    private BillboardController billboardInstance;

    [Tooltip("Show billboard when hovering over this object")]
    public bool billboardShowOnHover = false;

    [Tooltip("Show/hide billboard when clicking this object")]
    public bool billboardShowOnClick = false;

    [Tooltip("Inventory Manager ScriptableObject to read values from")]
    public InventoryManager inventoryManager;

    [Tooltip("Which inventory field to display on the billboard")]
    public InventoryManager.InventoryField billboardInventoryField = InventoryManager.InventoryField.Money;

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

        // Get Rigidbody if it exists (for gravity control during pickup)
        objectRigidbody = GetComponent<Rigidbody>();
        if (objectRigidbody != null && canPickUp)
        {
            originalUseGravity = objectRigidbody.useGravity;
            
            if (showDebugInfo)
            {
                Debug.Log($"ClickableObject on {gameObject.name}: Found Rigidbody, gravity will be disabled during pickup");
            }
        }

        // Store original transform information
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        // Find main camera
        mainCamera = Camera.main;
        if (mainCamera == null && canPickUp)
        {
            Debug.LogWarning($"ClickableObject on {gameObject.name}: canPickUp is enabled but no Main Camera found!");
        }

        // Instantiate billboard if enabled
        if (enableBillboard && billboardPrefab != null)
        {
            billboardInstance = Instantiate(billboardPrefab, transform.position, Quaternion.identity);
            // Use SetText to display the object name on the billboard
            if (billboardInstance != null)
            {
                billboardInstance.SetText(gameObject.name);
                billboardInstance.gameObject.SetActive(false); // Hide billboard by default
            }
        }
    }

    void Update()
    {
        // Handle pick up behavior with hold-to-lift mechanic
        if (canPickUp)
        {
     // Check if mouse button is being released while holding this object
   if (isBeingHeld && !Input.GetMouseButton(0))
   {
  // Mouse button released - drop the object
       ReleaseObject();

   if (showDebugInfo)
      {
 Debug.Log($"ClickableObject: {gameObject.name} - Left mouse released, dropping object");
      }
       return;
    }

// Update object position while being held
 if (isBeingHeld && mainCamera != null)
        {
             // Calculate target position using the stored actualHoldDistance
     Vector3 targetPosition = mainCamera.transform.position +
   mainCamera.transform.forward * actualHoldDistance +
        mainCamera.transform.TransformDirection(holdOffset);

  // Calculate target rotation
  Quaternion worldYRotation = Quaternion.Euler(0f, rotationSpeed * Time.time, 0f);
      Quaternion targetRotation = worldYRotation * holdingRotationOffset;

    if (usePhysicsHolding && objectRigidbody != null && !useKinematicWhileHeld)
      {
   // Physics-based holding: use forces to pull object toward target
         // This respects collisions and prevents tunneling
  Vector3 directionToTarget = targetPosition - transform.position;
       float distanceToTarget = directionToTarget.magnitude;
        
      // Apply force proportional to distance (spring-like behavior)
    Vector3 force = directionToTarget.normalized * holdForce * distanceToTarget;
           objectRigidbody.AddForce(force, ForceMode.Force);
      
      // Add damping to prevent oscillation
     objectRigidbody.velocity *= 0.95f;
     
    // Update actual hold distance based on current position
  Vector3 cameraToObject = transform.position - mainCamera.transform.position;
             float currentDistance = Vector3.Dot(cameraToObject, mainCamera.transform.forward);
       
              if (showDebugInfo && Time.frameCount % 30 == 0)
{
   Debug.Log($"ClickableObject: {gameObject.name} - Target distance: {actualHoldDistance:F2}, Current distance: {currentDistance:F2}, Force: {force.magnitude:F2}");
    }
       }
      else
   {
     // Smooth kinematic/direct control (eliminates jitter)
        // Use exponential smoothing for very smooth movement
   float smoothFactor = Time.deltaTime * holdSmoothing;
     Vector3 targetSmoothedPosition = Vector3.Lerp(smoothedPosition, targetPosition, smoothFactor);
       Quaternion targetSmoothedRotation = Quaternion.Slerp(smoothedRotation, targetRotation, smoothFactor);
   
    // Check if we can move to target position (collision check for kinematic)
    bool canMoveTo = true;
     if (objectRigidbody != null && useKinematicWhileHeld && keepCollisionsWhileHeld)
{
     // Use raycast or sphere cast to check if path is clear
      Vector3 moveDirection = targetSmoothedPosition - transform.position;
     float moveDistance = moveDirection.magnitude;

            if (moveDistance > 0.001f && objectCollider != null)
      {
     // Cast a sphere to check for obstacles (IGNORE TRIGGERS)
      RaycastHit hitInfo;
 float checkRadius = objectCollider.bounds.extents.magnitude * 0.5f;
    
      if (Physics.SphereCast(transform.position, checkRadius, moveDirection.normalized, out hitInfo, moveDistance, ~0, QueryTriggerInteraction.Ignore))
         {
         // Hit something - don't move through it (but ignore triggers)
   canMoveTo = false;
   hitObstacle = true;
    
            // Store the position we can reach (just before the obstacle)
   float safeDistance = Mathf.Max(0, hitInfo.distance - checkRadius * 0.1f);
     targetSmoothedPosition = transform.position + moveDirection.normalized * safeDistance;
     
      if (showDebugInfo && Time.frameCount % 30 == 0)
   {
    Debug.Log($"ClickableObject: {gameObject.name} - BLOCKED by {hitInfo.collider.gameObject.name}, stopping at safe distance");
    }
    }
     else
 {
        hitObstacle = false;
        }
  }
 }
   
    // Apply movement (either to target or to collision point)
    smoothedPosition = targetSmoothedPosition;
       smoothedRotation = targetSmoothedRotation;
   
      if (objectRigidbody != null && useKinematicWhileHeld)
  {
       // Use MovePosition for kinematic rigidbodies (physics-aware, prevents tunneling)
       objectRigidbody.MovePosition(smoothedPosition);
       objectRigidbody.MoveRotation(smoothedRotation);
     }
  else
      {
  // Direct transform control (least collision-safe, but works without Rigidbody)
  transform.position = smoothedPosition;
      transform.rotation = smoothedRotation;
      }
          }

    // Store position for velocity calculation BEFORE update
   Vector3 positionBeforeUpdate = transform.position;
       
                // Calculate velocity for momentum based on actual movement
  currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
  
  // Update lastPosition for next frame
    lastPosition = transform.position;
     }
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

        // Show billboard if configured
        if (enableBillboard && billboardShowOnHover)
        {
            ShowBillboard();
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
        if (materialInstance != null && !isChangingColor && highlightMode == HighlightMode.ColorChange)
        {
            // Restore original color
            materialInstance.color = originalColor;

            // Restore original emission
            if (useEmission && hasEmission)
            {
                materialInstance.SetColor("_EmissionColor", originalEmissionColor);
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

        // Hide billboard if configured
        if (enableBillboard && billboardShowOnHover)
        {
            HideBillboard();
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

        // Handle interactions if we have an interaction handler
        if (interactionHandler != null && IsAnyItemHeld)
        {
            interactionHandler.HandleInteraction(this, GetHeldObject());
        }

        // Handle coffee making if coffee maker is assigned
        if (coffeeMaker != null)
        {
            coffeeMaker.MakeCoffee();
        }

        // Trigger the Unity Event
        onClickEvent?.Invoke();

        // Toggle billboard on click if configured
        if (enableBillboard && billboardShowOnClick)
        {
            if (billboardInstance == null || !billboardInstance.gameObject.activeSelf)
            {
                ShowBillboard();
            }
            else
            {
                HideBillboard();
            }
        }

        // Handle pick-up behavior
        if (canPickUp)
        {
            TogglePickUp();
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

        // Show/hide billboard on click (update content if shown)
        if (enableBillboard && billboardInstance != null)
        {
            bool shouldShow = billboardShowOnClick && !billboardInstance.gameObject.activeSelf;
            billboardInstance.gameObject.SetActive(shouldShow);

            // Update the inventory display on the billboard if active
            if (shouldShow && inventoryManager != null)
            {
                UpdateBillboardInventory();
            }
        }

        // Handle pick up behavior - start holding when clicked
        if (canPickUp && !isBeingHeld)
        {
            StartHoldingObject();
        }

        // Call the virtual method for override in derived classes
        OnClickedCustom(hit);
    }

    /// <summary>
    /// Toggles the pick-up state of the object
    /// </summary>
    private void TogglePickUp()
    {
        if (mainCamera == null)
        {
         Debug.LogWarning($"ClickableObject on {gameObject.name}: Cannot pick up - no Main Camera found!");
            return;
     }

      if (!isBeingHeld)
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

// Start holding object
            StartHoldingObject();

      // Update action tracker if assigned
            if (actionTracker != null)
  {
      actionTracker.HeldItemName = gameObject.name;
        }
        }
     else
        {
    // Release object
            ReleaseObject();

// Clear held item in action tracker if assigned
            if (actionTracker != null && actionTracker.HeldItemName == gameObject.name)
            {
 actionTracker.HeldItemName = "";
        }
        }
    }

    /// <summary>
    /// Starts holding the object (called when clicked)
    /// </summary>
    private void StartHoldingObject()
    {
     if (mainCamera == null)
{
      Debug.LogWarning($"ClickableObject on {gameObject.name}: Cannot pick up - no Main Camera found!");
     return;
 }

 // Check if another item is already being held
      if (currentlyHeldObject != null && currentlyHeldObject != this)
  {
  if (showDebugInfo)
  {
 Debug.Log($"ClickableObject: Cannot pick up {gameObject.name} - {currentlyHeldObject.gameObject.name} is already being held!");
    }
      return;
   }

    // Store current transform info as the "pickup point"
      pickupPosition = transform.position;
      pickupRotation = transform.rotation;
      originalParent = transform.parent;

     // Calculate and store the actual distance from camera at pickup time
    Vector3 cameraToObject = transform.position - mainCamera.transform.position;
        actualHoldDistance = Vector3.Dot(cameraToObject, mainCamera.transform.forward);
        
   // Clamp to reasonable values (use holdDistance as max limit)
actualHoldDistance = Mathf.Clamp(actualHoldDistance, 0.5f, holdDistance);
     
        if (showDebugInfo)
      {
     Debug.Log($"ClickableObject: {gameObject.name} - Calculated hold distance: {actualHoldDistance:F2} (max: {holdDistance})");
        }

   // Calculate the rotation offset to apply while holding
holdingRotationOffset = Quaternion.Euler(rotationOffset);

  isBeingHeld = true;
    isHoldingMouseButton = true;

        // Register this object as the currently held item
     currentlyHeldObject = this;

  // Handle collider based on keepCollisionsWhileHeld setting
        if (objectCollider != null && !keepCollisionsWhileHeld)
   {
 objectCollider.enabled = false;
        
   if (showDebugInfo)
    {
     Debug.Log($"ClickableObject: {gameObject.name} - Collider disabled");
  }
   }
        else if (showDebugInfo && keepCollisionsWhileHeld)
        {
 Debug.Log($"ClickableObject: {gameObject.name} - Collisions kept enabled for pushing other objects");
      }

  // IMPORTANT: Always keep collider enabled if we want collision response
 // Even in kinematic mode, colliders must be enabled for collision detection
  if (keepCollisionsWhileHeld && objectCollider != null)
  {
   objectCollider.enabled = true;
        
        if (showDebugInfo)
        {
     Debug.Log($"ClickableObject: {gameObject.name} - Collider FORCED ENABLED for collision response");
        }
    }

  // Disable gravity while being held
if (objectRigidbody != null)
    {
     originalUseGravity = objectRigidbody.useGravity;
  objectRigidbody.useGravity = false;
      
   // Store and set kinematic state for smooth holding
  wasKinematic = objectRigidbody.isKinematic;
   
       // Store original collision detection and interpolation modes
 originalCollisionMode = objectRigidbody.collisionDetectionMode;
        originalInterpolation = objectRigidbody.interpolation;
      
    // Set collision detection based on kinematic state
   if (useKinematicWhileHeld && !usePhysicsHolding)
      {
     // Set kinematic mode
     objectRigidbody.isKinematic = true;

            // Use ContinuousSpeculative for kinematic (better collision detection)
            if (useContinuousSpeculativeForKinematic)
            {
  objectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
   }
}
      else if (useContinuousCollision && !wasKinematic)
      {
    // Dynamic mode with continuous collision
     objectRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
 }
    
  // Enable interpolation for smoother physics
  if (!wasKinematic)
   {
   objectRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
  
  // Only zero velocities if NOT using physics holding
   if (!usePhysicsHolding)
    {
   objectRigidbody.velocity = Vector3.zero;
objectRigidbody.angularVelocity = Vector3.zero;
  }
     
  if (showDebugInfo)
{
    Debug.Log($"ClickableObject: {gameObject.name} - Gravity disabled" + 
    (useKinematicWhileHeld && !usePhysicsHolding ? ", kinematic mode" : "") +
      (usePhysicsHolding ? ", physics holding enabled" : ", velocities zeroed") +
 (objectRigidbody.collisionDetectionMode == CollisionDetectionMode.ContinuousSpeculative ? ", continuous speculative collision" : 
    objectRigidbody.collisionDetectionMode == CollisionDetectionMode.ContinuousDynamic ? ", continuous dynamic collision" : ", discrete collision"));
   }
      }

   // Initialize smooth position/rotation to current values
        smoothedPosition = transform.position;
     smoothedRotation = transform.rotation;

   // Initialize velocity tracking
  lastPosition = transform.position;
     currentVelocity = Vector3.zero;
    
        // Initialize collision tracking
    lastValidPosition = transform.position;
        hitObstacle = false;
    }

  /// <summary>
    /// Releases the held object (called when mouse button is released)
    /// </summary>
    private void ReleaseObject()
    {
  isBeingHeld = false;
   isHoldingMouseButton = false;

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

      // Re-enable gravity and restore physics settings when dropped
   if (objectRigidbody != null)
 {
  objectRigidbody.useGravity = originalUseGravity;
      objectRigidbody.isKinematic = wasKinematic;
      
// Restore original collision detection and interpolation modes
      objectRigidbody.collisionDetectionMode = originalCollisionMode;
  objectRigidbody.interpolation = originalInterpolation;
   
       // Apply retained momentum based on momentumRetention setting
   // IMPORTANT: Apply momentum AFTER restoring physics settings
   if (momentumRetention > 0f && !wasKinematic)
   {
       // Calculate final velocity including any smoothing
  // Use the tracked velocity from actual movement
     Vector3 finalVelocity = currentVelocity * momentumRetention;
     
       // Apply the momentum to the rigidbody
  objectRigidbody.velocity = finalVelocity;
     
 if (showDebugInfo)
       {
    Debug.Log($"ClickableObject: {gameObject.name} - Applied momentum: {finalVelocity} (retention: {momentumRetention}, magnitude: {finalVelocity.magnitude:F2})");
  }
     }
      else if (showDebugInfo)
   {
   Debug.Log($"ClickableObject: {gameObject.name} - No momentum applied (retention: {momentumRetention}, wasKinematic: {wasKinematic})");
   }
        
        // IMPORTANT: Ensure object will fall if not kinematic
   // Add a small downward velocity to overcome any floating caused by collision
        if (!wasKinematic && originalUseGravity)
     {
       // Check if object has no significant velocity (might be floating)
   if (objectRigidbody.velocity.magnitude < 0.1f)
   {
      // Give it a small downward nudge to start falling
    objectRigidbody.velocity += Vector3.down * 0.1f;
     
     if (showDebugInfo)
         {
          Debug.Log($"ClickableObject: {gameObject.name} - Added downward nudge to prevent floating");
      }
  }
     }
    
  if (showDebugInfo)
   {
     Debug.Log($"ClickableObject: {gameObject.name} - Gravity restored to: {originalUseGravity}, Kinematic: {wasKinematic}, CollisionMode: {originalCollisionMode}");
 }
}

// Re-enable collider if it was disabled
      if (objectCollider != null && !keepCollisionsWhileHeld)
   {
    objectCollider.enabled = true;
  
     if (showDebugInfo)
  {
      Debug.Log($"ClickableObject: {gameObject.name} - Collider re-enabled");
    }
     }

        if (showDebugInfo)
 {
   Debug.Log($"ClickableObject: {gameObject.name} released at current position. Currently held: {HeldItemName}");
     }
    }

    void LateUpdate()
    {
        // Auto-release object if being held and mouse button is released
        if (isBeingHeld && isHoldingMouseButton && Input.GetMouseButtonUp(0))
        {
            ReleaseObject();
        }
    }

    /// <summary>
    /// Detect collisions while object is being held
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (isBeingHeld && keepCollisionsWhileHeld)
 {
            // Ignore trigger colliders
      if (collision.collider.isTrigger)
   return;
       
    hitObstacle = true;
      
  if (showDebugInfo)
   {
Debug.Log($"ClickableObject: {gameObject.name} collided with {collision.gameObject.name} while held!");
   }
}
    }

    /// <summary>
    /// Continue tracking collision contact
    /// </summary>
  void OnCollisionStay(Collision collision)
    {
        if (isBeingHeld && keepCollisionsWhileHeld)
      {
          // Ignore trigger colliders
if (collision.collider.isTrigger)
             return;
            
   hitObstacle = true;
  }
  }

  /// <summary>
    /// Track when collision ends
    /// </summary>
    void OnCollisionExit(Collision collision)
    {
        if (isBeingHeld && keepCollisionsWhileHeld)
 {
     // Ignore trigger colliders
         if (collision.collider.isTrigger)
          return;
 
hitObstacle = false;
   
 if (showDebugInfo)
    {
      Debug.Log($"ClickableObject: {gameObject.name} stopped colliding with {collision.gameObject.name}");
  }
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
    /// Public method to force release at current position
    /// </summary>
    public void ForceReturnToOriginal()
  {
      if (isBeingHeld)
      {
    ReleaseObject();
 }
    }

    #region Billboard Support

    private void ShowBillboard()
    {
        if (!enableBillboard || billboardPrefab == null) return;

        if (billboardInstance == null)
        {
            // Instantiate billboard prefab in the scene root
            billboardInstance = Instantiate(billboardPrefab);
            // Keep it inactive until configured
            billboardInstance.gameObject.SetActive(false);
        }

        // Update the billboard text from the inventory manager (if assigned)
        string text = "";
        if (inventoryManager != null)
        {
            text = inventoryManager.GetFieldDisplay(billboardInventoryField);
            // Subscribe to inventory updates so the billboard text stays current
            inventoryManager.onInventoryChanged -= OnInventoryChanged; // avoid double-subscribe
            inventoryManager.onInventoryChanged += OnInventoryChanged;
        }

        billboardInstance.SetText(text);
        billboardInstance.SetTarget(transform);
        billboardInstance.Show(transform, text);
    }

    private void HideBillboard()
    {
        if (billboardInstance != null)
        {
            billboardInstance.Hide();
        }

        if (inventoryManager != null)
        {
            inventoryManager.onInventoryChanged -= OnInventoryChanged;
        }
    }

    private void OnInventoryChanged()
    {
        if (billboardInstance != null && inventoryManager != null)
        {
            billboardInstance.SetText(inventoryManager.GetFieldDisplay(billboardInventoryField));
        }
    }

    #endregion

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
        }

        // Clear the held object reference if this object is being destroyed while held
        if (currentlyHeldObject == this)
        {
            currentlyHeldObject = null;

            // Clear held item in action tracker if assigned
            if (actionTracker != null && actionTracker.HeldItemName == gameObject.name)
            {
                actionTracker.HeldItemName = "";
            }
        }

        // Cleanup billboard instance if any
        if (billboardInstance != null)
        {
            if (inventoryManager != null)
            {
                inventoryManager.onInventoryChanged -= OnInventoryChanged;
            }

            Destroy(billboardInstance.gameObject);
            billboardInstance = null;
        }
    }

    /// <summary>
    /// Public method to update the inventory display on the billboard
    /// </summary>
    public void UpdateBillboardInventory()
    {
        if (billboardInstance != null && inventoryManager != null)
        {
            string displayText = inventoryManager.GetFieldDisplay(billboardInventoryField);

            // Update the billboard with the new text
            billboardInstance.SetText(displayText);
        }
    }
}


using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component to add to objects that should react to being clicked by the CameraRaycaster.
/// Attach this to any GameObject with a Collider that you want to be clickable.
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
    [Tooltip("Highlight object while mouse is hovering over it")]
    public bool highlightOnHover = true;
    
    [Tooltip("Color to use for hover highlight")]
    public Color hoverColor = new Color(1f, 0.8f, 0.4f, 1f); // Light orange
    
    [Tooltip("Emission intensity for hover highlight (makes it glow)")]
    [Range(0f, 5f)]
    public float hoverEmissionIntensity = 0.5f;
    
    [Tooltip("Use emission for hover effect (makes object glow)")]
    public bool useEmission = true;
 
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
    
    private Renderer objectRenderer;
    private Material materialInstance;
    private Color originalColor;
    private Color originalEmissionColor;
    private AudioSource audioSource;
    private bool isChangingColor = false;
    private bool isHovering = false;
    private bool hasEmission = false;
    
    // Float behavior variables
    private bool isFloating = false;
    private Vector3 pickupPosition;  // Position when object was picked up
    private Quaternion pickupRotation;  // Rotation when object was picked up
    private Vector3 originalPosition;  // Original spawn position
    private Quaternion originalRotation;  // Original spawn rotation
  private Transform originalParent;
    private Camera mainCamera;
private Collider objectCollider;
    private Quaternion floatingRotationOffset;  // Applied rotation offset during float
    
    void Start()
    {
   // Get the Renderer component if we need to change colors
        if (changeColorOnClick || highlightOnHover)
        {
 objectRenderer = GetComponent<Renderer>();
          if (objectRenderer != null && objectRenderer.material != null)
          {
     // Create a material instance to avoid modifying the shared material
           materialInstance = objectRenderer.material;
 originalColor = materialInstance.color;
    
                // Check if material supports emission
   if (materialInstance.HasProperty("_EmissionColor"))
    {
    hasEmission = true;
           originalEmissionColor = materialInstance.GetColor("_EmissionColor");
   }
      
         // Enable emission keyword if using emission
    if (useEmission && hasEmission && highlightOnHover)
        {
       materialInstance.EnableKeyword("_EMISSION");
  }
       }
     else if (showDebugInfo)
        {
      Debug.LogWarning($"ClickableObject on {gameObject.name}: Visual feedback enabled but no Renderer found.");
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
            // Create a rotation that rotates around world Y axis
            Quaternion worldYRotation = Quaternion.Euler(0f, rotationSpeed * Time.time, 0f);
       // Combine with the rotation offset
    Quaternion targetRotation = worldYRotation * floatingRotationOffset;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * floatSpeed);
        }
    }
    
    /// <summary>
    /// Called by CameraRaycaster when mouse enters this object
    /// </summary>
    public virtual void OnHoverEnter()
    {
        if (!highlightOnHover || isHovering) return;

        isHovering = true;
        
    if (showDebugInfo)
        {
      Debug.Log($"ClickableObject: Mouse entered {gameObject.name}");
  }
      
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
       return; // Don't pick up if another item is held
       }
            
            // Start floating
            StartFloating();
        }
        else
        {
         // Return to original position
        StopFloating();
        }
    }
    
    /// <summary>
    /// Starts the floating behavior
    /// </summary>
    private void StartFloating()
    {
        // Store current transform info as the "pickup point" (where it was when clicked)
        pickupPosition = transform.position;
        pickupRotation = transform.rotation;
 originalParent = transform.parent;
 
        // Calculate the rotation offset to apply during floating
        floatingRotationOffset = Quaternion.Euler(rotationOffset);
 
        isFloating = true;
        
   // Register this object as the currently held item
        currentlyHeldObject = this;
   
        // Optionally disable collider while floating to avoid weird interactions
        if (objectCollider != null)
        {
        objectCollider.enabled = false;
        }

        if (showDebugInfo)
{
        Debug.Log($"ClickableObject: {gameObject.name} started floating from position {pickupPosition}. Currently held: {HeldItemName}");
   }
    }
    
    /// <summary>
 /// Stops the floating behavior and returns object to original position
    /// </summary>
    private void StopFloating()
    {
        isFloating = false;
        
        // Unregister this object as the currently held item
     if (currentlyHeldObject == this)
    {
     currentlyHeldObject = null;
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
        
        // Smoothly move back to pickup position and rotation (not original spawn position)
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
 if (isHovering && highlightOnHover)
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
    // Clean up material instance
   if (materialInstance != null)
        {
         Destroy(materialInstance);
 }
    
        // Clear the held object reference if this object is being destroyed while held
        if (currentlyHeldObject == this)
        {
    currentlyHeldObject = null;
   }
    }
}

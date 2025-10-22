using UnityEngine;

/// <summary>
/// Handles raycasting from the center of the camera's frustum on mouse click.
/// Attach this to your main camera.
/// </summary>
public class CameraRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Maximum distance the ray will travel")]
    public float maxRayDistance = 100f;
    
    [Tooltip("Layer mask to filter what objects can be clicked")]
    public LayerMask clickableLayerMask = ~0; // All layers by default
    
    [Header("Input Settings")]
    [Tooltip("Mouse button to use for clicking (0 = left, 1 = right, 2 = middle)")]
    public int mouseButton = 0;
    
    [Tooltip("Enable hover detection for highlighting objects")]
    public bool enableHoverDetection = true;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool showDebugInfo = false;
    
    [Tooltip("Draw the raycast line in the Scene view")]
    public bool drawDebugRay = true;
    
    private Camera cam;
    private ClickableObject currentHoveredObject;
  
    void Start()
    {
        // Get the camera component
 cam = GetComponent<Camera>();
        
 if (cam == null)
        {
            Debug.LogError("CameraRaycaster: No Camera component found! This script should be attached to a camera.");
     }
    }

    void Update()
    {
  // Continuously check for hover if enabled
        if (enableHoverDetection)
        {
   CheckHover();
        }
    
// Check for mouse click
        if (Input.GetMouseButtonDown(mouseButton))
      {
            PerformRaycast();
        }
    }
  
    /// <summary>
    /// Checks if the mouse is hovering over a clickable object
    /// </summary>
    void CheckHover()
    {
        if (cam == null) return;
      
        // Create a ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
   // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRayDistance, clickableLayerMask))
      {
      // Try to get the ClickableObject component
  ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
     if (clickable == null)
            {
      // Also check parent objects for the component
      clickable = hit.collider.GetComponentInParent<ClickableObject>();
            }
      
            // If we found a clickable object
       if (clickable != null)
   {
        // If this is a new object (different from what we were hovering)
       if (currentHoveredObject != clickable)
  {
   // Exit hover on the previous object
    if (currentHoveredObject != null)
         {
             currentHoveredObject.OnHoverExit();
    }
 
         // Enter hover on the new object
    currentHoveredObject = clickable;
         currentHoveredObject.OnHoverEnter();
            }
     // else: still hovering over the same object, do nothing
  }
            else
        {
          // Hit something but it's not a ClickableObject
          ClearHover();
    }
        }
        else
        {
            // Didn't hit anything
      ClearHover();
}
    }
    
    /// <summary>
    /// Clears the current hover state
    /// </summary>
    void ClearHover()
    {
        if (currentHoveredObject != null)
        {
            currentHoveredObject.OnHoverExit();
currentHoveredObject = null;
        }
    }
    
    /// <summary>
    /// Performs a raycast from the center of the camera's viewport
    /// </summary>
    void PerformRaycast()
    {
if (cam == null) return;
        
        // Create a ray from the center of the screen (0.5, 0.5 is the center)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
   RaycastHit hit;
        
  // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRayDistance, clickableLayerMask))
        {
            if (showDebugInfo)
  {
      Debug.Log($"Hit: {hit.collider.gameObject.name} at distance {hit.distance}");
            }
        
// Draw debug ray in green if it hits something
    if (drawDebugRay)
            {
       Debug.DrawLine(ray.origin, hit.point, Color.green, 2f);
       }
       
   // Try to get the ClickableObject component and call OnClicked
  ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
      if (clickable != null)
     {
            clickable.OnClicked(hit);
    }
      else
            {
// Also check parent objects for the component
    clickable = hit.collider.GetComponentInParent<ClickableObject>();
            if (clickable != null)
   {
           clickable.OnClicked(hit);
        }
        else if (showDebugInfo)
        {
            Debug.Log($"{hit.collider.gameObject.name} has no ClickableObject component.");
      }
         }
      }
     else
        {
            // Draw debug ray in red if it doesn't hit anything
      if (drawDebugRay)
            {
     Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.red, 2f);
            }
       
     if (showDebugInfo)
       {
    Debug.Log("Raycast did not hit any objects.");
        }
        }
    }
    
    /// <summary>
    /// Public method to perform a raycast manually (useful for external triggers)
    /// </summary>
  public bool TryRaycast(out RaycastHit hit)
    {
        if (cam == null)
        {
       hit = new RaycastHit();
      return false;
        }
     
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
     return Physics.Raycast(ray, out hit, maxRayDistance, clickableLayerMask);
    }
    
    void OnDisable()
  {
        // Clear hover state when component is disabled
        ClearHover();
    }
}

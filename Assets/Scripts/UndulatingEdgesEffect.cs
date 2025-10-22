using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the UndulatingEdges shader effect on a UI Image.
/// Attach this to any UI Image that uses the UndulatingEdges shader.
/// </summary>
[RequireComponent(typeof(Image))]
public class UndulatingEdgesEffect : MonoBehaviour
{
    [Header("Wave Settings")]
  [Tooltip("How far the waves distort (0-0.1)")]
    [Range(0f, 0.1f)]
    public float amplitude = 0.02f;
    
    [Tooltip("Number of waves (0-50)")]
    [Range(0f, 50f)]
    public float frequency = 10f;
    
    [Tooltip("Animation speed (-10 to 10)")]
    [Range(-10f, 10f)]
    public float speed = 2f;
    
    [Header("Edge Settings")]
    [Tooltip("How far the effect extends inward (0-0.5)")]
    [Range(0f, 0.5f)]
  public float edgeWidth = 0.1f;
    
 [Tooltip("Smoothness of the edge transition (0-1)")]
    [Range(0f, 1f)]
    public float edgeSoftness = 0.1f;
    
    [Tooltip("Color of the wave effect")]
    public Color edgeColor = Color.white;
  
    [Header("Animation")]
    [Tooltip("Automatically animate waves")]
    public bool animate = true;
    
    [Tooltip("Update material properties in real-time")]
    public bool realtimeUpdates = true;

    private Image image;
    private Material materialInstance;
    
    // Cache property IDs
    private static readonly int AmplitudeID = Shader.PropertyToID("_Amplitude");
    private static readonly int FrequencyID = Shader.PropertyToID("_Frequency");
    private static readonly int SpeedID = Shader.PropertyToID("_Speed");
    private static readonly int EdgeWidthID = Shader.PropertyToID("_EdgeWidth");
    private static readonly int EdgeSoftnessID = Shader.PropertyToID("_EdgeSoftness");
    private static readonly int EdgeColorID = Shader.PropertyToID("_EdgeColor");

    void Awake()
    {
        image = GetComponent<Image>();
        
     // Create material instance if using the correct shader
        if (image.material?.shader.name == "Custom/UI/UndulatingEdges")
        {
            materialInstance = new Material(image.material);
 image.material = materialInstance;
         UpdateMaterialProperties();
        }
else
   {
     Debug.LogError($"UndulatingEdgesEffect on {gameObject.name}: Image material must use the UndulatingEdges shader!");
        enabled = false;
        }
    }

    void OnEnable()
    {
      if (materialInstance != null)
        {
            UpdateMaterialProperties();
 }
    }

    void Update()
    {
        if (realtimeUpdates && materialInstance != null)
        {
     UpdateMaterialProperties();
        }
    }

    /// <summary>
/// Updates all material properties based on current settings
    /// </summary>
    public void UpdateMaterialProperties()
    {
        if (materialInstance == null) return;
        
        materialInstance.SetFloat(AmplitudeID, animate ? amplitude : 0f);
 materialInstance.SetFloat(FrequencyID, frequency);
      materialInstance.SetFloat(SpeedID, animate ? speed : 0f);
        materialInstance.SetFloat(EdgeWidthID, edgeWidth);
        materialInstance.SetFloat(EdgeSoftnessID, edgeSoftness);
        materialInstance.SetColor(EdgeColorID, edgeColor);
}

    /// <summary>
    /// Enable/disable wave animation
    /// </summary>
    public void SetAnimationEnabled(bool enabled)
    {
        animate = enabled;
        UpdateMaterialProperties();
    }

    /// <summary>
    /// Set wave amplitude with optional animation
    /// </summary>
    public void SetAmplitude(float newAmplitude, float duration = 0f)
 {
        StartCoroutine(AnimateFloat(AmplitudeID, amplitude, newAmplitude, duration,
          (value) => amplitude = value));
    }

    /// <summary>
    /// Set wave frequency with optional animation
    /// </summary>
    public void SetFrequency(float newFrequency, float duration = 0f)
    {
  StartCoroutine(AnimateFloat(FrequencyID, frequency, newFrequency, duration,
            (value) => frequency = value));
    }

    /// <summary>
    /// Set wave speed with optional animation
    /// </summary>
    public void SetSpeed(float newSpeed, float duration = 0f)
    {
   StartCoroutine(AnimateFloat(SpeedID, speed, newSpeed, duration,
       (value) => speed = value));
}

    /// <summary>
    /// Set edge width with optional animation
    /// </summary>
    public void SetEdgeWidth(float width, float duration = 0f)
    {
        StartCoroutine(AnimateFloat(EdgeWidthID, edgeWidth, width, duration,
            (value) => edgeWidth = value));
    }

    /// <summary>
    /// Set edge color with optional animation
    /// </summary>
    public void SetEdgeColor(Color color, float duration = 0f)
    {
        StartCoroutine(AnimateColor(EdgeColorID, edgeColor, color, duration,
            (value) => edgeColor = value));
    }

    /// <summary>
  /// Animate a float property over time
    /// </summary>
    private System.Collections.IEnumerator AnimateFloat(int propertyID, float start, float end, float duration, System.Action<float> onUpdate)
    {
   if (duration <= 0f)
        {
    onUpdate(end);
            materialInstance.SetFloat(propertyID, end);
    yield break;
     }

  float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
        float current = Mathf.Lerp(start, end, t);
            
    onUpdate(current);
            materialInstance.SetFloat(propertyID, current);
            
            yield return null;
        }

        onUpdate(end);
        materialInstance.SetFloat(propertyID, end);
    }

    /// <summary>
    /// Animate a color property over time
    /// </summary>
    private System.Collections.IEnumerator AnimateColor(int propertyID, Color start, Color end, float duration, System.Action<Color> onUpdate)
    {
 if (duration <= 0f)
      {
            onUpdate(end);
   materialInstance.SetColor(propertyID, end);
       yield break;
        }

        float elapsed = 0f;
      while (elapsed < duration)
 {
      elapsed += Time.deltaTime;
            float t = elapsed / duration;
    Color current = Color.Lerp(start, end, t);
     
         onUpdate(current);
materialInstance.SetColor(propertyID, current);
     
     yield return null;
        }

        onUpdate(end);
        materialInstance.SetColor(propertyID, end);
    }

    void OnDestroy()
    {
   if (materialInstance != null)
   {
   Destroy(materialInstance);
        }
    }
}
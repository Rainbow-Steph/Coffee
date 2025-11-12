using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class BillboardController : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Transform to follow. Set when showing the billboard.")]
    public Transform target;

    [Tooltip("Local offset from the target position (interpreted in camera space: X=right, Y=up, Z=forward)")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Tooltip("Should the billboard always face the main camera?")]
    public bool faceCamera = true;

    [Tooltip("Should the billboard follow the target's position?")]
    public bool followTarget = true;

    [Header("Position Animation")]
    public bool enableFloat = true;
    public float floatAmplitude = 0.15f;
    public float floatSpeed = 1f;

    [Header("Rotation")]
    [Tooltip("Rotation speed around local Y (degrees/sec) in addition to facing camera")]
    public float rotationSpeed = 0f;

    [Header("Color Animation")]
    public bool enableColorAnimation = false;
    public Color colorA = Color.white;
    public Color colorB = Color.yellow;
    public float colorSpeed = 1f;

    [Header("Size Animation")]
    public bool enableSizeAnimation = false;
    public Vector3 sizeA = Vector3.one;
    public Vector3 sizeB = Vector3.one * 1.2f;
    public float sizeSpeed = 1f;

    [Header("Display")]
    [Tooltip("Optional label to prefix the displayed value")]
    public string labelPrefix = "";

    private TMP_Text tmpText;
    private Camera mainCamera;
    private float startTime;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        mainCamera = Camera.main;
        startTime = Time.time;

        // Start hidden by default
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        float t = Time.time - startTime;

        // Follow target position + float animation
        if (target != null && followTarget)
        {
            Vector3 floatOffset = Vector3.zero;
            if (enableFloat && floatAmplitude != 0f)
            {
                floatOffset.y = Mathf.Sin(t * floatSpeed) * floatAmplitude;
            }

            // Interpret `offset` in camera space: x=right, y=up, z=forward
            Vector3 camRelativeOffset;
            if (mainCamera != null)
            {
                camRelativeOffset = mainCamera.transform.right * offset.x
                                  + mainCamera.transform.up * offset.y
                                  + mainCamera.transform.forward * offset.z;
            }
            else
            {
                camRelativeOffset = offset;
            }

            transform.position = target.position + camRelativeOffset + floatOffset;
        }

        // Face camera (billboard)
        if (faceCamera && mainCamera != null)
        {
            // Use camera forward to orient text to face camera
            Vector3 lookDir = transform.position - mainCamera.transform.position;
            if (lookDir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir.normalized, mainCamera.transform.up);
            }
        }

        // Apply additional rotation
        if (rotationSpeed != 0f)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // Color animation
        if (enableColorAnimation && tmpText != null)
        {
            float ct = (Mathf.Sin(t * colorSpeed) + 1f) * 0.5f; // 0..1
            tmpText.color = Color.Lerp(colorA, colorB, ct);
        }

        // Size animation
        if (enableSizeAnimation)
        {
            float st = (Mathf.Sin(t * sizeSpeed) + 1f) * 0.5f;
            transform.localScale = Vector3.Lerp(sizeA, sizeB, st);
        }
    }

    /// <summary>
    /// Show and attach the billboard to a target with a starting text
    /// </summary>
    public void Show(Transform newTarget, string text)
    {
        target = newTarget;
        if (tmpText != null)
        {
            tmpText.text = string.IsNullOrEmpty(labelPrefix) ? text : labelPrefix + " " + text;
        }
        startTime = Time.time;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the billboard and clear the target
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        target = null;
    }

    public void SetText(string text)
    {
        if (tmpText != null)
        {
            tmpText.text = string.IsNullOrEmpty(labelPrefix) ? text : labelPrefix + " " + text;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

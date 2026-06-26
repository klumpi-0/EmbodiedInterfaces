using UnityEngine;
using TMPro;
using Oculus.Interaction;

/// <summary>
/// Controls the InfoBox UI element:
/// - Sets header and body text via public methods or Inspector
/// - Auto-resizes the RoundedBox background based on text content
/// </summary>
[ExecuteAlways]
public class InfoBoxController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The GameObject with the RoundedBoxProperties script (background quad)")]
    [SerializeField] private RoundedBoxProperties roundedBox;

    [Tooltip("TextMeshPro component for the header")]
    [SerializeField] private TextMeshPro headerText;

    [Tooltip("TextMeshPro component for the body text")]
    [SerializeField] private TextMeshPro bodyText;

    [Header("Layout Settings")]
    [Tooltip("Padding on left and right sides (in world units)")]
    [SerializeField] private float paddingHorizontal = 5f;

    [Tooltip("Padding on top and bottom (in world units)")]
    [SerializeField] private float paddingVertical = 5f;

    [Tooltip("Space between header and body text (in world units)")]
    [SerializeField] private float spacingBetweenTexts = 2f;

    [Tooltip("Minimum width of the background box")]
    [SerializeField] private float minWidth = 30f;

    [Tooltip("Minimum height of the background box")]
    [SerializeField] private float minHeight = 20f;

    [Header("Initial Content (optional)")]
    [SerializeField] private string initialHeader = "Header";
    [SerializeField] [TextArea] private string initialBody = "Your text goes here.";

    // ---------------------------------------------------------------

    private void Start()
    {
        // Apply initial content if set in the Inspector
        if (!string.IsNullOrEmpty(initialHeader) || !string.IsNullOrEmpty(initialBody))
            SetTexts(initialHeader, initialBody);
    }

#if UNITY_EDITOR
    // Live preview in Editor without entering Play Mode
    private void OnValidate()
    {
        // Small delay so TMP has time to rebuild its mesh
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            SetTexts(initialHeader, initialBody);
        };
    }
#endif

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>Sets both texts and immediately resizes the background.</summary>
    public void SetTexts(string header, string body)
    {
        SetHeader(header);
        SetBody(body);
        // Resize happens inside SetBody (last call), but call explicitly to be safe
        ResizeBackground();
    }

    /// <summary>Sets only the header text and resizes.</summary>
    public void SetHeader(string header)
    {
        if (headerText == null) return;
        headerText.text = header;
        headerText.ForceMeshUpdate();
        ResizeBackground();
    }

    /// <summary>Sets only the body text and resizes.</summary>
    public void SetBody(string body)
    {
        if (bodyText == null) return;
        bodyText.text = body;
        bodyText.ForceMeshUpdate();
        ResizeBackground();
    }

    // ---------------------------------------------------------------
    // Core resize logic
    // ---------------------------------------------------------------

    private void ResizeBackground()
    {
        if (roundedBox == null) return;

        // --- Measure preferred sizes reported by TMP ---
        float headerW = 0f, headerH = 0f;
        float bodyW   = 0f, bodyH   = 0f;

        if (headerText != null)
        {
            headerW = headerText.GetPreferredValues().x;
            headerH = headerText.GetPreferredValues().y;
        }

        if (bodyText != null)
        {
            bodyW = bodyText.GetPreferredValues().x;
            bodyH = bodyText.GetPreferredValues().y;
        }

        // --- Calculate required box dimensions ---
        float requiredWidth  = Mathf.Max(headerW, bodyW) + paddingHorizontal * 2f;
        float requiredHeight = headerH + spacingBetweenTexts + bodyH + paddingVertical * 2f;

        float finalWidth  = Mathf.Max(requiredWidth,  minWidth);
        float finalHeight = Mathf.Max(requiredHeight, minHeight);

        // --- Apply to RoundedBoxProperties ---
        roundedBox.Width  = finalWidth;
        roundedBox.Height = finalHeight;

        // Reposition text elements relative to box center
        RepositionTexts(finalWidth, finalHeight, headerH, bodyH);
    }

    private void RepositionTexts(float boxWidth, float boxHeight, float headerH, float bodyH)
    {
        // Origin = center of the box
        float topEdge = boxHeight * 0.5f - paddingVertical;

        if (headerText != null)
        {
            Vector3 pos = headerText.transform.localPosition;
            pos.y = topEdge - headerH * 0.5f;
            headerText.transform.localPosition = pos;
        }

        if (bodyText != null)
        {
            Vector3 pos = bodyText.transform.localPosition;
            pos.y = topEdge - headerH - spacingBetweenTexts - bodyH * 0.5f;
            bodyText.transform.localPosition = pos;
        }
    }
}

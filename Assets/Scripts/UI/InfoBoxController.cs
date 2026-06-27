using Oculus.Interaction;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the InfoBox UI element.
/// The box grows symmetrically (up AND down) around its center pivot.
/// Text block is always centered inside the box.
/// </summary>
[ExecuteAlways]
public class InfoBoxController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The RoundedBoxProperties script on the background quad")]
    [SerializeField] private RoundedBoxProperties roundedBox;

    [Tooltip("TextMeshPro component for the header")]
    [SerializeField] private TextMeshPro headerText;

    [Tooltip("TextMeshPro component for the body text")]
    [SerializeField] private TextMeshPro bodyText;

    [Header("Layout Settings")]
    [SerializeField] private float paddingHorizontal = 5f;
    [SerializeField] private float paddingVertical = 5f;
    [SerializeField] private float spacingBetweenTexts = 2f;
    [SerializeField] private float minWidth = 30f;
    [SerializeField] private float minHeight = 20f;

    [Header("Initial Content")]
    [SerializeField] private string initialHeader = "Header";
    [SerializeField][TextArea] private string initialBody = "Your text goes here.";

    // ---------------------------------------------------------------

    private void Start() => SetTexts(initialHeader, initialBody);

#if UNITY_EDITOR
    private void OnValidate()
    {
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

    public void SetTexts(string header, string body)
    {
        if (headerText != null) { headerText.text = header; headerText.ForceMeshUpdate(); }
        if (bodyText != null) { bodyText.text = body; bodyText.ForceMeshUpdate(); }
        Rebuild();
    }

    public void SetHeader(string header)
    {
        if (headerText != null) { headerText.text = header; headerText.ForceMeshUpdate(); }
        Rebuild();
    }

    public void SetBody(string body)
    {
        if (bodyText != null) { bodyText.text = body; bodyText.ForceMeshUpdate(); }
        Rebuild();
    }

    // ---------------------------------------------------------------
    // Core layout — everything is calculated relative to box center
    // ---------------------------------------------------------------

    private void Rebuild()
    {
        if (roundedBox == null) return;

        // --- 1. Measure each text block ---
        Vector2 headerSize = headerText != null
            ? headerText.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity)
            : Vector2.zero;

        Vector2 bodySize = bodyText != null
            ? bodyText.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity)
            : Vector2.zero;

        // --- 2. Total content block ---
        bool hasHeader = headerText != null && !string.IsNullOrEmpty(headerText.text);
        bool hasBody = bodyText != null && !string.IsNullOrEmpty(bodyText.text);

        float gap = (hasHeader && hasBody) ? spacingBetweenTexts : 0f;
        float contentHeight = headerSize.y + gap + bodySize.y;
        float contentWidth = Mathf.Max(headerSize.x, bodySize.x);

        // --- 3. Box size ---
        float boxWidth = Mathf.Max(contentWidth + paddingHorizontal * 2f, minWidth);
        float boxHeight = Mathf.Max(contentHeight + paddingVertical * 2f, minHeight);

        roundedBox.Width = boxWidth;
        roundedBox.Height = boxHeight;

        // --- 4. Position texts so the whole block is vertically centered ---
        // Box pivot is at center (0,0). Content block is centered around 0.
        //
        //  +------------ +boxHeight/2 (top) ------------+
        //  |  paddingVertical                            |
        //  |  [header rect center at blockTop - h/2]    |
        //  |  [gap]                                      |
        //  |  [body rect center at ...]                  |
        //  |  paddingVertical                            |
        //  +------------ -boxHeight/2 (bottom) ----------+

        float blockTop = contentHeight * 0.5f;  // in local space, content starts here

        if (headerText != null)
        {
            Vector3 p = headerText.transform.localPosition;
            p.y = blockTop - headerSize.y * 0.5f;
            headerText.transform.localPosition = p;
        }

        if (bodyText != null)
        {
            float bodyCenter = blockTop - headerSize.y - gap - bodySize.y * 0.5f;
            Vector3 p = bodyText.transform.localPosition;
            p.y = bodyCenter;
            bodyText.transform.localPosition = p;
        }
    }
}
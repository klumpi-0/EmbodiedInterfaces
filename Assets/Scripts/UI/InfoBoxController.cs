using Oculus.Interaction;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class InfoBoxController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoundedBoxProperties roundedBox;
    [SerializeField] private TextMeshPro headerText;
    [SerializeField] private TextMeshPro bodyText;

    [Header("Layout Settings")]
    [SerializeField] private float paddingHorizontal = 5f;
    [SerializeField] private float paddingVertical = 5f;
    [SerializeField] private float spacingBetweenTexts = 2f;
    [SerializeField] private float minWidth = 30f;
    [SerializeField] private float minHeight = 20f;

    [Tooltip("Fixed content width in world units. Text wraps at this width. " +
             "Box width = this + 2 * paddingHorizontal.")]
    [SerializeField] private float contentWidth = 40f;

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
    // Layout
    // ---------------------------------------------------------------

    private void Rebuild()
    {
        if (roundedBox == null) return;

        // Measure with a fixed wrap width so TMP respects line breaks
        Vector2 headerSize = headerText != null
            ? headerText.GetPreferredValues(contentWidth, float.PositiveInfinity)
            : Vector2.zero;

        Vector2 bodySize = bodyText != null
            ? bodyText.GetPreferredValues(contentWidth, float.PositiveInfinity)
            : Vector2.zero;

        bool hasHeader = headerText != null && !string.IsNullOrEmpty(headerText.text);
        bool hasBody = bodyText != null && !string.IsNullOrEmpty(bodyText.text);
        float gap = (hasHeader && hasBody) ? spacingBetweenTexts : 0f;
        float contentHeight = headerSize.y + gap + bodySize.y;

        // Box dimensions
        float boxWidth = Mathf.Max(contentWidth + paddingHorizontal * 2f, minWidth);
        float boxHeight = Mathf.Max(contentHeight + paddingVertical * 2f, minHeight);

        roundedBox.Width = boxWidth;
        roundedBox.Height = boxHeight;

        // Center the whole text block vertically inside the box
        float blockTop = contentHeight * 0.5f;

        if (headerText != null)
        {
            Vector3 p = headerText.transform.localPosition;
            p.y = blockTop - headerSize.y * 0.5f;
            headerText.transform.localPosition = p;
        }

        if (bodyText != null)
        {
            Vector3 p = bodyText.transform.localPosition;
            p.y = blockTop - headerSize.y - gap - bodySize.y * 0.5f;
            bodyText.transform.localPosition = p;
        }
    }
}
using UnityEngine;

public class FloraHovering : MonoBehaviour
{
    public static FloraHovering Instance;

    [SerializeField] Transform targetTransform;

    [Header("Hovering")]
    [SerializeField] float intervallHovering = 5f;
    [SerializeField] float hoverAmplitude = 0.05f;   // Wie weit hoch/runter (in Units)

    [Header("Scaling")]
    [SerializeField] float intervallScaling = 3f;
    [SerializeField] float scaleAmplitude = 0.03f;   // Wie stark größer/kleiner

    [Header("Figure-Eight (seitwärts)")]
    [SerializeField] private bool figureEightEnabled = false;
    [SerializeField] private float intervallFigureEight = 4f;
    [SerializeField] private float figureEightWidth = 0.05f;   // Ausschlag in X
    [SerializeField] private float figureEightDepth = 0.03f;   // Ausschlag in Z

    private Vector3 startPosition;
    private Vector3 startScale;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Ausgangswerte speichern
        startPosition = targetTransform.localPosition;
        startScale = targetTransform.localScale;
        SetFigureEightEnabled(true);
    }

    void Update()
    {
        float time = Time.time;

        float hoverOffset = Mathf.Sin(time * (Mathf.PI * 2f / intervallHovering)) * hoverAmplitude;
        Vector3 newPos = startPosition;
        newPos.y += hoverOffset;

        if (figureEightEnabled)
        {
            float t = time * (Mathf.PI * 2f / intervallFigureEight);
            float sinT = Mathf.Sin(t);
            float cosT = Mathf.Cos(t);

            newPos.x += sinT * figureEightWidth;
            newPos.z += sinT * cosT * figureEightDepth;
        }

        targetTransform.localPosition = newPos;

        float scaleOffset = Mathf.Sin(time * (Mathf.PI * 2f / intervallScaling)) * scaleAmplitude;
        targetTransform.localScale = startScale + Vector3.one * scaleOffset;
    }

    public void SetFigureEightEnabled(bool enabled)
    {
        figureEightEnabled = enabled;
    }

    public void ToggleFigureEight()
    {
        figureEightEnabled = !figureEightEnabled;
    }
}
using UnityEngine;

public class FloraHovering : MonoBehaviour
{
    [SerializeField] Transform targetTransform;

    [Header("Hovering")]
    [SerializeField] float intervallHovering = 5f;
    [SerializeField] float hoverAmplitude = 0.05f;   // Wie weit hoch/runter (in Units)

    [Header("Scaling")]
    [SerializeField] float intervallScaling = 3f;
    [SerializeField] float scaleAmplitude = 0.03f;   // Wie stark größer/kleiner

    private Vector3 startPosition;
    private Vector3 startScale;

    void Start()
    {
        // Ausgangswerte speichern
        startPosition = targetTransform.localPosition;
        startScale = targetTransform.localScale;
    }

    void Update()
    {
        float time = Time.time;

        float hoverOffset = Mathf.Sin(time * (Mathf.PI * 2f / intervallHovering)) * hoverAmplitude;
        Vector3 newPos = startPosition;
        newPos.y += hoverOffset;
        targetTransform.localPosition = newPos;

        float scaleOffset = Mathf.Sin(time * (Mathf.PI * 2f / intervallScaling)) * scaleAmplitude;
        targetTransform.localScale = startScale + Vector3.one * scaleOffset;
    }
}
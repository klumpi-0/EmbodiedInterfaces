using UnityEngine;
using UnityEngine.Events;

public class FloraSpinAnimation : MonoBehaviour
{
    public static FloraSpinAnimation Instance;

    [Header("References")]
    [SerializeField] private Transform floraTransform;

    [Header("Spin Settings")]
    [SerializeField] private float spinDuration = 1.2f;
    [SerializeField] private int spinCount = 1;                  // Wie viele Umdrehungen
    [SerializeField] private float riseHeight = 0.12f;           // Wie hoch während der Drehung
    [SerializeField] private AnimationCurve riseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f); // Berg-Kurve

    [Header("Events")]
    public UnityEvent onSpinStart;
    public UnityEvent onSpinFinished;

    private Vector3 _startLocalPos;
    private Quaternion _startLocalRot;
    private bool _isAnimating = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (floraTransform == null)
            floraTransform = transform;

        _startLocalPos = floraTransform.localPosition;
        _startLocalRot = floraTransform.localRotation;

        // Berg-Kurve als Default setzen falls nicht im Inspector gesetzt
        if (riseCurve.keys.Length < 3)
        {
            riseCurve = new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 2f),
                new Keyframe(0.5f, 1f, 0f, 0f),
                new Keyframe(1f, 0f, -2f, 0f)
            );
        }
    }

    public void TriggerSpin()
    {
        if (_isAnimating) return;
        StartCoroutine(SpinRoutine());
    }

    private System.Collections.IEnumerator SpinRoutine()
    {
        _isAnimating = true;
        onSpinStart?.Invoke();

        float totalDegrees = 360f * spinCount;
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);

            // Rotation — gleichmäßig mit leichtem EaseIn/Out
            float easedT = EaseInOutQuad(t);
            float currentAngle = totalDegrees * easedT;
            floraTransform.localRotation = _startLocalRot * Quaternion.Euler(0f, currentAngle, 0f);

            // Höhe — Berg-Kurve über die Animations-Kurve
            float heightOffset = riseCurve.Evaluate(t) * riseHeight;
            floraTransform.localPosition = _startLocalPos + new Vector3(0f, heightOffset, 0f);

            yield return null;
        }

        // Exakt auf Startposition/-rotation zurücksetzen
        floraTransform.localPosition = _startLocalPos;
        floraTransform.localRotation = _startLocalRot;

        _isAnimating = false;
        onSpinFinished?.Invoke();
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    public void UpdateStartTransform()
    {
        if (!_isAnimating)
        {
            _startLocalPos = floraTransform.localPosition;
            _startLocalRot = floraTransform.localRotation;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (floraTransform == null)
            floraTransform = transform;
    }
#endif
}
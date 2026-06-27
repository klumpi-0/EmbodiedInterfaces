using UnityEngine;
using UnityEngine.Events;

public class FloraButtonPress : MonoBehaviour
{
    public static FloraButtonPress Instance;

    [Header("References")]
    [SerializeField] private Transform floraTransform;

    [Header("Press Settings")]
    [SerializeField] private float pressDownDistance = 0.08f;   // wie weit runter
    [SerializeField] private float pressForwardDistance = 0.04f; // wie weit nach vorne
    [SerializeField] private float pressDownDuration = 0.2f;    // wie schnell runter
    [SerializeField] private float pressUpDuration = 0.35f;     // wie schnell wieder hoch
    [SerializeField] private float holdDuration = 0.08f;        // kurz unten halten

    [Header("Repetition Settings")]
    [SerializeField] private float timeBetweenRepetitions;
    [SerializeField] private bool shouldRepeatAnimation;

    [Header("Events")]
    public UnityEvent onPressStart;
    public UnityEvent onPressBottom;  // Moment wo sie unten ankommt
    public UnityEvent onPressFinished;

    private Vector3 _startLocalPos;
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
    }

    // Das ist der öffentliche Auslöser — per UnityEvent oder Code aufrufbar
    public void TriggerPress(bool resetStartPos = true, bool repeatAnimation = false)
    {
        if (_isAnimating) return;
        if (resetStartPos) { UpdateStartPosition(); }
        if (repeatAnimation) { onPressFinished.AddListener(RestartAnimation); shouldRepeatAnimation = true; }
        else { onPressFinished.RemoveListener(RestartAnimation); shouldRepeatAnimation = false; }
        StartCoroutine(PressRoutine());
    }

    private System.Collections.IEnumerator PressRoutine()
    {
        _isAnimating = true;
        onPressStart?.Invoke();

        Vector3 startPos = floraTransform.localPosition;

        // Ziel: lokal nach unten + leicht nach vorne
        // "vorne" ist relativ zum Parent-Transform
        Vector3 pressOffset = new Vector3(0, -pressDownDistance, pressForwardDistance);
        Vector3 pressTarget = _startLocalPos + pressOffset;

        // --- Phase 1: Runterdrücken ---
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressDownDuration;
            float ease = EaseInQuad(Mathf.Clamp01(t));
            floraTransform.localPosition = Vector3.Lerp(startPos, pressTarget, ease);
            yield return null;
        }
        floraTransform.localPosition = pressTarget;
        onPressBottom?.Invoke();

        // --- Phase 2: Kurz halten ---
        yield return new WaitForSeconds(holdDuration);

        // --- Phase 3: Wieder hoch ---
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pressUpDuration;
            float ease = EaseOutBack(Mathf.Clamp01(t));
            floraTransform.localPosition = Vector3.Lerp(pressTarget, _startLocalPos, ease);
            yield return null;
        }
        floraTransform.localPosition = _startLocalPos;

        _isAnimating = false;
        onPressFinished?.Invoke();
    }

    // Schnell rein
    private float EaseInQuad(float t) => t * t;

    // Leicht überschwingen beim Zurückkommen — wirkt lebendiger
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    // Startposition neu setzen falls Flora bewegt wurde
    public void UpdateStartPosition()
    {
        if (!_isAnimating)
            _startLocalPos = floraTransform.localPosition;
    }

    private void RestartAnimation()
    {
        StartCoroutine(RestartCoroutine());
    }

    private System.Collections.IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(timeBetweenRepetitions);
        if (shouldRepeatAnimation) { TriggerPress(repeatAnimation: true); }
    }

    public void StopRepeatAnimationPlaying()
    {
        shouldRepeatAnimation = false;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (floraTransform == null)
            floraTransform = transform;
    }
#endif
}
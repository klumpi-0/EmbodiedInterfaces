using UnityEngine;

/// <summary>
/// Bewegt ein Glow-Objekt (z.B. ein Licht + Glow-Sprite) von einem Startpunkt
/// zu einem Endpunkt. Einfach an ein GameObject hängen, das die visuelle
/// Glow-Darstellung enthält (z.B. Sprite mit additivem/Emission-Material
/// und/oder eine Light2D bzw. Light-Komponente).
/// </summary>
[RequireComponent(typeof(Transform))]
public class GlowMover : MonoBehaviour
{
    [Header("Wegpunkte")]
    [Tooltip("Startposition des Glows")]
    public Transform startPoint;

    [Tooltip("Zielposition des Glows")]
    public Transform endPoint;

    [Header("Bewegung")]
    [Tooltip("Dauer der Bewegung in Sekunden")]
    public float duration = 1.5f;

    [Tooltip("Easing-Kurve für die Bewegung (0-1 -> 0-1)")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Bewegung beim Start automatisch abspielen")]
    public bool playOnStart = true;

    [Tooltip("Bewegung nach Ende in Loop wiederholen")]
    public bool loop = false;

    [Tooltip("Am Ende zurück zum Start pingpongen statt neu zu starten")]
    public bool pingPong = false;

    [Header("Glow-Optik")]
    [Tooltip("Farbe des Glows")]
    public Color glowColor = new Color(1f, 0.92f, 0.3f, 1f); // gelb

    [Tooltip("Optionales Licht (Light oder Light2D), dessen Farbe/Intensität mit dem Glow pulsen soll")]
    public Light glowLight;

    [Tooltip("Basis-Intensität des Lichts")]
    public float lightIntensity = 2.5f;

    [Tooltip("Pulsieren der Intensität während der Bewegung (0 = konstant)")]
    public float pulseAmount = 0.5f;

    [Tooltip("Pulsgeschwindigkeit")]
    public float pulseSpeed = 6f;

    [Tooltip("SpriteRenderer für das Glow-Bild (z.B. weicher radialer Blob), Farbe wird gesetzt")]
    public SpriteRenderer glowSprite;

    [Tooltip("Optionaler Trail, der die Bewegungsspur zeigt (leer lassen, falls nicht benötigt)")]
    public TrailRenderer trail;

    private float _t;
    private bool _isPlaying;
    private bool _reverse;

    private void Awake()
    {
        ApplyGlowColor();
    }

    private void Start()
    {
        if (startPoint != null)
            transform.position = startPoint.position;

        if (playOnStart)
            Play();
    }

    /// <summary>Startet die Bewegung von vorne.</summary>
    public void Play()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogWarning("[GlowMover] Start- oder Endpunkt nicht gesetzt.");
            return;
        }

        _t = 0f;
        _reverse = false;
        _isPlaying = true;
        transform.position = startPoint.position;

        if (trail != null)
            trail.Clear();
    }

    /// <summary>Bewegung anhalten.</summary>
    public void Stop()
    {
        _isPlaying = false;
    }

    private void Update()
    {
        if (!_isPlaying || startPoint == null || endPoint == null)
            return;

        _t += Time.deltaTime / Mathf.Max(duration, 0.0001f);

        float clamped = Mathf.Clamp01(_t);
        float evalT = _reverse ? 1f - clamped : clamped;
        float curved = movementCurve.Evaluate(evalT);

        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, curved);

        UpdatePulse(clamped);

        if (_t >= 1f)
        {
            if (pingPong)
            {
                _t = 0f;
                _reverse = !_reverse;
            }
            else if (loop)
            {
                _t = 0f;
                transform.position = startPoint.position;
            }
            else
            {
                _isPlaying = false;
                transform.position = endPoint.position;
            }
        }
    }

    private void UpdatePulse(float normalizedTime)
    {
        if (glowLight == null)
            return;

        float pulse = pulseAmount > 0f
            ? Mathf.Sin(normalizedTime * pulseSpeed * Mathf.PI * 2f) * pulseAmount
            : 0f;

        glowLight.intensity = lightIntensity + pulse;
    }

    private void ApplyGlowColor()
    {
        if (glowLight != null)
            glowLight.color = glowColor;

        if (glowSprite != null)
            glowSprite.color = glowColor;
    }

    // Zur Laufzeit im Inspector die Farbe live anpassen können
    private void OnValidate()
    {
        ApplyGlowColor();
    }
}

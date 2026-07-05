using UnityEngine;

/// <summary>
/// Erzeugt einen gelben "Zauber-Nebel"-Glow, der von einem Referenzobjekt aus
/// in eine wählbare Richtung (Oben/Unten/Links/Rechts) ausgeht. Die Reichweite
/// wird per Raycast bestimmt: der Glow reicht nur bis zum getroffenen Objekt
/// (oder bis maxDistance, falls nichts getroffen wird).
///
/// Die Optik wird zur Laufzeit als ParticleSystem mit einer weichen,
/// selbst generierten Glow-Textur (additiv, Farbverlauf, leichtes Noise)
/// aufgebaut – es müssen keine externen Materialien/Texturen zugewiesen werden.
/// </summary>
public class DirectionalGlow : MonoBehaviour
{
    public enum GlowDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    [Header("Referenz & Richtung")]
    [Tooltip("Objekt, von dem der Glow ausgeht")]
    public Transform sourceObject;

    [Tooltip("Richtung des Glows. 'None' schaltet den Glow komplett aus")]
    public GlowDirection direction = GlowDirection.Up;

    [Tooltip("Falls aktiv, werden lokale Achsen des sourceObject verwendet " +
             "(transform.up/right) statt der Weltachsen")]
    public bool useLocalAxes = false;

    [Tooltip("Zusätzlicher Versatz des Ursprungs relativ zum sourceObject (lokal)")]
    public Vector3 originOffset = Vector3.zero;

    [Header("Raycast")]
    [Tooltip("Maximale Reichweite, falls der Raycast nichts trifft")]
    public float maxDistance = 20f;

    [Tooltip("Layer, die vom Raycast erkannt werden sollen")]
    public LayerMask hitMask = ~0;

    [Tooltip("2D-Physik (Physics2D) statt 3D-Physik für den Raycast verwenden")]
    public bool use2DPhysics = false;

    [Tooltip("Jeden Frame neu berechnen (z.B. wenn sich Objekte bewegen). " +
             "Wenn aus, nur bei Richtungswechsel per SetDirection().")]
    public bool updateContinuously = true;

    [Header("Optik – Nebel/Glow")]
    [Tooltip("Grundfarbe des Nebels")]
    public Color glowColor = new Color(1f, 0.82f, 0.25f, 1f);

    [Tooltip("Breite des Nebelstreifens (quer zur Richtung)")]
    public float thickness = 1.2f;

    [Tooltip("Größe der einzelnen Nebel-Partikel")]
    public float particleSize = 1.4f;

    [Tooltip("Partikeldichte pro Längeneinheit der Strecke")]
    public float density = 6f;

    [Tooltip("Stärke des Wabern/Verwirbelns (Noise-Modul)")]
    public float swirl = 0.4f;

    [Tooltip("Wie schnell der Nebel in der Höhe/Breite variiert")]
    public float sizeVariation = 0.3f;

    private ParticleSystem _ps;
    private ParticleSystemRenderer _psRenderer;
    private GameObject _psObject;
    private Vector3 _lastOrigin;
    private Vector3 _lastHitPoint;
    private bool _hasHit;

    private void Awake()
    {
        EnsureParticleSystem();
    }

    private void Start()
    {
        Recompute();
    }

    private void Update()
    {
        if (updateContinuously)
            Recompute();
    }

    /// <summary>Richtung von außen setzen (löst sofort eine Neuberechnung aus).</summary>
    public void SetDirection(GlowDirection newDirection)
    {
        direction = newDirection;
        Recompute();
    }

    /// <summary>Berechnet Ursprung, Raycast und aktualisiert die Nebel-Darstellung.</summary>
    public void Recompute()
    {
        if (_ps == null)
            EnsureParticleSystem();

        if (direction == GlowDirection.None || sourceObject == null)
        {
            SetEmissionActive(false);
            return;
        }

        Vector3 dir = GetDirectionVector();
        Vector3 origin = sourceObject.position + sourceObject.TransformDirection(originOffset);

        float distance = maxDistance;
        Vector3 hitPoint = origin + dir * maxDistance;
        _hasHit = false;

        if (use2DPhysics)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, hitMask);
            if (hit.collider != null)
            {
                distance = hit.distance;
                hitPoint = hit.point;
                _hasHit = true;
            }
        }
        else
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, hitMask))
            {
                distance = hit.distance;
                hitPoint = hit.point;
                _hasHit = true;
            }
        }

        _lastOrigin = origin;
        _lastHitPoint = hitPoint;

        UpdateFogVisual(origin, dir, distance);
        SetEmissionActive(true);
    }

    private Vector3 GetDirectionVector()
    {
        if (useLocalAxes && sourceObject != null)
        {
            switch (direction)
            {
                case GlowDirection.Up: return sourceObject.up;
                case GlowDirection.Down: return -sourceObject.up;
                case GlowDirection.Left: return -sourceObject.right;
                case GlowDirection.Right: return sourceObject.right;
            }
        }
        else
        {
            switch (direction)
            {
                case GlowDirection.Up: return Vector3.up;
                case GlowDirection.Down: return Vector3.down;
                case GlowDirection.Left: return Vector3.left;
                case GlowDirection.Right: return Vector3.right;
            }
        }

        return Vector3.zero;
    }

    private void UpdateFogVisual(Vector3 origin, Vector3 dir, float distance)
    {
        if (distance < 0.01f)
            distance = 0.01f;

        Vector3 midPoint = origin + dir * (distance * 0.5f);
        _psObject.transform.position = midPoint;
        _psObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        var shape = _ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(thickness, distance, thickness);

        var emission = _ps.emission;
        emission.rateOverTime = density * distance;

        var main = _ps.main;
        main.startColor = glowColor;
        main.startSize = new ParticleSystem.MinMaxCurve(
            particleSize * (1f - sizeVariation),
            particleSize * (1f + sizeVariation));

        var noise = _ps.noise;
        noise.strength = swirl;
    }

    private void SetEmissionActive(bool active)
    {
        if (_ps == null)
            return;

        var emission = _ps.emission;
        emission.enabled = active;

        if (!active && _ps.isPlaying)
            _ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        else if (active && !_ps.isPlaying)
            _ps.Play();
    }

    private void EnsureParticleSystem()
    {
        if (_psObject != null)
            return;

        _psObject = new GameObject("DirectionalGlow_Fog");
        _psObject.transform.SetParent(transform, false);

        _ps = _psObject.AddComponent<ParticleSystem>();
        _psRenderer = _psObject.GetComponent<ParticleSystemRenderer>();

        var main = _ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 2.2f);
        main.startSpeed = 0f;
        main.startSize = particleSize;
        main.startColor = glowColor;
        main.maxParticles = 800;

        var emission = _ps.emission;
        emission.rateOverTime = 0f;

        var shape = _ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(thickness, 1f, thickness);

        // Sanftes Ein-/Ausblenden über die Lebenszeit für weiche Nebel-Kanten
        var colorOverLifetime = _ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.6f, 0.2f),
                new GradientAlphaKey(0.6f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = gradient;

        // Leichtes Wachsen/Schrumpfen für organischeres Nebel-Gefühl
        var sizeOverLifetime = _ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f, AnimationCurve.EaseInOut(0f, 0.7f, 1f, 1.3f));

        // Wabern/Verwirbeln wie Nebel oder Zauberpartikel
        var noise = _ps.noise;
        noise.enabled = true;
        noise.strength = swirl;
        noise.frequency = 0.3f;
        noise.scrollSpeed = 0.2f;

        _psRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        _psRenderer.material = CreateGlowMaterial();
    }

    /// <summary>Erzeugt zur Laufzeit ein additives Material mit weicher, radialer Glow-Textur.</summary>
    private Material CreateGlowMaterial()
    {
        Texture2D tex = CreateSoftGlowTexture(64);

        Shader shader = Shader.Find("Particles/Standard Unlit")
                         ?? Shader.Find("Legacy Shaders/Particles/Additive")
                         ?? Shader.Find("Mobile/Particles/Additive")
                         ?? Shader.Find("Sprites/Default");

        Material mat = new Material(shader);

        if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
        if (mat.HasProperty("_BaseColorMap")) mat.SetTexture("_BaseColorMap", tex);

        return mat;
    }

    private Texture2D CreateSoftGlowTexture(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDist = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center) / maxDist;
                float alpha = Mathf.Clamp01(1f - dist);
                alpha = Mathf.Pow(alpha, 2f); // weichere, rundere Kante
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    private void OnDrawGizmosSelected()
    {
        if (sourceObject == null || direction == GlowDirection.None)
            return;

        Vector3 dir = GetDirectionVector();
        Vector3 origin = sourceObject.position + sourceObject.TransformDirection(originOffset);

        Gizmos.color = _hasHit ? Color.yellow : new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawLine(origin, _hasHit ? _lastHitPoint : origin + dir * maxDistance);

        if (_hasHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_lastHitPoint, 0.15f);
        }
    }
}

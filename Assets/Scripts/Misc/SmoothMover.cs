using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SmoothMover : MonoBehaviour
{
    public Vector3 targetPosition;
    public Vector3 targetRotation;

    [SerializeField] float duration = 1f;
    [SerializeField] float curveStrength = 1f;

    [Header("Drunk Movement")]
    [Tooltip("Wenn aktiv, folgt die Bewegung nicht der geraden Strecke, sondern " +
             "schlingert leicht zur Seite/oben-unten und in der Rotation - " +
             "wirkt dadurch weniger 'robotisch' bzw. realistischer.")]
    [SerializeField] bool drunkMovement = false;

    [Tooltip("Stärke des seitlichen Schlingerns")]
    [SerializeField] float drunkPositionAmount = 0.15f;

    [Tooltip("Stärke des Rotations-Wobbles (in Grad)")]
    [SerializeField] float drunkRotationAmount = 1f;

    [Tooltip("Wie schnell das Schlingern oszilliert")]
    [SerializeField] float drunkFrequency = 0.25f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion endRot;
    private float time;

    // Zufälliger Offset pro Instanz, damit mehrere Mover nicht synchron schlingern
    private float drunkSeed;

    public UnityEvent atFinalTransformEvent = new UnityEvent();


    public void Init(Vector3 targetPosition_, Vector3 targetRotation_, float duration_, float curveStrength_ = 1f, bool drunkMovement_ = false)
    {
        targetPosition = targetPosition_;
        targetRotation = targetRotation_;
        endRot = Quaternion.Euler(targetRotation_);
        duration = duration_;
        curveStrength = curveStrength_;
        drunkMovement = drunkMovement_;
        atFinalTransformEvent.RemoveAllListeners();

        atFinalTransformEvent.AddListener(DestroyMyself);
        StartCoroutine(InvokeDelayed(duration));
    }

    public void Init(Transform targetTransform_, float duration_, float curveStrength_ = 1f, bool drunkMovement_ = false)
    {
        targetPosition = targetTransform_.position;
        targetRotation = targetTransform_.rotation.eulerAngles;
        endRot = targetTransform_.rotation;
        duration = duration_;
        curveStrength = curveStrength_;
        drunkMovement = drunkMovement_;
        atFinalTransformEvent.RemoveAllListeners();

        atFinalTransformEvent.AddListener(DestroyMyself);
        StartCoroutine(InvokeDelayed(duration));
    }

    void OnEnable()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        if (targetRotation != Vector3.zero)
        {
            endRot = Quaternion.Euler(targetRotation);
        }
        else
        {
            endRot = startRot;
        }

        drunkSeed = Random.Range(0f, 1000f);
        time = 0f;
    }

    void Update()
    {
        if (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // this results in an S-shaped curve, with the curve strength param
            // determining the shape, higher values result in a slower start/end with faster middle
            if (t < 0.5f)
                t = 0.5f * Mathf.Pow(2f * t, curveStrength);
            else
                t = 1f - 0.5f * Mathf.Pow(2f * (1f - t), curveStrength);

            Vector3 basePosition = Vector3.Lerp(startPos, targetPosition, t);
            Quaternion baseRotation = Quaternion.Slerp(startRot, endRot, t);

            if (drunkMovement)
            {
                ApplyDrunkMovement(t, ref basePosition, ref baseRotation);
            }

            transform.position = basePosition;
            transform.rotation = baseRotation;
        }
    }

    /// <summary>
    /// Fügt der Bewegung ein seitliches Schlingern hinzu. Die Stärke wird über
    /// eine Sinus-Envelope moduliert, die bei t=0 und t=1 auf 0 geht - so bleibt
    /// Start- und Zielposition/-rotation exakt, nur der Weg dazwischen wirkt unrund.
    /// </summary>
    private void ApplyDrunkMovement(float t, ref Vector3 position, ref Quaternion rotation)
    {
        Vector3 moveDir = (targetPosition - startPos);
        if (moveDir.sqrMagnitude < 0.0001f)
            moveDir = transform.forward;
        moveDir.Normalize();

        Vector3 sideAxis = Vector3.Cross(Vector3.up, moveDir);
        if (sideAxis.sqrMagnitude < 0.0001f)
            sideAxis = Vector3.right;
        sideAxis.Normalize();

        Vector3 upAxis = Vector3.Cross(moveDir, sideAxis).normalized;

        // 0 an den Enden, Maximum in der Mitte der Bewegung
        float envelope = Mathf.Sin(t * Mathf.PI);

        float noiseTime = time * drunkFrequency;
        float sideNoise = (Mathf.PerlinNoise(drunkSeed, noiseTime) - 0.5f) * 2f;
        float upNoise = (Mathf.PerlinNoise(drunkSeed + 50f, noiseTime * 0.8f) - 0.5f) * 2f;
        float rotNoise = (Mathf.PerlinNoise(drunkSeed + 100f, noiseTime * 0.6f) - 0.5f) * 2f;

        Vector3 offset = sideAxis * (sideNoise * drunkPositionAmount * envelope)
                        + upAxis * (upNoise * drunkPositionAmount * 0.5f * envelope);

        position += offset;

        Quaternion wobble = Quaternion.AngleAxis(rotNoise * drunkRotationAmount * envelope, upAxis)
                           * Quaternion.AngleAxis(sideNoise * drunkRotationAmount * 0.5f * envelope, sideAxis);

        rotation = wobble * rotation;
    }

    IEnumerator InvokeDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        atFinalTransformEvent?.Invoke();
    }

    public void SetPositionAndRotation(Vector3 position, Vector3 rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
        endRot = Quaternion.Euler(targetRotation);
    }

    public void SetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public float GetDuration() { return duration; }
    public void SetDuration(float duration_) { duration = duration_; }
    public void SetCurve(float curve) { curveStrength = curve; }
    public void SetDrunkMovement(bool enabled_) { drunkMovement = enabled_; }
    public bool GetDrunkMovement() { return drunkMovement; }

    private void DestroyMyself()
    {
        Destroy(this);
    }
}
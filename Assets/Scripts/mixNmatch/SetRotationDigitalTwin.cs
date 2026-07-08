using System.Globalization;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class SetRotationDigitalTwin : MonoBehaviour
{
    public static SetRotationDigitalTwin Instance;

    [SerializeField] private bool debugRotation;
    [SerializeField] private bool useSerialPort;

    [Header("Rotations")]
    [SerializeField] private float lowRotation;
    [SerializeField] private float middleRotation;
    [SerializeField] private float highRotation;

    [Header("Offset")]
    [SerializeField] private float lowOffset;
    [SerializeField] private float middleOffset;
    [SerializeField] private float highOffset;

    [Header("References")]
    [SerializeField] private GameObject lowCube;
    [SerializeField] private GameObject middleCube;
    [SerializeField] private GameObject highCube;

    [Header("Button")]
    [SerializeField] private bool buttonIsPressed;
    [SerializeField] private bool lastButtonIsPressed;
    public UnityEvent buttonPressedEvent = new UnityEvent();
    [SerializeField] private InteractableUnityEventWrapper wrapper;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        wrapper.WhenSelect.AddListener(ActivateButtonPressedEvent);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            wrapper.WhenSelect.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.L) || OVRInput.Get(OVRInput.Button.Three))
        {
            CalibrateOffsets();
        }

        CheckIfInvokeButtonPressed();

        if (debugRotation)
        {
            SetRotationForCube(lowCube, lowRotation, lowOffset);
            SetRotationForCube(middleCube, middleRotation, middleOffset);
            SetRotationForCube(highCube, highRotation, highOffset);
        }

        if (useSerialPort)
        {
            // Null-Check: Falls der Port nicht geöffnet werden konnte, soll das
            // hier nicht mit einer NullReferenceException crashen.
            if (SerialDataHandler.Instance == null)
                return;

            lowRotation = SerialDataHandler.Instance.lowAngle;
            middleRotation = SerialDataHandler.Instance.middleAngle;
            highRotation = SerialDataHandler.Instance.highAngle;

            SetRotationForCube(lowCube, lowRotation, lowOffset);
            SetRotationForCube(middleCube, middleRotation, middleOffset);
            SetRotationForCube(highCube, highRotation, highOffset);

            buttonIsPressed = SerialDataHandler.Instance.buttonPressed;

            if (SerialDataHandler.Instance.buttonPressedDown)
            {
                ActivateButtonPressedEvent();
            }
            lastButtonIsPressed = buttonIsPressed;
        }
    }

    /// <summary>
    /// Alternativer Eingang für die Rotation als String, falls die Werte nicht
    /// über den SerialDataHandler, sondern direkt übergeben werden.
    /// Erwartetes Format: "low,middle,high" oder "low,middle,high,button(0/1)"
    /// </summary>
    public void SetRotation(string rotationInput)
    {
        if (string.IsNullOrWhiteSpace(rotationInput))
            return;

        string[] parts = rotationInput.Trim().Split(',');
        if (parts.Length < 3)
        {
            Debug.LogWarning($"[SetRotationDigitalTwin] Ungültiges Format für SetRotation: \"{rotationInput}\"");
            return;
        }

        if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float low))
            SetRotationForCube(lowCube, low, lowOffset);

        if (float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float middle))
            SetRotationForCube(middleCube, middle, middleOffset);

        if (float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float high))
            SetRotationForCube(highCube, high, highOffset);

        if (parts.Length >= 4 && parts[3].Trim() == "1")
        {
            ActivateButtonPressedEvent();
        }
    }

    private void SetRotationForCube(GameObject cube, float newRotation, float offset)
    {
        if (cube == null) return;

        float y = NormalizeAngle(newRotation + offset);
        cube.transform.localRotation = Quaternion.Euler(0f, y, 0f);
    }
    /// <summary>
    /// Kalibriert den Versatz zwischen Arduino-Rohwinkel und Unity-Rotation.
    /// Aufruf genau in dem Moment, in dem die realen Bauteile von Hand korrekt
    /// ausgerichtet ("nach vorne") gehalten werden. Danach zeigen die Cubes bei
    /// diesem Rohwinkel exakt auf 0°, und alle weiteren Bewegungen sind relativ
    /// dazu korrekt.
    ///
    /// Rechnung: offset = -aktuellerRohwinkel, denn dann gilt bei
    /// SetRotationForCube: rohwinkel + offset = rohwinkel - rohwinkel = 0.
    /// </summary>
    public void CalibrateOffsets()
    {
        if (SerialDataHandler.Instance == null)
        {
            Debug.LogWarning("[SetRotationDigitalTwin] Kalibrierung nicht möglich: SerialDataHandler.Instance ist null.");
            return;
        }

        lowOffset = -SerialDataHandler.Instance.lowAngle;
        middleOffset = -SerialDataHandler.Instance.middleAngle;
        highOffset = -SerialDataHandler.Instance.highAngle;

        Debug.Log($"[SetRotationDigitalTwin] Kalibriert. Offsets: low={lowOffset:F2}, middle={middleOffset:F2}, high={highOffset:F2}");
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f)
            angle += 360f;
        return angle;
    }

    private void ActivateButtonPressedEvent()
    {
        buttonPressedEvent.Invoke();
    }

    private void CheckIfInvokeButtonPressed()
    {
        if (!lastButtonIsPressed && buttonIsPressed)
        {
            ActivateButtonPressedEvent();
        }
    }
}
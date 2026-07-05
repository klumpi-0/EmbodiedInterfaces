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

    [Header("References")]
    [SerializeField] private GameObject lowCube;
    [SerializeField] private GameObject middleCube;
    [SerializeField] private GameObject highCube;

    [Header("Button")]
    [SerializeField] private bool buttonIsPressed;
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

        if (debugRotation)
        {
            SetRotationForCube(lowCube, lowRotation);
            SetRotationForCube(middleCube, middleRotation);
            SetRotationForCube(highCube, highRotation);
        }

        if (useSerialPort)
        {
            // Null-Check: Falls der Port nicht geöffnet werden konnte, soll das
            // hier nicht mit einer NullReferenceException crashen.
            if (SerialDataHandler.Instance == null)
                return;

            SetRotationForCube(lowCube, SerialDataHandler.Instance.lowAngle);
            SetRotationForCube(middleCube, SerialDataHandler.Instance.middleAngle);
            SetRotationForCube(highCube, SerialDataHandler.Instance.highAngle);

            buttonIsPressed = SerialDataHandler.Instance.buttonPressed;

            if (SerialDataHandler.Instance.buttonPressedDown)
            {
                ActivateButtonPressedEvent();
            }
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
            SetRotationForCube(lowCube, low);

        if (float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float middle))
            SetRotationForCube(middleCube, middle);

        if (float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float high))
            SetRotationForCube(highCube, high);

        if (parts.Length >= 4 && parts[3].Trim() == "1")
        {
            ActivateButtonPressedEvent();
        }
    }

    private void SetRotationForCube(GameObject cube, float newRotation)
    {
        if (cube == null)
            return;

        var angles = cube.transform.eulerAngles;
        angles.y = newRotation;
        cube.transform.eulerAngles = angles;
    }

    private void ActivateButtonPressedEvent()
    {
        buttonPressedEvent.Invoke();
    }
}
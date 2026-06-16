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
    public UnityEvent buttonPressedEvent {  get; private set; }
    [SerializeField] private InteractableUnityEventWrapper wrapper;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wrapper.WhenSelect.AddListener(ActivateButtonPressedEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if (debugRotation)
        {
            SetRoationForCube(lowCube, lowRotation);
            SetRoationForCube(middleCube, middleRotation);
            SetRoationForCube(highCube, highRotation);
        }
        if (useSerialPort)
        {
            SetRoationForCube(lowCube, SerialDataHandler.Instance.currentAngle);
            if (SerialDataHandler.Instance.buttonPressedDown) { ActivateButtonPressedEvent(); }
        }
    }

    public void SetRotation(string rotationInput)
    {

    }

    private void SetRoationForCube(GameObject cube, float newRotation)
    {
        var angles = cube.transform.eulerAngles;
        angles.y = newRotation;
        cube.transform.eulerAngles = angles;
    }

    private void ActivateButtonPressedEvent()
    {
        buttonPressedEvent.Invoke();
    }
}

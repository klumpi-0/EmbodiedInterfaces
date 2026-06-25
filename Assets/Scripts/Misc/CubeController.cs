using UnityEngine;
using System.IO.Ports;
using TMPro;

public class CubeController : MonoBehaviour
{
    [Header("Connection Settings")]
    public string portName = "COM3";
    private SerialPort stream;

    [Header("Rotation Settings")]
    public bool snapToSides = false;
    public float snapSpeed = 10f;

    [Header("UI Settings")]
    public TextMeshProUGUI leafStatusText;

    private float currentAngle = 0f;
    private float targetRotationY = 0f;

    // --- NEW: State Tracking Variables ---
    private bool isRotationLocked = false;
    private bool lastButtonState = false;

    // Mapping arrays to assign the correct leaf number to each side
    // Index 0 = Front, 1 = Right, 2 = Back, 3 = Left
    private readonly int[] leafNumberMapping = { 1, 4, 3, 2 };

    void Start()
    {
        stream = new SerialPort(portName, 9600);
        stream.ReadTimeout = 50;

        try
        {
            stream.Open();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Serial Port connection failed: {e.Message}");
        }

        // Ensure the bodyText starts completely hidden when the game begins
        if (leafStatusText != null)
        {
            leafStatusText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (stream != null && stream.IsOpen && stream.BytesToRead > 0)
        {
            try
            {
                string incomingString = stream.ReadLine();
                string[] dataSegments = incomingString.Split(',');

                if (dataSegments.Length == 2)
                {
                    currentAngle = float.Parse(dataSegments[0], System.Globalization.CultureInfo.InvariantCulture);

                    // True if button is currently held down, False if not
                    bool currentButtonState = int.Parse(dataSegments[1]) == 1;

                    // --- TOGGLE LOGIC (Edge Detection) ---
                    // Only trigger if the button is pressed NOW, but wasn't pressed in the previous frame
                    if (currentButtonState == true && lastButtonState == false)
                    {
                        isRotationLocked = !isRotationLocked; // Flip the lock state
                    }

                    // Save the current state for the next frame's comparison
                    lastButtonState = currentButtonState;

                    // --- ROTATION LOGIC ---
                    // Only update the target rotation if the system is UNLOCKED
                    if (!isRotationLocked)
                    {
                        if (snapToSides)
                        {
                            targetRotationY = Mathf.Round(currentAngle / 90f) * 90f;
                        }
                        else
                        {
                            targetRotationY = currentAngle;
                        }
                    }

                    // --- UI VISIBILITY LOGIC ---
                    if (leafStatusText != null)
                    {
                        if (isRotationLocked)
                        {
                            // 1. Unhide the bodyText
                            leafStatusText.gameObject.SetActive(true);

                            // 2. Calculate which side it is locked on
                            int activeSideIndex = (Mathf.RoundToInt(targetRotationY / 90f) % 4 + 4) % 4;
                            int activeLeaf = leafNumberMapping[activeSideIndex];

                            // 3. Update the bodyText
                            leafStatusText.text = $"Leaf {activeLeaf} is chosen.";
                        }
                        else
                        {
                            // Hide the bodyText completely while unlocked and rotating
                            leafStatusText.gameObject.SetActive(false);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Ignore fragmented data safely
            }
        }

        // --- APPLY PHYSICAL MOVEMENT ---
        Quaternion targetQuaternion = Quaternion.Euler(0, -targetRotationY, 0);

        if (snapToSides)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, Time.deltaTime * snapSpeed);
        }
        else
        {
            transform.rotation = targetQuaternion;
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null && stream.IsOpen) stream.Close();
    }
}
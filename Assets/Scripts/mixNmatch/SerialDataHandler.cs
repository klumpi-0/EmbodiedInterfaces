using System;
using System.Globalization;
using System.IO.Ports;
using UnityEngine;

public class SerialDataHandler : MonoBehaviour
{
    public static SerialDataHandler Instance;
    public float currentAngle {  get; private set; }
    public bool buttonPressed {  get; private set; }
    private bool lastButtonPressed;
    public bool buttonPressedDown { get; private set; }

    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;
    private SerialPort serial;

    private void Awake()
    {
        if(Instance  == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.ReadTimeout = 50;
            serial.Open();

            Debug.Log($"Port {portName} geöffnet");
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Öffnen des Ports: {e.Message}");
        }
    }
    private void Update()
    {
        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            string data = serial.ReadLine();
            Debug.Log($"Empfangen: {data}");
            ProcessSerialData(data);
        }
        catch (TimeoutException)
        {
            // Keine Daten verfügbar
        }
        lastButtonPressed = buttonPressed;
    }

    private void OnDestroy()
    {
        if (serial != null && serial.IsOpen)
        {
            serial.Close();
        }
    }
    public void ProcessSerialData(string data)
    {
        // Erwartet Format: WWW,B
        string[] parts = data.Trim().Split(',');

        if (parts.Length != 2)
        {
            Debug.LogWarning($"Ungültiges Datenformat: {data}");
            return;
        }

        if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float angle))
        {
            currentAngle = angle;
        }
        if (parts[1] == "1") { buttonPressed = true; }
        else { buttonPressed = false; }
        //buttonPressed = parts[1] == "1";
        buttonPressedDown = CheckIfButtonPressedDownThisFrame();
        Debug.Log($"Winkel: {currentAngle}°, Button: {buttonPressed}");
    }

    private bool CheckIfButtonPressedDownThisFrame()
    {
        if(buttonPressed != lastButtonPressed && buttonPressed) { return true; }
        return false;
    }
}
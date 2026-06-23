using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialDataHandler : MonoBehaviour
{
    public static SerialDataHandler Instance;

    public float currentAngle { get; private set; }
    public bool buttonPressed { get; private set; }
    private bool lastButtonPressed;
    public bool buttonPressedDown { get; private set; }

    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;

    private SerialPort serial;
    private Thread serialThread;
    private volatile bool isRunning = false;
    private readonly ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.ReadTimeout = 500;
            serial.Open();
            Debug.Log($"Port {portName} geöffnet");

            isRunning = true;
            serialThread = new Thread(ReadSerialLoop);
            serialThread.IsBackground = true;
            serialThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Öffnen des Ports: {e.Message}");
        }
    }

    private void ReadSerialLoop()
    {
        while (isRunning)
        {
            try
            {
                string data = serial.ReadLine();
                dataQueue.Enqueue(data);
            }
            catch (TimeoutException)
            {
                // Keine Daten – normal, weiter warten
            }
            catch (Exception e)
            {
                if (isRunning)
                    Debug.LogError($"Serial-Fehler: {e.Message}");
            }
        }
    }

    private void Update()
    {
        lastButtonPressed = buttonPressed;

        // Nur neueste Nachricht verarbeiten, Queue leeren
        string latestData = null;
        while (dataQueue.TryDequeue(out string data))
            latestData = data;

        if (latestData != null)
            ProcessSerialData(latestData);

        buttonPressedDown = CheckIfButtonPressedDownThisFrame();
    }

    private void ProcessSerialData(string data)
    {
        string[] parts = data.Trim().Split(',');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"Ungültiges Datenformat: {data}");
            return;
        }

        if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float angle))
            currentAngle = angle;

        buttonPressed = parts[1].Trim() == "1";
    }

    private bool CheckIfButtonPressedDownThisFrame()
    {
        return buttonPressed && buttonPressed != lastButtonPressed;
    }

    private void OnDestroy()
    {
        isRunning = false;
        serialThread?.Join(500); // max 500ms warten

        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}
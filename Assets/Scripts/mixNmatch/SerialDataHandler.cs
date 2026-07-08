using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

/// <summary>
/// Liest die (unver�nderte) Arduino-Ausgabe ein. Diese verteilt sich pro Zyklus
/// �ber mehrere Zeilen, z.B.:
///
///   Sensor 1 : 45.23 deg  (Weak Field)
///   Sensor 2 : 12.10 deg
///   Sensor 3 : No Magnet
///   Physical Button: PRESSED
///   ------------------------------
///
/// Das eigentliche Lesen (serial.ReadLine) l�uft in einem Background-Thread,
/// damit blockierende Reads NICHT den Unity-Hauptthread / die Framerate belasten.
/// Im Thread wird NUR gelesen und in eine lock-free Queue geschrieben - kein
/// Debug.Log, kein Parsing dort (das w�rde bei hoher Rate GC-Druck/Spikes erzeugen).
///
/// Im Hauptthread (Update) werden ALLE seit dem letzten Frame angekommenen
/// Zeilen der Reihe nach verarbeitet (nicht nur die letzte!), weil die Werte
/// �ber mehrere Zeilen verteilt sind. Erst wenn die Trennzeile ("------...")
/// gelesen wird, gilt ein Block als vollst�ndig und die Werte werden atomar
/// in die �ffentlichen Properties �bernommen.
/// </summary>
public class SerialDataHandler : MonoBehaviour
{
    public static SerialDataHandler Instance;

    // �ffentlich sichtbare, "committete" Werte (erst nach vollst�ndigem Block g�ltig)
    public float lowAngle { get; private set; }
    public float middleAngle { get; private set; }
    public float highAngle { get; private set; }

    // Abw�rtskompatibel zu vorher (zeigt auf den ersten Sensor)
    public float currentAngle => lowAngle;

    public bool buttonPressed { get; private set; }
    private bool lastButtonPressed;
    public bool buttonPressedDown { get; private set; }

    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 115200;

    private SerialPort serial;
    private Thread serialThread;
    private volatile bool isRunning = false;
    private readonly ConcurrentQueue<string> dataQueue = new ConcurrentQueue<string>();

    // Zwischenspeicher, w�hrend ein Block (Sensor1..3 + Button + Trennzeile) eintrudelt.
    private float pendingLow;
    private float pendingMiddle;
    private float pendingHigh;
    private bool pendingButtonPressed;

    private const string SeparatorPrefix = "------";

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
            Debug.Log($"Port {portName} ge�ffnet");

            isRunning = true;
            serialThread = new Thread(ReadSerialLoop);
            serialThread.IsBackground = true;
            serialThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim �ffnen des Ports: {e.Message}");
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
                // Keine Daten - normal, weiter warten.
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

        // WICHTIG: Hier alle Zeilen abarbeiten (nicht wie fr�her nur die letzte),
        // da ein vollst�ndiger Datensatz �ber mehrere Zeilen verteilt ist.
        while (dataQueue.TryDequeue(out string line))
        {
            ProcessLine(line);
        }

        buttonPressedDown = buttonPressed && buttonPressed != lastButtonPressed;
    }

    private void ProcessLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        line = line.Trim();

        if (line.StartsWith("Sensor 1"))
        {
            pendingLow = ExtractAngle(line, pendingLow);
        }
        else if (line.StartsWith("Sensor 2"))
        {
            pendingMiddle = ExtractAngle(line, pendingMiddle);
        }
        else if (line.StartsWith("Sensor 3"))
        {
            pendingHigh = ExtractAngle(line, pendingHigh);
        }
        else if (line.StartsWith("Physical Button"))
        {
            pendingButtonPressed = line.EndsWith("PRESSED");
        }
        else if (line.StartsWith(SeparatorPrefix))
        {
            // Block vollst�ndig -> alle gesammelten Werte atomar �bernehmen.
            lowAngle = pendingLow;
            middleAngle = pendingMiddle;
            highAngle = pendingHigh;
            buttonPressed = pendingButtonPressed;
        }
        // Andere Zeilen (z.B. die einmalige Startup-Meldung) werden ignoriert.
    }

    /// <summary>
    /// Parst z.B. "Sensor 1 : 45.23 deg  (Weak Field)" -> 45.23f.
    /// Bei "No Magnet" oder einem Parse-Fehler wird der bisherige Wert
    /// beibehalten, damit der digitale Zwilling nicht auf 0 springt.
    /// </summary>
    private float ExtractAngle(string line, float previousValue)
    {
        int colonIndex = line.IndexOf(':');
        if (colonIndex < 0)
            return previousValue;

        string rest = line.Substring(colonIndex + 1).Trim();

        if (rest.StartsWith("No Magnet"))
            return previousValue;

        int degIndex = rest.IndexOf("deg");
        string numberPart = degIndex >= 0 ? rest.Substring(0, degIndex).Trim() : rest;

        if (float.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out float angle))
        {
            return angle;
        }

        Debug.LogWarning($"[SerialDataHandler] Konnte Winkel nicht parsen: \"{line}\"");
        return previousValue;
    }

    private void OnDestroy()
    {
        isRunning = false;
        serialThread?.Join(500);
        if (serial != null && serial.IsOpen)
            serial.Close();
    }
}
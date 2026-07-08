using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Auswertungsskript für den MixNMatch-Puzzle-Flow.
/// Hängt sich an die vorhandenen UnityEvents von MixNMatchController und protokolliert:
/// - Gesamtzeit der Anwendung
/// - Zeit pro Versuch bis ein Match gefunden wurde (inkl. richtig/falsch)
/// - Gesamtzeit pro Phase
///
/// Einfach als Component auf ein (leeres) GameObject in der Szene ziehen.
/// Erwartet, dass MixNMatchController.Instance vor Start() gesetzt ist.
/// </summary>
public class EvaluationController : MonoBehaviour
{
    public static EvaluationController Instance;

    [Header("Einstellungen")]
    [SerializeField] private string fileNamePrefix = "Auswertung";
    [Tooltip("Wenn aktiv, wird jede einzelne Aktion sofort in eine Log-Datei geschrieben (gut zum Debuggen).")]
    [SerializeField] private bool writeLiveLog = true;

    // Interne Zeitmessung
    private float appStartTime;
    private float currentPhaseStartTime;
    private float currentAttemptStartTime;
    private int currentAttemptNumber;

    private string sessionId;
    private string outputFolder;
    private string logFilePath;
    private string summaryFilePath;

    [Serializable]
    public class AttemptRecord
    {
        public int phase;
        public int attemptNumber;
        public bool wasCorrect;
        public float durationSeconds;      // Dauer seit letztem Versuch / Phasenstart
        public float timeStampSinceAppStart;
    }

    [Serializable]
    public class PhaseRecord
    {
        public int phase;
        public float durationSeconds;
        public int totalAttempts;
        public int wrongAttempts;
    }

    private readonly List<AttemptRecord> attempts = new List<AttemptRecord>();
    private readonly List<PhaseRecord> phases = new List<PhaseRecord>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sessionId = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        outputFolder = Path.Combine(Application.persistentDataPath, "Auswertungen");
        Directory.CreateDirectory(outputFolder);

        logFilePath = Path.Combine(outputFolder, $"{fileNamePrefix}_{sessionId}_log.csv");
        summaryFilePath = Path.Combine(outputFolder, $"{fileNamePrefix}_{sessionId}_summary.csv");

        if (writeLiveLog)
        {
            File.WriteAllText(logFilePath, "Zeitstempel(s);Phase;Ereignis;Versuch;Richtig;Dauer(s)\n");
        }
    }

    private void Start()
    {
        appStartTime = Time.realtimeSinceStartup;
        currentPhaseStartTime = appStartTime;
        currentAttemptStartTime = appStartTime;
        currentAttemptNumber = 0;

        if (MixNMatchController.Instance != null)
        {
            var ctrl = MixNMatchController.Instance;
            ctrl.setNewPuzzleEvent.AddListener(OnNewPuzzle);
            ctrl.foundMatchEvent.AddListener(OnFoundMatch);
            ctrl.wrongFeedbackEvent.AddListener(OnWrongFeedback);
            ctrl.plantedPlantEvent.AddListener(OnPlantedPlant);
        }
        else
        {
            Debug.LogWarning("[EvaluationController] MixNMatchController.Instance ist null. " +
                              "Stelle sicher, dass dieses Skript NACH dem MixNMatchController initialisiert wird " +
                              "(z.B. via Script Execution Order), sonst werden keine Events mitgeschnitten.");
        }
    }

    private int CurrentPhase =>
        MixNMatchController.Instance != null && MixNMatchController.Instance.data != null
            ? MixNMatchController.Instance.data.numberPhase
            : -1;

    private void OnNewPuzzle()
    {
        currentPhaseStartTime = Time.realtimeSinceStartup;
        currentAttemptStartTime = currentPhaseStartTime;
        currentAttemptNumber = 0;
        LogLine("NeuePhase", null, null, null);
    }

    private void OnFoundMatch()
    {
        RecordAttempt(true);
    }

    private void OnWrongFeedback()
    {
        RecordAttempt(false);
    }

    private void RecordAttempt(bool wasCorrect)
    {
        currentAttemptNumber++;
        float now = Time.realtimeSinceStartup;
        float duration = now - currentAttemptStartTime;

        var record = new AttemptRecord
        {
            phase = CurrentPhase,
            attemptNumber = currentAttemptNumber,
            wasCorrect = wasCorrect,
            durationSeconds = duration,
            timeStampSinceAppStart = now - appStartTime
        };
        attempts.Add(record);

        LogLine(wasCorrect ? "MatchGefunden" : "FalscherVersuch", currentAttemptNumber, wasCorrect, duration);

        // Zeitmessung für den nächsten Versuch beginnt jetzt
        currentAttemptStartTime = now;
    }

    private void OnPlantedPlant()
    {
        float now = Time.realtimeSinceStartup;
        float phaseDuration = now - currentPhaseStartTime;
        int phase = CurrentPhase;

        int totalAttemptsThisPhase = attempts.FindAll(a => a.phase == phase).Count;
        int wrongAttemptsThisPhase = attempts.FindAll(a => a.phase == phase && !a.wasCorrect).Count;

        phases.Add(new PhaseRecord
        {
            phase = phase,
            durationSeconds = phaseDuration,
            totalAttempts = totalAttemptsThisPhase,
            wrongAttempts = wrongAttemptsThisPhase
        });

        LogLine("PhaseAbgeschlossen", null, null, phaseDuration);
    }

    private void LogLine(string ereignis, int? versuch, bool? richtig, float? dauer)
    {
        if (!writeLiveLog) return;

        float timestamp = Time.realtimeSinceStartup - appStartTime;
        string line = string.Format("{0:F2};{1};{2};{3};{4};{5}\n",
            timestamp,
            CurrentPhase,
            ereignis,
            versuch.HasValue ? versuch.Value.ToString() : "",
            richtig.HasValue ? (richtig.Value ? "Ja" : "Nein") : "",
            dauer.HasValue ? dauer.Value.ToString("F2") : "");

        try
        {
            File.AppendAllText(logFilePath, line);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[EvaluationController] Konnte Log nicht schreiben: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        WriteSummary();
    }

    /// <summary>
    /// Schreibt die Gesamtauswertung. Kann auch manuell aufgerufen werden,
    /// z.B. über einen "Beenden"-Button, falls die App nicht sauber geschlossen wird.
    /// </summary>
    public void WriteSummary()
    {
        float totalAppTime = Time.realtimeSinceStartup - appStartTime;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== Gesamtauswertung ===");
        sb.AppendLine($"Gesamtzeit der Anwendung (s);{totalAppTime:F2}");
        sb.AppendLine();
        sb.AppendLine("Phase;Dauer(s);AnzahlVersucheGesamt;AnzahlFalscheVersuche");
        foreach (var p in phases)
        {
            sb.AppendLine($"{p.phase};{p.durationSeconds:F2};{p.totalAttempts};{p.wrongAttempts}");
        }
        sb.AppendLine();
        sb.AppendLine("Phase;Versuch;Richtig;Dauer(s);ZeitpunktSeitStart(s)");
        foreach (var a in attempts)
        {
            sb.AppendLine($"{a.phase};{a.attemptNumber};{(a.wasCorrect ? "Ja" : "Nein")};{a.durationSeconds:F2};{a.timeStampSinceAppStart:F2}");
        }

        try
        {
            File.WriteAllText(summaryFilePath, sb.ToString());
            Debug.Log("[EvaluationController] Auswertung gespeichert unter: " + summaryFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("[EvaluationController] Konnte Auswertung nicht speichern: " + e.Message);
        }
    }
}

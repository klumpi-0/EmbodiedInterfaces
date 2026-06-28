using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ProgressFieldController : MonoBehaviour
{
    [Range(0,3)] private int progress;
    [Range(0,3)] private int Progress
    {
        get => progress;
        set
        {
            if(progress == value) return;
            progress = value;
            ChangedProgress(progress);
        }
    }
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private TextMeshProUGUI activityText;
    [SerializeField] private Image image;
    private string[] activityNames = { "Mix'N'Match", "Pflanzen", "Informationen", "Weiter" };
    [SerializeField] private Sprite[] sprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image.preserveAspect = true;
        ChangedProgress(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Progress = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Progress = (Progress + 1)%4;
        }
    }

    private void ChangeToProgress(int progress)
    {
        this.progress = progress;
        SetStepText(progress);
        SetTask(activityNames[progress]);
        SetIcon(sprites[progress]);
    }

    private void ChangedProgress(int newProgress)
    {
        ChangeToProgress(newProgress);
    }

    private void SetStepText(int step)
    {
        stepText.text = $"Step {step + 1} / 4 |";
    }

    private void SetTask(string task)
    {
        activityText.text = task ;
    }

    private void SetIcon(Sprite icon)
    {
        image.sprite = icon ;
    }
}

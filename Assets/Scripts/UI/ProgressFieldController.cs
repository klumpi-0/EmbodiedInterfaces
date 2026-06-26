using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ProgressFieldController : MonoBehaviour
{
    [SerializeField][Range(0,3)] private int progress;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private TextMeshProUGUI activityText;
    [SerializeField] private Image image;
    private string[] activityNames = { "Drehen", "Pflanzen", "Informationen", "Weiter" };
    [SerializeField] private Sprite[] sprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToProgress(int progress)
    {
        this.progress = progress;
        SetStepText(progress);
        SetTask(activityNames[progress]);
        SetIcon(sprites[progress]);
    }

    private void SetStepText(int step)
    {
        stepText.text = $"Step {step} / 4 |";
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

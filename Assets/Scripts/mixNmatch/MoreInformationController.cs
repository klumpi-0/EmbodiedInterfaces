using TMPro;
using UnityEngine;

public class MoreInformationController : MonoBehaviour
{
    public static MoreInformationController Instance;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] lowCubeTexts;
    [SerializeField] private TextMeshProUGUI[] middleCubeTexts;
    [SerializeField] private TextMeshProUGUI[] highCubeTexts;

    [Header("Text")]
    [SerializeField] private string lowText;
    [SerializeField] private string middleText;
    [SerializeField] private string highText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ProcessRoations.Instance.newFrontSideLowEvent.AddListener(EnableTextLow);
        ProcessRoations.Instance.newFrontSideMiddleEvent.AddListener(EnableTextMiddle);
        ProcessRoations.Instance.newFrontSideHighEvent.AddListener(EnableTextHigh);
        ProcessRoations.Instance.leftFrontFacingSideEvent.AddListener(DisableTextOnCube);
    }

    public void SetAndUpdateNewText(string lowText, string middleText, string highText)
    {
        this.lowText = lowText;
        this.middleText = middleText;
        this.highText = highText;
        UpdateTextOnGui(lowText, lowCubeTexts);
        UpdateTextOnGui(middleText, middleCubeTexts);
        UpdateTextOnGui(highText, highCubeTexts);
    }

    private void UpdateTextOnGui(string text, TextMeshProUGUI[] guiElements)
    {
        foreach(var element in guiElements)
        {
            element.text = text;
        }
    }

    private void EnableTextLow(int numberSide)
    {
        lowCubeTexts[numberSide].gameObject.SetActive(true);
    }
    private void EnableTextMiddle(int numberSide)
    {
        middleCubeTexts[numberSide].gameObject.SetActive(true);
    }
    private void EnableTextHigh(int numberSide)
    {
        highCubeTexts[numberSide].gameObject.SetActive(true);
    }

    private void DisableAllTextOnCube(TextMeshProUGUI[] cubeTexts)
    {
        foreach (var text in cubeTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    private void DisableTextOnCube(int cubeNumber)
    {
        switch (cubeNumber) 
        {
            case 0: DisableAllTextOnCube(lowCubeTexts); break;
            case 1: DisableAllTextOnCube(middleCubeTexts); break;
            case 2: DisableAllTextOnCube(highCubeTexts); break;
        }
    }
}

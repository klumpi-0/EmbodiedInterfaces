using TMPro;
using UnityEngine;

public class MoreInformationController : MonoBehaviour
{
    public static MoreInformationController Instance;

    [SerializeField] private bool moreInformationIsActive;

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
        ProcessRoations.Instance.leftFrontFacingSideEvent.AddListener(DisableAllText);
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
        if (!moreInformationIsActive) { return; }
        lowCubeTexts[numberSide].gameObject.SetActive(true);
    }
    private void EnableTextMiddle(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        middleCubeTexts[numberSide].gameObject.SetActive(true);
    }
    private void EnableTextHigh(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        highCubeTexts[numberSide].gameObject.SetActive(true);
    }

    private void DisableTextOnOneCube(TextMeshProUGUI[] cubeTexts)
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
            case 0: DisableTextOnOneCube(lowCubeTexts); break;
            case 1: DisableTextOnOneCube(middleCubeTexts); break;
            case 2: DisableTextOnOneCube(highCubeTexts); break;
        }
    }

    public void DisableAllText(int randomInt)
    {
        DisableTextOnOneCube(lowCubeTexts);
        DisableTextOnOneCube(middleCubeTexts);
        DisableTextOnOneCube(highCubeTexts);
    }

    public void SetMoreInformationIsActive(bool isActive)
    {
        moreInformationIsActive = isActive;
    }
}

using TMPro;
using UnityEngine;

public class MoreInformationController : MonoBehaviour
{
    public static MoreInformationController Instance;

    [SerializeField] private bool moreInformationIsActive;
    [SerializeField] private int[] arrowPositions;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] lowCubeTexts;
    [SerializeField] private TextMeshProUGUI[] middleCubeTexts;
    [SerializeField] private TextMeshProUGUI[] highCubeTexts;

    [Header("Text")]
    [SerializeField] private string lowText;
    [SerializeField] private string middleText;
    [SerializeField] private string highText;
    [SerializeField] private string arrowText;

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

    public void SetAndUpdateNewText(string lowText, string middleText, string highText, int[] newArrowPositions)
    {
        this.arrowPositions = newArrowPositions;
        this.lowText = lowText;
        this.middleText = middleText;
        this.highText = highText;
        UpdateTextOnGui(lowText, lowCubeTexts, arrowPositions[0]);
        UpdateTextOnGui(middleText, middleCubeTexts, arrowPositions[1]);
        UpdateTextOnGui(highText, highCubeTexts, arrowPositions[2]);
    }

    private void UpdateTextOnGui(string text, TextMeshProUGUI[] guiElements, int arrowPos)
    {
        int index = 0;
        foreach(var element in guiElements)
        {
            element.text = text;
            if(index == arrowPos)
            {
                element.text = arrowText;
            }
            index++;
        }
    }

    private void EnableTextLow(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        lowCubeTexts[numberSide].gameObject.SetActive(true);
        if(numberSide != arrowPositions[0])
        {
            MNM_AudioController.Instance.PlayMoreInfoClip(0);
        }
    }
    private void EnableTextMiddle(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        middleCubeTexts[numberSide].gameObject.SetActive(true);
        if (numberSide != arrowPositions[1])
        {
            MNM_AudioController.Instance.PlayMoreInfoClip(1);
        }
    }
    private void EnableTextHigh(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        highCubeTexts[numberSide].gameObject.SetActive(true);
        if (numberSide != arrowPositions[2])
        {
            MNM_AudioController.Instance.PlayMoreInfoClip(2);
        }
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

    public void SetArrowPositions(int[] newArrowPositions)
    {
        this.arrowPositions = newArrowPositions;
    }
}

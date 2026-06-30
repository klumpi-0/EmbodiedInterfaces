using Oculus.Interaction;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MoreInformationController : MonoBehaviour
{
    public static MoreInformationController Instance;

    [SerializeField] private bool moreInformationIsActive;
    [SerializeField] private int[] arrowPositions;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI[] lowCubeTexts;
    [SerializeField] private TextMeshProUGUI[] middleCubeTexts;
    [SerializeField] private TextMeshProUGUI[] highCubeTexts;
    [SerializeField] private GameObject lowPos;
    [SerializeField] private GameObject middlePos;
    [SerializeField] private GameObject highPos;

    [Header("Text")]
    [SerializeField] private string lowText;
    [SerializeField] private string middleText;
    [SerializeField] private string highText;
    [SerializeField] private string arrowText;

    [Header("All Information received")]
    [SerializeField] private bool lowInformationReceived;
    [SerializeField] private bool middleInformationReceived;
    [SerializeField] private bool highInformationReceived;
    [SerializeField] private bool firedEventInProcess;


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

    private void Update()
    {
        if(lowInformationReceived && middleInformationReceived && highInformationReceived && !firedEventInProcess)
        {
            firedEventInProcess = true;
            MixNMatchController.Instance.InitAllInformationReceivedEvent();
        }
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
            element.text = "";
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
            //MNM_AudioController.Instance.PlayMoreInfoClip(0);
            float delay = 2f;
            SetInformationReceivedDelayed(0, MNM_AudioController.Instance.moreInfo_01.length + delay);
            FloraController.Instance.MoveMultiple(
                new Transform[] { lowPos.transform },
                new AudioClip[] { MNM_AudioController.Instance.moreInfo_01 },
                new FloraStates[] { FloraStates.Waiting },
                new string[] { "Wurzel" },
                new string[] { lowText },
                duration: delay);
        }
    }
    private void EnableTextMiddle(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        middleCubeTexts[numberSide].gameObject.SetActive(true);
        if (numberSide != arrowPositions[1])
        {
            //MNM_AudioController.Instance.PlayMoreInfoClip(1);
            float delay = 2f;
            SetInformationReceivedDelayed(1, MNM_AudioController.Instance.moreInfo_02.length + delay);
            FloraController.Instance.MoveMultiple(
                new Transform[] { middlePos.transform },
                new AudioClip[] { MNM_AudioController.Instance.moreInfo_02 },
                new FloraStates[] { FloraStates.Waiting },
                new string[] { "Stamm" },
                new string[] { middleText },
                duration: delay);
        }
    }
    private void EnableTextHigh(int numberSide)
    {
        if (!moreInformationIsActive) { return; }
        highCubeTexts[numberSide].gameObject.SetActive(true);
        if (numberSide != arrowPositions[2])
        {
            //MNM_AudioController.Instance.PlayMoreInfoClip(2);
            float delay = 2f;
            SetInformationReceivedDelayed(2, MNM_AudioController.Instance.moreInfo_03.length + delay);
            FloraController.Instance.MoveMultiple(
                new Transform[] { highPos.transform },
                new AudioClip[] { MNM_AudioController.Instance.moreInfo_03 },
                new FloraStates[] { FloraStates.Waiting },
                new string[] { "Blüte" },
                new string[] { highText },
                duration: delay);
        }
    }

    private void DisableTextOnOneCube(TextMeshProUGUI[] cubeTexts)
    {
        foreach (var text in cubeTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    private void SetInformationReceivedDelayed(int whichCube, float delay)
    {
        StartCoroutine(SetInformationReceivedDelayedCoroutine(whichCube, delay));
    }

    private IEnumerator SetInformationReceivedDelayedCoroutine(int whichCube, float delay)
    {
        yield return new WaitForSeconds(delay);
        switch(whichCube)
        {
            case 0:lowInformationReceived = true; break;
            case 1:middleInformationReceived = true;break;
            case 2:highInformationReceived = true; break;
        }
    }

    public void SetAllInformationReceivedFalse()
    {
        lowInformationReceived = false;
        middleInformationReceived = false;
        highInformationReceived = false;
        firedEventInProcess = false;
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="randomInt">Is never used just put anything into it</param>
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

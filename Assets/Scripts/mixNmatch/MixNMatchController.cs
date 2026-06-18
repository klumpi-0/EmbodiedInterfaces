using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MixNMatchController : MonoBehaviour
{
    public static MixNMatchController Instance;

    [Header("Settings")]
    public PuzzleData data;
    [SerializeField] private Sprite[] lowSprites;
    [SerializeField] private Sprite[] middleSprites;
    [SerializeField] private Sprite[] highSprites;
    [SerializeField] private int[] solution;

    [Header("References")]
    [SerializeField] private SetImagesOnMixNMatch imageSetter;
    [SerializeField] private ProcessRoations processRotation;
    [SerializeField] private GameObject currentFollowUpObject;
    [SerializeField] private GameObject[] plantBubbles;

    [Header("Events")]
    [Tooltip("Gets Invoked when user locks in correct solution for mNm puzzle")]
    public UnityEvent foundMatchEvent;
    [Tooltip("Gets invoked if user needs to get wrong feedback")]
    public UnityEvent wrongFeedbackEvent;
    [Tooltip("Gets invoked when user planted plant into final position")]
    public UnityEvent plantedPlantEvent;
    [Tooltip("Gets invoked when user locked in the forward arrows")]
    public UnityEvent forwardArrowsEvent;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foundMatchEvent.AddListener(ActivatePlantBubble);

        plantedPlantEvent.AddListener(SetBubbleFinished);
        plantedPlantEvent.AddListener(StartFlowerWaveGround);
        plantedPlantEvent.AddListener(SetupFollowUpInformation);

        DeactivateAllBubbles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNewPuzzleData(PuzzleData data, bool allreadySolved = false)
    {
        this.data = data;
        if (!allreadySolved)
        {
            this.data.solvedPuzzle = false;
            this.data.plantedPlant = false;
            MoreInformationController.Instance.SetMoreInformationIsActive(false);
        }
        (lowSprites, middleSprites, highSprites, solution) = CreateMixNMatchFill.Instance.CreateNewFill(this.data);
        processRotation?.SetTargetRotation(data.correctSolutionSites);
        imageSetter?.ApplyImages(lowSprites, middleSprites, highSprites);
        ProcessRoations.Instance?.SetTargetRotation(solution);
        ProcessRoations.Instance.SetTargetRotationArrows(data.arrowSolutionSites);
        MNM_AudioController.Instance?.SetAudioFiles(data.introClip, data.finishedPuzzleClip, data.morInfo_01Clip, data.morInfo_02Clip, data.morInfo_03Clip);
        MNM_AudioController.Instance.PlayIntroClip();
        FollowTaskController.Instance?.SetCurrentFollowTask(data.grabbablePrefab);
        MoreInformationController.Instance.SetAndUpdateNewText(data.lowInfoText, data.middleInfoText, data.highInfoText, data.arrowSolutionSites);
        MoreInformationController.Instance.DisableAllText(1);
    }

    private void StartFlowerWaveGround()
    {
        FlowerWaveSpawner.Instance.StartWave(this.data.groundFlowerPrefab, this.data.shrinkChanceGround);
    }

    private void SetupFollowUpInformation()
    {

        //SetImagesOnMixNMatch.Instance.ApplySingleImageToAllSides(data.correctSprites);
        SetImagesOnMixNMatch.Instance.SetupMoreInformationImages(data.arrowSolutionSites, data.correctSprites);
        MoreInformationController.Instance.SetAndUpdateNewText(data.lowInfoText, data.middleInfoText, data.highInfoText, data.arrowSolutionSites);
        MoreInformationController.Instance.SetMoreInformationIsActive(true);
    }

    #region Bubble Stuff
    private void ActivatePlantBubble()
    {
        var currentBubble = plantBubbles[data.numberPhase];
        currentBubble.SetActive(true);
        var logic = currentBubble.GetComponent<PlantInBubbleLogic>();
        logic.SetupPlantBubble(currentFollowUpObject, currentFollowUpObject.GetComponent<Grabbable>());
    }


    private void SetBubbleFinished()
    {
        plantBubbles[data.numberPhase].GetComponent<PlantInBubbleLogic>().enabled = false;
    }
    /// <summary>
    /// Gets called if bubble has flower in it
    /// </summary>

    private void DeactivateAllBubbles()
    {
        foreach(GameObject bubble in plantBubbles)
        {
            bubble.SetActive(false);
        }
    }

    public void SetCurrentFollowUpObject(GameObject followObject)
    {
        currentFollowUpObject = followObject;
    }
    #endregion
    /// <summary>
    /// Gets called if user pressed button and found the correct mesh
    /// </summary>
    public void InitFoundMatchEvent()
    {
        if (!data.solvedPuzzle)
        {
            // User solved the different task puzzles
            Debug.Log("Found match");
            data.solvedPuzzle = true;
            foundMatchEvent.Invoke();
        }
    }

    public void InitForwardArrowsEvent()
    {
        if (data.solvedPuzzle)
        {
            // User just alligned the forward arrows and pressed the button
            Debug.Log("Skip to next task");
            forwardArrowsEvent.Invoke();
        }
    }

    public void InitWrongFeedbackEvent()
    {
        wrongFeedbackEvent.Invoke();
    }

    public void ForceFoundMatchEvent()
    {
        foundMatchEvent.Invoke();
    }

    public void InitPlantedEvent(float delay)
    {
        StartCoroutine(CoroutinePlantEvent(delay));
    }

    private IEnumerator CoroutinePlantEvent(float delay)
    {
        Debug.Log("Invoked planted plant event");
        yield return new WaitForSeconds(delay);
        plantedPlantEvent.Invoke();
    }

}

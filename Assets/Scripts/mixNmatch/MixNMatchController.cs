using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MixNMatchController : MonoBehaviour
{
    public static MixNMatchController Instance;

    [Header("Settings")]
    [SerializeField] private PuzzleData data;
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
    [Tooltip("Gets invoked when user planted plant into final position")]
    public UnityEvent plantedPlantEvent;


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
        }
        (lowSprites, middleSprites, highSprites, solution) = CreateMixNMatchFill.Instance.CreateNewFill(this.data);
        processRotation?.SetTargetRotation(data.correctSolutionSites);
        imageSetter?.ApplyImages(lowSprites, middleSprites, highSprites);
        ProcessRoations.Instance?.SetTargetRotation(solution);
        MNM_AudioController.Instance?.SetAudioFiles(data.introClip, data.finishedPuzzleClip, data.morInfo_01Clip, data.morInfo_02Clip, data.morInfo_03Clip);
        FollowTaskController.Instance?.SetCurrentFollowTask(data.grabbablePrefab);
    }

    private void StartFlowerWaveGround()
    {
        FlowerWaveSpawner.Instance.StartWave(this.data.groundFlowerPrefab, this.data.shrinkChanceGround);
    }

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

    public void InitFoundMatchEvent()
    {
        if(!data.solvedPuzzle)
        {
            Debug.Log("Found match");
            data.solvedPuzzle = true;
            foundMatchEvent.Invoke();
        }
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

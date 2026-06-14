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
    [SerializeField] private GameObject plantBubble_1;
    [SerializeField] private GameObject plantBubble_2;
    [SerializeField] private GameObject plantBubble_3;
    [SerializeField] private GameObject plantBubble_4;

    [Header("Events")]
    public UnityEvent foundMatchEvent;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNewPuzzleData(PuzzleData data)
    {
        this.data = data;
        (lowSprites, middleSprites, highSprites, solution) = CreateMixNMatchFill.Instance.CreateNewFill(this.data);
        processRotation?.SetTargetRotation(data.correctSolutionSites);
        imageSetter?.ApplyImages(lowSprites, middleSprites, highSprites);
        ProcessRoations.Instance.SetTargetRotation(solution);
        //FollowTaskController.Instance?.SetCurrentFollowTask(data.prefabTask);
        MNM_AudioController.Instance?.SetAudioFiles(data.introClip, data.finishedPuzzleClip, data.morInfo_01Clip, data.morInfo_02Clip, data.morInfo_03Clip);
    }

    public void InitFoundMatchEvent()
    {
        if(!data.finishedPuzzle)
        {
            Debug.Log("Found match");
            data.finishedPuzzle = true;
            foundMatchEvent.Invoke();
        }
    }

    public void ForceFoundMatchEvent()
    {
        foundMatchEvent.Invoke();
    }

}

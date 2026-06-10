using UnityEngine;
using UnityEngine.Events;

public class MixNMatchController : MonoBehaviour
{
    public static MixNMatchController Instance;

    [Header("Settings")]
    [SerializeField] private PuzzleData data;


    [Header("References")]
    [SerializeField] private SetImagesOnMixNMatch imageSetter;
    [SerializeField] private ProcessRoations processRotation;

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
        processRotation.SetTargetRotation(data.correctSolutionSites);
        imageSetter.ApplyImages(data);
        FollowTaskController.Instance.SetCurrentFollowTask(data.prefabTask);
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

}

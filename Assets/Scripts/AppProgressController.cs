using UnityEngine;

public class AppProgressController : MonoBehaviour
{
    [SerializeField] private int progress;
    [SerializeField] private PuzzleData[] puzzleDatas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadFirstTask();
        MixNMatchController.Instance.forwardArrowsEvent.AddListener(SkipToNextTask);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            LoadFirstTask();
        }
    }

    public void LoadFirstTask()
    {
        MixNMatchController.Instance.SetNewPuzzleData(puzzleDatas[0]);
    }

    public void SkipToNextTask()
    {
        if(progress + 1 >= puzzleDatas.Length) { return; }
        MixNMatchController.Instance.SetNewPuzzleData(puzzleDatas[progress + 1]);
        progress = progress + 1;
    }
}

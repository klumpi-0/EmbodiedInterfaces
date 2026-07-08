using UnityEngine;

public class AppProgressController : MonoBehaviour
{
    [SerializeField] private int progress;
    [SerializeField] private PuzzleData[] puzzleDatas;
    [SerializeField] private Transform flowerParent;
    [SerializeField] private Transform grapParent;
    [SerializeField] private Transform bubbleParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //LoadFirstTask();
        MixNMatchController.Instance.forwardArrowsEvent.AddListener(SkipToNextTask);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.R))
        {
            ResetToFirstTask();
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

    private void ResetToFirstTask()
    {
        LoadFirstTask();
        // flower delete
        foreach(Transform flower in flowerParent)
        {
            Destroy(flower.gameObject);
        }
        // grab delete
        foreach(Transform grap in grapParent)
        {
            Destroy(grap.gameObject);
        }
        // deactivete bubbles
        foreach (Transform bubble in bubbleParent)
        {
            bubble.gameObject.SetActive(false);
        }
        progress = 0;
        EvaluationController.Instance.WriteSummary();
    }
}

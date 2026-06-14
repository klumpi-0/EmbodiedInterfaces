using UnityEngine;
using UnityEngine.UI;

public class TestChangeImage : MonoBehaviour
{
    public PuzzleData puzzleData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        puzzleData.solvedPuzzle = false;
        MixNMatchController.Instance.SetNewPuzzleData(puzzleData);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

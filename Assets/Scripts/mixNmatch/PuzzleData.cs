using UnityEngine;

public enum ProgressMNM
{
    
}

[CreateAssetMenu(fileName = "PuzzleData", menuName = "Puzzle/Puzzle Data")]
public class PuzzleData : ScriptableObject
{
    [Header("Low Cube")]
    public Sprite[] lowSprites = new Sprite[4];

    [Header("Middle Cube")]
    public Sprite[] middleSprites = new Sprite[4];

    [Header("High Cube")]
    public Sprite[] highSprites = new Sprite[4];

    [Header("Correct Solution")]
    public int[] correctSolutionSites = new int[3];
    public bool finishedPuzzle = false;

    [Header("Prefab for Task afterwards")]
    public GameObject prefabTask;

    [Header("AudioClips")]
    public AudioClip introClip;
    public AudioClip finishedPuzzleClip;
    public AudioClip morInfo_01Clip;
    public AudioClip morInfo_02Clip;
    public AudioClip morInfo_03Clip;
}
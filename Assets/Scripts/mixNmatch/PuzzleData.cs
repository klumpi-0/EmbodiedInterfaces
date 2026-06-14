using System;
using UnityEngine;

public enum ProgressMNM
{
    
}

[CreateAssetMenu(fileName = "PuzzleData", menuName = "Puzzle/Puzzle Data")]
public class PuzzleData : ScriptableObject
{
    [Header("Images")]
    [Tooltip("Images go from low to high (0 = low, 1 = middle, 2 = )")]
    public Sprite[] correctSprites = new Sprite[3];

    [Header("Diversion Images")]
    public Sprite[] lowSprites = new Sprite[3];
    public Sprite[] middleSprites = new Sprite[3];
    public Sprite[] highSprites = new Sprite[3];

    [Header("Correct Solution")]
    [Tooltip("Gets created by Unity during runtime")]
    public int[] correctSolutionSites = new int[3];
    public bool finishedPuzzle = false;

    [Header("Prefab for Task afterwards")]
    [Obsolete]
    public GameObject prefabTask;

    [Header("Flower spawn afterwards")]
    public Mesh flowerMesh;
    public Material flowerMaterial;
    public GameObject flowerPrefab;


    [Header("AudioClips")]
    public AudioClip introClip;
    public AudioClip finishedPuzzleClip;
    public AudioClip morInfo_01Clip;
    public AudioClip morInfo_02Clip;
    public AudioClip morInfo_03Clip;

    [Header("Text")]
    public string finishedText;
    public string highInfoText;
    public string middleInfoText;
    public string lowInfoText;
}
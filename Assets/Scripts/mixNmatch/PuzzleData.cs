using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum ProgressMNM
{
    
}

[CreateAssetMenu(fileName = "PuzzleData", menuName = "Puzzle/Puzzle Data")]
public class PuzzleData : ScriptableObject
{
    [Header("Overall Information")]
    public string namePhase;
    [Tooltip("Is manly used to activate the correct plantBubble")]
    public int numberPhase;

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
    public int[] arrowSolutionSites = new int[3];
    public bool solvedPuzzle = false;
    public bool plantedPlant = false;

    [Header("Flower spawn afterwards")]
    public GameObject grabbablePrefab;
    public GameObject groundFlowerPrefab;
    public float shrinkChanceGround;


    [Header("AudioClips")]
    public AudioClip introClip;
    public AudioClip finishedPuzzleClip;
    public AudioClip afterPlantAudio;
    public AudioClip morInfo_01Clip;
    public AudioClip morInfo_02Clip;
    public AudioClip morInfo_03Clip;

    [Header("Text")]
    public string finishedText;
    public string highInfoText;
    public string middleInfoText;
    public string lowInfoText;

    [Header("Intro")]
    public int[] intro_transforms;
    public AudioClip[] intro_clips;
    public FloraStates[] intro_states;

    [Header("Plant")]
    public int[] plant_transforms;
    public AudioClip[] plant_clips;
    public FloraStates[] plant_states;

    [Header("MoreInformationen")]
    public int[] moreInfo_transforms;
    public AudioClip[] moreInfo_clips;
    public FloraStates[] moreInfo_states;

    [Header("Weiter")]
    public int[] weiter_transforms;
    public AudioClip[] weiter_clips;
    public FloraStates[] weiter_states;

    public (Transform[], AudioClip[], FloraStates[]) GetIntroValues()
    {
        return (SceneTargets.Instance.GetTargets(intro_transforms), intro_clips, intro_states);
    }

    public (Transform[], AudioClip[], FloraStates[]) GetPlantValues()
    {
        return (SceneTargets.Instance.GetTargets(plant_transforms), plant_clips, plant_states);
    }

    public (Transform[], AudioClip[], FloraStates[]) GetMoreInformationValues()
    {
        return (SceneTargets.Instance.GetTargets(moreInfo_transforms), moreInfo_clips, moreInfo_states);
    }

    public (Transform[], AudioClip[], FloraStates[]) GetWeiterValues()
    {
        return (SceneTargets.Instance.GetTargets(weiter_transforms), weiter_clips, weiter_states);
    }
}
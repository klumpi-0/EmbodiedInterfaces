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
    [TextArea] public string highInfoText;
    [TextArea] public string middleInfoText;
    [TextArea] public string lowInfoText;

    [Header("Intro")]
    public int[] intro_transforms;
    public string[] intro_header;
    [TextArea] public string[] intro_texts;
    public AudioClip[] intro_clips;
    public FloraStates[] intro_states;

    [Header("Plant")]
    public int[] plant_transforms;
    public string[] plant_header;
    [TextArea] public string[] plant_texts;
    public AudioClip[] plant_clips;
    public FloraStates[] plant_states;

    [Header("MoreInformationen")]
    public int[] moreInfo_transforms;
    public string[] moreInfo_header;
    [TextArea] public string[] moreInfo_texts;
    public AudioClip[] moreInfo_clips;
    public FloraStates[] moreInfo_states;

    [Header("Weiter")]
    public int[] weiter_transforms;
    public string[] weiter_header;
    [TextArea]public string[] weiter_texts;
    public AudioClip[] weiter_clips;
    public FloraStates[] weiter_states;

    public (Transform[], AudioClip[], FloraStates[], string[], string[]) GetIntroValues()
    {
        return (SceneTargets.Instance.GetTargets(intro_transforms), intro_clips, intro_states, intro_header, intro_texts);
    }

    public (Transform[], AudioClip[], FloraStates[], string[], string[]) GetPlantValues()
    {
        return (SceneTargets.Instance.GetTargets(plant_transforms), plant_clips, plant_states, plant_header, plant_texts);
    }

    public (Transform[], AudioClip[], FloraStates[], string[], string[]) GetMoreInformationValues()
    {
        return (SceneTargets.Instance.GetTargets(moreInfo_transforms), moreInfo_clips, moreInfo_states, moreInfo_header, moreInfo_texts);
    }

    public (Transform[], AudioClip[], FloraStates[], string[], string[]) GetWeiterValues()
    {
        return (SceneTargets.Instance.GetTargets(weiter_transforms), weiter_clips, weiter_states, weiter_header, weiter_texts);
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public enum FloraStates
{
    Waiting,
    Moving,
    Edge,
    Spinning,
    Pushing
}

public class FloraController : MonoBehaviour
{
    public static FloraController Instance;

    [Header("References")]
    public GameObject floraMoveObject;
    [SerializeField] private InfoBoxController textField;
    [Header("Events")]
    public UnityEvent startedMovingEvent;
    public UnityEvent finishedMovingEvent;
    public UnityEvent finishedMultipleEvent;
    [Header("Settings")]
    [SerializeField] private Vector3 offsetVector;

    [Header("Debug")]
    [SerializeField] private GameObject debugTarget;
    [SerializeField] private AudioClip debugClip;
    [SerializeField] private Transform[] debugTargets;
    [SerializeField] private AudioClip[] debugClips;
    [SerializeField] private FloraStates[] debugStates;

    [Header("LastFloraInput")]
    [SerializeField] private Transform[] lastTransforms;
    [SerializeField] private AudioClip[] lastClips;
    [SerializeField] private FloraStates[] lastStates;

    [Header("MoveTargets")]
    [SerializeField] private Transform mixNmatchTarget;
    [SerializeField] private Transform buttonTarget;
    [SerializeField] private Transform restTarget;
    [SerializeField] private Transform[] plantBubbleTargets;
 
    [Header("State Stuff")]
    public Action<FloraStates> OnStateChangeAction;
    private FloraStates floraState;
    public FloraStates FloraState
    {
        get => floraState;
        set
        {
            if (floraState == value)
                return;
            floraState = value;
            ChangedState(floraState);
        }
    }

    [SerializeField] private bool isMoving;

    private void Awake()
    {
        if(Instance  == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floraMoveObject.transform.position = Vector3.zero;
        startedMovingEvent.AddListener(SetIsMovingTrue);
        finishedMovingEvent.AddListener(SetIsMovingFalse);
        // Listeners for App Progress Events
        MixNMatchController.Instance.setNewPuzzleEvent.AddListener(PlayIntro);
        MixNMatchController.Instance.foundMatchEvent.AddListener(PlayPlanting);
        MixNMatchController.Instance.plantedPlantEvent.AddListener(PlayMoreInformation);
        MixNMatchController.Instance.AllInformationReceivedEvent.AddListener(PlayWeiter);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            //MoveFloraAndPlayClip(debugTarget.transform, debugClip, playClipDelayed:true, stateAfterMove:FloraStates.Spinning);
            MoveMultiple(debugTargets, debugClips, debugStates);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            FloraSpinAnimation.Instance.TriggerSpin();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            FloraButtonPress.Instance.StopRepeatAnimationPlaying();
        }
    }

    #region Move Methods

    public void MoveFlora(Vector3 targetPos, Vector3 targetRot, float duration = 2f, bool useOffset = false, FloraStates? stateAfterMove = null)
    {
        startedMovingEvent.Invoke();
        SmoothMover mover = floraMoveObject.AddComponent<SmoothMover>();
        if (useOffset) { targetPos = targetPos + offsetVector; }
        mover.Init(targetPos, targetRot, duration);
        mover.atFinalTransformEvent.AddListener(InvokeFinishedMovingEvent);
        if (stateAfterMove.HasValue)
        {
            FloraStates targetState = stateAfterMove.Value;
            mover.atFinalTransformEvent.AddListener(() => FloraState = targetState); 
        }
    }

    public void MoveFlora(Transform target, float duration = 2f, bool useOffset = false, FloraStates? stateAfterMove = null)
    {
        MoveFlora(target.position, target.rotation.eulerAngles, duration, useOffset, stateAfterMove);
    }

    public void MoveFloraAndPlayClip(Vector3 targetPos, Vector3 targetRot, AudioClip clip, float duration = 2f, bool useOffset = false, bool playClipDelayed = false, FloraStates? stateAfterMove = null)
    {
        if (playClipDelayed) { MNM_AudioController.Instance.PlayAudioClipDelayed(clip, duration); }
        else { MNM_AudioController.Instance.PlayAudioClip(clip); }
        MoveFlora(targetPos, targetRot, duration, useOffset, stateAfterMove);
    }

    public void MoveFloraAndPlayClip(Transform target, AudioClip clip, float duration = 2f, bool useOffset = false, bool playClipDelayed = false, FloraStates? stateAfterMove = null)
    {
        if (playClipDelayed) { MNM_AudioController.Instance.PlayAudioClipDelayed(clip, duration); }
        else { MNM_AudioController.Instance.PlayAudioClip(clip); }
        MoveFlora(target, duration, useOffset, stateAfterMove);
    }

    private void MoveMultiple(Transform[] targets, AudioClip[] clips, FloraStates?[] statesAfterMove, float duration = 2f, bool useOffset = false)
    {
        if((targets.Length != statesAfterMove.Length) || statesAfterMove.Length != clips.Length) { Debug.LogError("Not all array same length"); return; }
        StartCoroutine(MoveMultipleCoroutine(targets, clips, statesAfterMove, duration));
    }
    public void MoveMultiple(Transform[] targets, AudioClip[] clips, FloraStates[] states, float duration = 2f)
    {
        // Konvertierung nur hier
        if ((targets.Length != states.Length) || states.Length != clips.Length) { Debug.LogError("Not all array same length"); return; }
        SaveLastMultipleMovement(targets, clips, states);
        var nullableStates = Array.ConvertAll(states, s => (FloraStates?)s);
        MoveMultiple(targets, clips, nullableStates, duration:duration);
    }

    private IEnumerator MoveMultipleCoroutine(Transform[] targets, AudioClip[] clips, FloraStates?[] statesAfterMove, float duration = 2f, bool useOffset = false)
    {
        for(int i = 0; i < clips.Length; i++)
        {
            MoveFloraAndPlayClip(targets[i], clips[i], duration:duration, playClipDelayed:true, stateAfterMove: statesAfterMove[i]);
            yield return new WaitForSeconds(duration + clips[i].length);
        }
        finishedMultipleEvent.Invoke();
    }

    public void ReplayLastFloraAnimation()
    {
        MoveMultiple(lastTransforms, lastClips, lastStates);
    }

    #endregion

    private void SaveLastMultipleMovement(Transform[] targets, AudioClip[] clips, FloraStates[] states)
    {
        lastTransforms = targets;
        lastClips = clips;
        lastStates = states;
    }

    private void ChangedState(FloraStates newState)
    {
        ResetAllStates();
        switch(newState)
        {
            case FloraStates.Spinning:
                FloraSpinAnimation.Instance.TriggerSpin(repeatAnimation:true); break;
            case FloraStates.Pushing:
                FloraButtonPress.Instance.TriggerPress(repeatAnimation:true); break;
            case FloraStates.Waiting:
                break;
        }
    }

    private void ResetAllStates()
    {
        FloraButtonPress.Instance.StopRepeatAnimationPlaying();
        FloraSpinAnimation.Instance.StopRepeatAnimationPlaying();
    }

    private void InvokeFinishedMovingEvent()
    {
        finishedMovingEvent.Invoke();
    }

    public void SetOffsetVector(Vector3 offset)
    {
        offsetVector = offset;
    }

    private void SetIsMovingTrue()
    {
        isMoving = true;
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

    private void SetIsMovingFalse() { isMoving = false; }

    #region MoveCommands
    private void MoveFloraToMixNMatch()
    {
        MoveFloraAndPlayClip(mixNmatchTarget, MNM_AudioController.Instance.introText_clip, playClipDelayed:true, stateAfterMove:FloraStates.Spinning);
    }

    private void PlayIntro()
    {
        var tmp = MixNMatchController.Instance.data.GetIntroValues();
        MoveMultiple(tmp.Item1, tmp.Item2, tmp.Item3);
    }

    private void PlayPlanting()
    {
        var tmp = MixNMatchController.Instance.data.GetPlantValues();
        MoveMultiple(tmp.Item1, tmp.Item2, tmp.Item3);
    }

    private void PlayMoreInformation()
    {
        var tmp = MixNMatchController.Instance.data.GetMoreInformationValues();
        MoveMultiple(tmp.Item1, tmp.Item2, tmp.Item3);
    }

    private void PlayWeiter()
    {
        var tmp = MixNMatchController.Instance.data.GetWeiterValues();
        MoveMultiple(tmp.Item1, tmp.Item2, tmp.Item3);
    }

    private void MoveFloraToPlantBubble()
    {
        MoveFloraAndPlayClip(plantBubbleTargets[MixNMatchController.Instance.data.numberPhase], MNM_AudioController.Instance.afterPlantAudio_clip, playClipDelayed: true);
    }
    #endregion
}

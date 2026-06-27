using System;
using UnityEngine;
using UnityEngine.Events;

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
    [Header("Events")]
    public UnityEvent startedMovingEvent;
    public UnityEvent finishedMovingEvent;
    [Header("Settings")]
    [SerializeField] private Vector3 offsetVector;

    [Header("Debug")]
    [SerializeField] private GameObject debugTarget;
    [SerializeField] private AudioClip debugClip;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            MoveFloraAndPlayClip(debugTarget.transform, debugClip, playClipDelayed:true, stateAfterMove:FloraStates.Spinning);
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

    private void ChangedState(FloraStates newState)
    {
        ResetAllStates();
        switch(newState)
        {
            case FloraStates.Spinning:
                FloraSpinAnimation.Instance.TriggerSpin(repeatAnimation:true); break;
            case FloraStates.Pushing:
                FloraButtonPress.Instance.TriggerPress(repeatAnimation:true); break;
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

    private void SetFloraState(FloraStates newState)
    {
        FloraState = newState;
    }
}

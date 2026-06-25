using UnityEngine;
using UnityEngine.Events;

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

    public bool isMoving;
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
            MoveFloraAndPlayClip(debugTarget.transform, debugClip, playClipDelayed:true);
        }
    }

    public void MoveFlora(Vector3 targetPos, Vector3 targetRot, float duration = 2f, bool useOffset = false)
    {
        startedMovingEvent.Invoke();
        SmoothMover mover = floraMoveObject.AddComponent<SmoothMover>();
        if (useOffset) { targetPos = targetPos + offsetVector; }
        mover.Init(targetPos, targetRot, duration);
        mover.atFinalTransformEvent.AddListener(InvokeFinishedMovingEvent);

    }

    public void MoveFlora(Transform target, float duration = 2f, bool useOffset = false)
    {
        MoveFlora(target.position, target.rotation.eulerAngles, duration, useOffset);
    }

    public void MoveFloraAndPlayClip(Vector3 targetPos, Vector3 targetRot, AudioClip clip, float duration = 2f, bool useOffset = false, bool playClipDelayed = false)
    {
        if (playClipDelayed) { MNM_AudioController.Instance.PlayAudioClipDelayed(clip, duration); }
        else { MNM_AudioController.Instance.PlayAudioClip(clip); }
        MoveFlora(targetPos, targetRot, duration, useOffset);
    }

    public void MoveFloraAndPlayClip(Transform target, AudioClip clip, float duration = 2f, bool useOffset = false, bool playClipDelayed = false)
    {
        if (playClipDelayed) { MNM_AudioController.Instance.PlayAudioClipDelayed(clip, duration); }
        else { MNM_AudioController.Instance.PlayAudioClip(clip); }
        MoveFlora(target, duration, useOffset);
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

    private void SetIsMovingFalse() { isMoving = false; }
}

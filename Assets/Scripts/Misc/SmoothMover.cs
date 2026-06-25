using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SmoothMover : MonoBehaviour
{
    public Vector3 targetPosition;
    public Vector3 targetRotation;

    [SerializeField] float duration = 1f;
    [SerializeField] float curveStrength = 1f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion endRot;
    private float time;

    public UnityEvent atFinalTransformEvent = new UnityEvent();


    public void Init(Vector3 targetPosition_, Vector3 targetRotation_, float duration_, float curveStrength_ = 1f)
    {
        targetPosition = targetPosition_;
        targetRotation = targetRotation_;
        endRot = Quaternion.Euler(targetRotation_);
        duration = duration_;
        curveStrength = curveStrength_;
        atFinalTransformEvent.RemoveAllListeners(); 
        atFinalTransformEvent.AddListener(DestroyMyself);
        StartCoroutine(InvokeDelayed(duration));
    }

    public void Init(Transform targetTransform_, float duration_, float curveStrength_ = 1f)
    {
        targetPosition = targetTransform_.position;
        targetRotation = targetTransform_.rotation.eulerAngles;
        endRot = targetTransform_.rotation;
        duration = duration_;
        curveStrength = curveStrength_;
        atFinalTransformEvent.RemoveAllListeners(); 
        atFinalTransformEvent.AddListener(DestroyMyself);
        StartCoroutine(InvokeDelayed(duration));
    }

    void OnEnable()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        if(targetRotation != Vector3.zero )
        {
            endRot = Quaternion.Euler(targetRotation);
        }
        else
        {
            endRot = startRot;
        }
        

        time = 0f;
    }

    void Update()
    {
        if (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // this results in an S-shaped curve, with the curve strength param
            // determining the shape, higher values result in a slower start/end with faster middle
            if (t < 0.5f)
                t =  0.5f * Mathf.Pow(2f * t, curveStrength);
            else
                t = 1f - 0.5f * Mathf.Pow(2f * (1f - t), curveStrength);

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
        }
    }

    IEnumerator InvokeDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        atFinalTransformEvent?.Invoke();
    }

    public void SetPositionAndRotation(Vector3 position, Vector3 rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
        endRot = Quaternion.Euler(targetRotation);
    }

    public void SetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    public float GetDuration() {  return duration; }
    public void SetDuration(float duration_) { duration = duration_; }
    public void SetCurve(float curve) { curveStrength = curve; }

    private void DestroyMyself()
    {
        Destroy(this);
    }
}

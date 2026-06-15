using Oculus.Interaction;
using System.Collections;
using System.ComponentModel;
using System.Net.NetworkInformation;
using UnityEngine;


public class PlantInBubbleVisuals : MonoBehaviour
{
    private enum BubbleState
    {
        Waiting,
        IsInside,
        MoveToFinal,
        IsPlanted
    }

    [Header("Settings")]
    [SerializeField] private BubbleState bubbleState;
    [SerializeField] private Color waitingColor;
    [SerializeField] private Color insideColor;
    [SerializeField] private Color normalColor;
    [SerializeField][Range(0f, 2f)] private float waitingPower;
    [SerializeField][Range(0f, 2f)] private float normalPower;
    private float waitingIntervall = 2f;
    private float intervallSpeed = 2f;
    private float floatToFinalPositionTime = 2f;

    [Header("References")]
    [SerializeField] private GameObject flowerObject;
    [SerializeField] private Transform targetTransformFlower;
    [SerializeField] private Renderer bubbleRenderer;

    [Header("Coroutines")]
    private Coroutine currentRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bubbleRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        ActivateStateWaiting(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ActivateStateIsInside();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            ActivateStateMoveToFinal();
        }
    }

    public void ActivateStateWaiting(bool isStartup = false)
    {
        bubbleState = BubbleState.Waiting;
        currentRoutine = StartCoroutine(WaitingStateRoutine(isStartup));
    }

    private IEnumerator WaitingStateRoutine(bool isStartUp)
    {
        yield return null;
        if(isStartUp)
        {
            bubbleRenderer.material.SetColor("_Color", waitingColor);
        }
        else
        {
            StartCoroutine(LerpToColor(2f, waitingColor));
        }
        while (true)
        {
            float value = (Mathf.Sin(Time.time * intervallSpeed) + 1f) * 0.5f;
            value = Mathf.Lerp(waitingPower, normalPower, value);
            bubbleRenderer.material.SetFloat("_Power", value);
            yield return null;
        }
    }

    public void ActivateStateIsInside()
    {
        bubbleState = BubbleState.IsInside;
        if(currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(IsInsideRoutine());
    }

    private IEnumerator IsInsideRoutine()
    {
        yield return null;
        //bubbleRenderer.material.SetColor("_Color", waitingColor);
        StartCoroutine(LerpToColor(2f, insideColor));
        while (true)
        {
            float value = (Mathf.Sin(Time.time * intervallSpeed) + 1f) * 0.5f;
            value = Mathf.Lerp(waitingPower, normalPower, value);
            bubbleRenderer.material.SetFloat("_Power", value);
            yield return null;
        }
    }

    public float ActivateStateMoveToFinal()
    {
        bubbleState = BubbleState.MoveToFinal;
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(MoveToFinalRoutine(floatToFinalPositionTime));
        return floatToFinalPositionTime;
    }

    private IEnumerator MoveToFinalRoutine(float duration)
    {
        Color startColor = bubbleRenderer.material.GetColor("_Color");
        float startPower = bubbleRenderer.material.GetFloat("_Power");
        float startTime = Time.time;
        float endTime = startTime + duration;

        SmoothMover mover = flowerObject.AddComponent<SmoothMover>();
        mover.Init(targetTransformFlower, duration);
        mover.atFinalTransformEvent.AddListener(ActivateStateIsPlanted);
        while (true)
        {
            float timeProgress = Mathf.InverseLerp(startTime, endTime, Time.time);
            Color newColor = Color.Lerp(startColor, normalColor, timeProgress);
            bubbleRenderer.material.SetColor("_Color", newColor);
            float newPower = Mathf.Lerp(startPower, normalPower, timeProgress);
            bubbleRenderer.material.SetFloat("_Power", newPower);
            yield return null;
        }
    }

    public void ActivateStateIsPlanted()
    {
        bubbleState = BubbleState.IsPlanted;
        StopCoroutine(currentRoutine);
    }

    private IEnumerator LerpToColor(float duration, Color endColor)
    {
        Color startColor = bubbleRenderer.material.GetColor("_Color");
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float timeProgress = Mathf.InverseLerp(startTime, endTime, Time.time);
            Color newColor = Color.Lerp(startColor, endColor, timeProgress);
            bubbleRenderer.material.SetColor("_Color", newColor);
            yield return null;
        }
    }

    public void SetUpVisuals(GameObject flowerObject, Transform targetTransform)
    {
        this.flowerObject = flowerObject;
        this.targetTransformFlower = targetTransform;
    }
}

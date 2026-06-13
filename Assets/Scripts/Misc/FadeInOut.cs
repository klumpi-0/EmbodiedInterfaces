using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FadeInOut : MonoBehaviour
{
    [InspectorButton("onFadeIn")]
    public bool fadeIn;

    [InspectorButton("onFadeOut")]
    public bool fadeOut;

    [InspectorButton("onCalibrate")]
    public bool calibrate;

    public Vector3 startPos;
    Vector3 targetPos;

    public Vector3 startScale;
    Vector3 targetScale;

    public Vector3 animationVector = new Vector3(0, .3f, 0);
    public float animLerpSpeed = .9f;

    public UnityEvent fadeInEvent;
    public UnityEvent fadeOutEvent;

    public bool fadeOutOnLaunch = true;


    void Start()
    {
        //startPos = transform.position;
        //startScale = transform.localScale;
        if (fadeOutOnLaunch) onFadeOut();
    }

    void Reset()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    Vector3 prevPos;
    Vector3 prevScale;

    //public bool autoCalibrate = true;
    void Update()
    {
        /*
        if (!autoCalibrate) return;
        if (animating) return;
        if (transform.position != prevPos)
        {
            startPos = transform.position;
            targetPos = transform.position;
        }
        //if (transform.localScale != prevScale) startScale = transform.localScale;
        prevPos = transform.position;
        //prevScale = transform.localScale;
        */
    }

    public void onFadeOut()
    {
        isFadedIn = false;
        StopAllCoroutines();
        if(this.isActiveAndEnabled)
        {
            StartCoroutine(lerpTowards(startPos - animationVector, new Vector3(0, 0, 0)));
        }
        fadeOutEvent.Invoke();
    }

    public void onFadeIn()
    {
        isFadedIn = true;
        StopAllCoroutines();
        if (this.isActiveAndEnabled)
        {
            StartCoroutine(lerpTowards(startPos, startScale));
        }
        fadeInEvent.Invoke();
    }

    public void onCalibrate ()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        onFadeIn();
    }

    public float animationDuration = 1f;
    bool animating = false;

    public bool isFadedIn = false;
    IEnumerator lerpTowards(Vector3 tPos, Vector3 tScale)
    {
        animating = true;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            Vector3 lerpedScale = Vector3.Lerp(tScale, transform.localScale, animLerpSpeed);
            Vector3 lerpedPos = Vector3.Lerp(tPos, transform.position, animLerpSpeed);

            transform.localScale = lerpedScale;
            transform.position = lerpedPos;
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }

        // Make sure we got there
        transform.position = tPos;
        transform.localScale = tScale;
        animating = false;
        yield return null;
    }

    public void TriggerFadeInOrOut()
    {
        if (isFadedIn)
        {
            onFadeOut();
        }
        else
        {
            onFadeIn();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class MoveCameraToNextPosition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject cameraRig;

    [Header("Target Position")]
    [SerializeField] private List<Transform> targetPositions;
    [SerializeField] private int count = 0;

    private void Start()
    {
        ScreenFade.Instance.OnBlackScreenEvent.AddListener(SkipToNextPosition);
    }

    public void SkipToNextPosition()
    {
        if(count + 1 > targetPositions.Count) { return; }
        cameraRig.transform.position = targetPositions[count + 1].position;
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProcessRoations : MonoBehaviour
{
    public static ProcessRoations Instance;

    [Header("Nearest Sides")]
    [SerializeField] private int frontFacingSideLow;
    [SerializeField] private int frontFacingSideMiddle;
    [SerializeField] private int frontFacingSideHigh;
    [SerializeField] private int[] frontFacingSides = new int[3];

    [Header("Settings")]
    [Tooltip("Based on this number, the calcualtion in the script will calculate stuff dynamicly")]
    [SerializeField] private int numberSides = 4;
    [SerializeField] private int[] targetRotation;
    [SerializeField] private int[] targetArrowRotation;

    [Header("Events")]
    public UnityEvent<int> newFrontSideLowEvent;
    public UnityEvent<int> newFrontSideMiddleEvent;
    public UnityEvent<int> newFrontSideHighEvent;
    public UnityEvent<int> leftFrontFacingSideEvent;

    [Header("References")]
    [SerializeField] private GetVirtuallRotation rotation;

    [Header("RotationData")]
    [Tooltip("Angle which must be undergone to call the new frontSide Events")]
    [SerializeField] private float angleForFrontSideEvents = 20f;
    private float rotLow, rotMiddle, rotHigh;
    private float lastRotLow, lastRotMiddle, lastRotHigh;

    // Internal values
    private float[] rotationSteps;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        rotationSteps = CalculateRoationSteps();
    }

    private void Start()
    {
        SetRotationDigitalTwin.Instance.buttonPressedEvent.AddListener(CheckForTargetRotation);
    }

    // Update is called once per frame
    void Update()
    {
        (rotLow, rotMiddle, rotHigh) = GetVirtuallRotation.Instance.GetVirtuallRotations();
        UpdateFrontFacingSides();
        //CheckForTargetRotation();
        CheckIfEventsNeedInvoked();
        // Get last rotations
        (lastRotLow, lastRotMiddle, lastRotHigh) = GetVirtuallRotation.Instance.GetVirtuallRotations();
    }

    private void UpdateFrontFacingSides()
    {
        frontFacingSides[0] = GetNearestSide(rotation.rotationLow);
        frontFacingSides[1] = GetNearestSide(rotation.rotationMiddle);
        frontFacingSides[2] = GetNearestSide(rotation.rotationHigh);
    }

    private float[] CalculateRoationSteps()
    {
        List<float> rotList = new List<float>();
        for (int i = 0; i < numberSides; i++)
        {
            rotList.Add(i * 360 / numberSides);
        }
        Debug.Log($"Rotation Steps: {rotList.Count}");
        return rotList.ToArray();
    }

    // TODO: maybe change angle, that it is a little bit smaller 
    private int GetNearestSide(float currentRot)
    {
        int nearestSide = 0;
        float smallestAngle = float.MaxValue;
        for(int i = 0; i < rotationSteps.Length; i++)
        {
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentRot, rotationSteps[i]));
            if(angleDifference < smallestAngle)
            {
                nearestSide = i;
                smallestAngle = angleDifference;
            }
        }
        return nearestSide;
    }

    

    private void CheckForTargetRotation()
    {
        if(frontFacingSides.SequenceEqual(targetRotation))
        {
            MixNMatchController.Instance.InitFoundMatchEvent();
        }
        if (frontFacingSides.SequenceEqual(targetArrowRotation))
        {
            MixNMatchController.Instance.InitForwardArrowsEvent();
        }
    }

    private void CheckIfEventsNeedInvoked()
    {
        if (EnteredQuarterRotationArea(lastRotLow, rotLow, angleForFrontSideEvents, out int frontSideLow))
        {
            newFrontSideLowEvent.Invoke(frontSideLow);
        }
        if (EnteredQuarterRotationArea(lastRotMiddle, rotMiddle, angleForFrontSideEvents, out int frontSideMiddle))
        {
            newFrontSideMiddleEvent.Invoke(frontSideMiddle);
        }
        if (EnteredQuarterRotationArea(lastRotHigh, rotHigh, angleForFrontSideEvents, out int frontSideHigh))
        {
            newFrontSideHighEvent.Invoke(frontSideHigh);
        }
        if(LeftQuarterRotationArea(lastRotLow, rotLow, angleForFrontSideEvents)) { leftFrontFacingSideEvent.Invoke(0); }
        if(LeftQuarterRotationArea(lastRotMiddle, rotMiddle, angleForFrontSideEvents)) { leftFrontFacingSideEvent.Invoke(1); }
        if(LeftQuarterRotationArea(lastRotHigh, rotHigh, angleForFrontSideEvents)) { leftFrontFacingSideEvent.Invoke(2); }
    }

    public static bool EnteredQuarterRotationArea(
        float previousYRotation,
        float currentYRotation,
        float tolerance,
        out int nearestPosition)
    {
        nearestPosition = GetNearestQuarterPosition(currentYRotation);

        bool wasInside = IsNearQuarterRotation(previousYRotation, tolerance, out _);
        bool isInside = IsNearQuarterRotation(currentYRotation, tolerance, out _);

        return !wasInside && isInside;
    }

    public static bool IsNearQuarterRotation(
        float yRotation,
        float tolerance,
        out int nearestPosition)
    {
        yRotation = (yRotation % 360f + 360f) % 360f;

        nearestPosition = Mathf.RoundToInt(yRotation / 90f) % 4;

        float targetAngle = nearestPosition * 90f;
        float delta = Mathf.Abs(Mathf.DeltaAngle(yRotation, targetAngle));

        return delta <= tolerance;
    }

    public static bool LeftQuarterRotationArea(
    float previousYRotation,
    float currentYRotation,
    float tolerance)
    {
        bool wasInside = IsNearQuarterRotation(previousYRotation, tolerance, out _);
        bool isInside = IsNearQuarterRotation(currentYRotation, tolerance, out _);

        return wasInside && !isInside;
    }


    public static int GetNearestQuarterPosition(float yRotation)
    {
        yRotation = (yRotation % 360f + 360f) % 360f;
        return Mathf.RoundToInt(yRotation / 90f) % 4;
    }



    /// <summary>
    /// Is used to return the front facing sides of the mixNmatch.
    /// </summary>
    /// <returns>Returns an array with the  number of sides looking forward [low, middle, high] </returns>
    public int[] GetFrontFacingSidesArray()
    {
        return frontFacingSides;
    }
    public void SetTargetRotation(int[] newTargetRotation)
    {
        targetRotation = newTargetRotation;
    }

    public void SetTargetRotationArrows(int[] newTargetRotation)
    {
        targetArrowRotation = newTargetRotation;
    }
}

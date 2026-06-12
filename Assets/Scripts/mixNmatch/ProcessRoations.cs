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

    [Header("Events")]
    UnityEvent<int> newFrontSideLowEvent;
    UnityEvent<int> newFrontSideMiddleEvent;
    UnityEvent<int> newFrontSideHighEvent;

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

    // Update is called once per frame
    void Update()
    {
        (rotLow, rotMiddle, rotHigh) = GetVirtuallRotation.Instance.GetVirtuallRotations();
        UpdateFrontFacingSides();
        CheckForTargetRotation();
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
            if(Mathf.Abs(currentRot - rotationSteps[i]) < smallestAngle)
            {
                nearestSide = i;
                smallestAngle = Mathf.Abs(currentRot - rotationSteps[i]);
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
    }

    private void CheckIfEventsNeedInvoked()
    {

    }

    private bool NeedToFireEvent(float currentValue, float lastValue)
    {
        return false;
    }

    private bool UnderTargetAngle(float targetAngle, float currentAngle)
    {
        return Mathf.Abs(targetAngle - currentAngle) < angleForFrontSideEvents;
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
}

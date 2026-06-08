using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ProcessRoations : MonoBehaviour
{
    public static ProcessRoations Instance;

    [Header("Nearest Sides")]
    [SerializeField] private int frontFacingSideLow;
    [SerializeField] private int frontFacingSideMiddle;
    [SerializeField] private int frontFacingSideHigh;

    [Header("Settings")]
    [Tooltip("Based on this number, the calcualtion in the script will calculate stuff dynamicly")]
    [SerializeField] private int numberSides = 4;

    [Header("References")]
    [SerializeField] private GetVirtuallRotation rotation;

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
        frontFacingSideLow = GetNearestSide(rotation.rotationLow);
        frontFacingSideMiddle = GetNearestSide(rotation.rotationMiddle);
        frontFacingSideHigh = GetNearestSide(rotation.rotationHigh);
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

    private int GetNearestSide(float currentRot)
    {
        int nearestSide = 0;
        float smallestAngle = float.MaxValue;
        for(int i = 0; i < rotationSteps.Length; i++)
        {
            Debug.Log("Found " + rotationSteps[i]);
            if(Mathf.Abs(currentRot - rotationSteps[i]) < smallestAngle)
            {
                nearestSide = i;
                smallestAngle = Mathf.Abs(currentRot - rotationSteps[i]);
                Debug.Log($"Found nearest angle({smallestAngle}) with side {nearestSide}");
            }
        }
        return nearestSide;
    }

    /// <summary>
    /// Is used to return the front facing sides of the mixNmatch.
    /// </summary>
    /// <returns>Returns an array with the  number of sides looking forward [low, middle, high] </returns>
    public int[] GetFrontFacingSidesArray()
    {
        return new int[] { frontFacingSideLow, frontFacingSideMiddle, frontFacingSideHigh };
    }

    public (int, int, int) GetFrontFacingSides()
    {
        return (frontFacingSideLow, frontFacingSideMiddle, frontFacingSideHigh);
    }
}

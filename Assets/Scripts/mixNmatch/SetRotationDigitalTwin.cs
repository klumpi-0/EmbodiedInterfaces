using UnityEngine;

public class SetRotationDigitalTwin : MonoBehaviour
{
    [SerializeField] private bool debugRotation;
    [Header("Rotations")]
    [SerializeField] private float lowRotation;
    [SerializeField] private float middleRotation;
    [SerializeField] private float highRotation;

    [Header("References")]
    [SerializeField] private GameObject lowCube;
    [SerializeField] private GameObject middleCube;
    [SerializeField] private GameObject highCube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (debugRotation)
        {
            SetRoationForCube(lowCube, lowRotation);
            SetRoationForCube(middleCube, middleRotation);
            SetRoationForCube(highCube, highRotation);
        }
    }

    public void SetRotation(string rotationInput)
    {

    }

    private void SetRoationForCube(GameObject cube, float newRotation)
    {
        var angles = cube.transform.eulerAngles;
        angles.y = newRotation;
        cube.transform.eulerAngles = angles;
    }
}

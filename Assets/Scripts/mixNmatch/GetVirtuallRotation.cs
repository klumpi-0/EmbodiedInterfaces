using UnityEngine;

public class GetVirtuallRotation : MonoBehaviour
{
    public static GetVirtuallRotation Instance;

    [Header("Rotations")]
    public float rotationLow { private set; get; }
    public float rotationMiddle { private set; get; }
    public float rotationHigh { private set; get; }

    [Header("References")]
    [SerializeField] private GameObject cubeLow;
    [SerializeField] private GameObject cubeMiddel;
    [SerializeField] private GameObject cubeHigh;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotationLow = GetAndSetRotation(cubeLow);
        rotationMiddle = GetAndSetRotation(cubeMiddel);
        rotationHigh = GetAndSetRotation(cubeHigh);
    }

    private float GetAndSetRotation(GameObject cube)
    {
        return Mathf.Abs(cube.transform.localEulerAngles.y % 360);
    }

    public (float, float, float) GetVirtuallRotations()
    {
        return (rotationLow, rotationMiddle, rotationHigh);
    }
}

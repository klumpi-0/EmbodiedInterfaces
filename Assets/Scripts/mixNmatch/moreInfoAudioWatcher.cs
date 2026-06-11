using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Is a script which specifically is used to watch the rotation
/// </summary>
public class moreInfoAudioWatcher : MonoBehaviour
{
    private float lastRotCubeLow;
    private float lastRotCubeMiddle;
    private float lastRotCubeHigh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        SetNewLastRotations();   
    }

    private void SetNewLastRotations()
    {
        lastRotCubeLow = GetVirtuallRotation.Instance.rotationLow;
        lastRotCubeMiddle = GetVirtuallRotation.Instance.rotationMiddle;
        lastRotCubeHigh = GetVirtuallRotation.Instance.rotationHigh;
    }

    public void CheckIfCubeRotated(int numberCube)
    {
        
    }
}

using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform rotateTransform;
    [SerializeField] private bool flipFrontFacingSide;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(rotateTransform == null)
        {
            rotateTransform = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotateAround();
    }

    private void RotateAround()
    {
        rotateTransform.LookAt(Camera.main.transform);
        if(flipFrontFacingSide )
        {
            rotateTransform.Rotate(0, 180f, 0);
        }
    }
}

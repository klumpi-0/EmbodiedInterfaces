using Oculus.Interaction.Body.Input;
using UnityEngine;

public class RegestrateMixNMatch : MonoBehaviour
{
    [SerializeField] private GameObject rightController;
    [SerializeField] private GameObject leftController;

    [SerializeField] private Vector3 offsetVector;

    [SerializeField] private GameObject moveObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.Two) && OVRInput.Get(OVRInput.Button.Four))
        {
            MoveMNM();
            RotateMixNMatch();
        }
    }

    void MoveMNM()
    {
        Debug.Log("Moved");
        if (rightController == null || leftController == null || moveObject == null)
            return;

        // Mitte zwischen beiden Controllern berechnen
        Vector3 middlePoint = (rightController.transform.position + leftController.transform.position) * 0.5f;

        // Offset anwenden
        moveObject.transform.position = middlePoint + offsetVector;
    }

    private void RotateMixNMatch()
    {
        if (rightController == null || leftController == null || moveObject == null)
            return;

        // Vektor von links nach rechts
        Vector3 leftToRight = rightController.transform.position - leftController.transform.position;

        // Forward = senkrecht dazu (zeigt "nach hinten" aus Nutzersicht)
        Vector3 newForward = -Vector3.Cross(leftToRight, Vector3.up).normalized;

        moveObject.transform.rotation = Quaternion.LookRotation(newForward, Vector3.up);
    }

    private void CalculateNewTransform()
    {

    }
}

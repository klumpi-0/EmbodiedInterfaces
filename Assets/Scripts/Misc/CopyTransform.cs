using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [Header("Objekt, dessen Bewegung kopiert wird")]
    public Transform sourceObject;
    [Header("Objekt, auf den die Transform kopiert wird")]
    public Transform targetObject;
    [Header("Position kopieren")]
    public bool copyPosition = true;

    [Header("Rotation kopieren")]
    public bool copyRotation = true;

    void Update()
    {
        if (sourceObject == null)
            return;

        if (copyPosition)
        {
            targetObject.position = sourceObject.position;
        }

        if (copyRotation)
        {
            targetObject.rotation = sourceObject.rotation;
        }
    }
}
using Oculus.Interaction;
using UnityEngine;

public class PlantInBubbleLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flowerObject;
    [SerializeField] private Grabbable flowerGrabbable;
    [SerializeField] private Transform targetTransformFlower;
    [SerializeField] private bool isInCollider;
    [SerializeField] private PlantInBubbleVisuals visuals;

    private void OnEnable()
    {
        if (flowerGrabbable == null)
        {
            flowerGrabbable = flowerObject.GetComponent<Grabbable>();
        }
        flowerGrabbable.WhenPointerEventRaised += HandlePointerEvent;
    }
    private void OnDisable()
    {
        flowerGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == flowerObject)
        {
            isInCollider = true;
            visuals.ActivateStateIsInside();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == flowerObject)
        {
            isInCollider = false;
            visuals.ActivateStateWaiting();
        }
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Unselect)
        {
            if (isInCollider)
            {
                visuals.ActivateStateMoveToFinal();
            }
        }
    }

}

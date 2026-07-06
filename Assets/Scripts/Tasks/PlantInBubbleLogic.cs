using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PlantInBubbleLogic : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flowerObject;
    [SerializeField] private Grabbable flowerGrabbable;
    [SerializeField] private Transform targetTransformFlower;
    [SerializeField] private bool isInCollider;
    [SerializeField] private PlantInBubbleVisuals visuals;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            DebugMoveInPlant();
        }
    }

    public void SetupPlantBubble(GameObject flower, Grabbable grabbable)
    {
        flowerObject = flower;
        flowerGrabbable = grabbable;
        visuals.SetUpVisuals(flowerObject, targetTransformFlower);
        OnEnable();
    }

    private void OnEnable()
    {
        if(flowerObject == null)
        {
            return;
        }
        if (flowerGrabbable == null)
        {
            flowerGrabbable = flowerObject.GetComponent<Grabbable>();
        }
        flowerGrabbable.WhenPointerEventRaised += HandlePointerEvent;
        visuals.ActivateStateWaiting();
    }
    private void OnDisable()
    {
        if (flowerGrabbable == null) { return; }
        flowerGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == flowerObject)
        {
            isInCollider = true;
            visuals.ActivateStateIsInside();
            if (flowerGrabbable == null)
            {
                flowerGrabbable = flowerObject.GetComponent<Grabbable>();
            }
            flowerGrabbable.WhenPointerEventRaised += HandlePointerEvent;

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
                var delayTime = visuals.ActivateStateMoveToFinal();
                MixNMatchController.Instance.InitPlantedEvent(delayTime);
                HinderGrabbableMoving();
            }
        }
    }

    private void DebugMoveInPlant()
    {
        isInCollider = true;
        if (isInCollider)
        {
            var delayTime = visuals.ActivateStateMoveToFinal();
            MixNMatchController.Instance.InitPlantedEvent(delayTime);

        }
    }

    private void HinderGrabbableMoving()
    {
        flowerGrabbable.enabled = false;
        var freeTransform = flowerGrabbable.gameObject.GetComponent<GrabFreeTransformer>();
        freeTransform.enabled = false;
        flowerGrabbable.gameObject.GetComponentInChildren<HandGrabInteractable>().enabled = false;
    }

}

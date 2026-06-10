using Oculus.Interaction;
using UnityEngine;

public class Task01 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flowerObject;
    [SerializeField] private Grabbable flowerGrabbable;
    [SerializeField] private Transform targetTransformFlower;
    [SerializeField] private bool isInCollider;

    private void Awake()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == flowerObject)
        {
            isInCollider = true;
            FollowTaskController.Instance.finishedTaskEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == flowerObject)
        {
            isInCollider = false;
        }
    }

    private void OnEnable()
    {
        if (flowerGrabbable == null)
        {
            flowerGrabbable = flowerObject.GetComponent<Grabbable>();
        }
        FollowTaskController.Instance.finishedTaskEvent.AddListener(MoveFowerSmootToEnd);
        flowerGrabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDisable()
    {
        flowerGrabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Unselect)
        {
            if (isInCollider)
            {
                FollowTaskController.Instance.finishedTaskEvent?.Invoke();
            }
        }
    }

    private void MoveFowerSmootToEnd()
    {
        SmoothMover mover = flowerObject.AddComponent<SmoothMover>();
        mover.Init(targetTransformFlower, 2); 
    }

}

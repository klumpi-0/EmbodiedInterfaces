using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class InteractableZoneHandler : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private float zoneRadius = 0.15f;
    [SerializeField] private bool showGizmo = true;

    [Header("Events")]
    public UnityEvent onEnterZone;
    public UnityEvent onExitZone;
    public UnityEvent onPinch;
    public UnityEvent onRelease;

    private HandGrabInteractable _interactable;
    private bool _isInZone = false;
    private bool _isSelected = false;

    private void Awake()
    {
        _interactable = GetComponent<HandGrabInteractable>();
        if (_interactable == null)
        {
            Debug.LogError("[InteractableZoneHandler] Kein HandGrabInteractable gefunden!", this);
            return;
        }
    }

    private void OnEnable()
    {
        if (_interactable == null) return;
        _interactable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void OnDisable()
    {
        if (_interactable == null) return;
        _interactable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Hover:
                if (!_isInZone)
                {
                    _isInZone = true;
                    onEnterZone?.Invoke();
                }
                break;

            case PointerEventType.Unhover:
                if (_isInZone)
                {
                    _isInZone = false;
                    onExitZone?.Invoke();
                }
                break;

            case PointerEventType.Select:
                _isSelected = true;
                onPinch?.Invoke();
                break;

            case PointerEventType.Unselect:
                _isSelected = false;
                onRelease?.Invoke();
                break;

            case PointerEventType.Cancel:
                if (_isSelected) { _isSelected = false; onRelease?.Invoke(); }
                if (_isInZone) { _isInZone = false; onExitZone?.Invoke(); }
                break;
        }
    }

    private void OnValidate()
    {
        var col = GetComponent<SphereCollider>();
        if (col != null) col.radius = zoneRadius;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = _isInZone ? Color.green : new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}
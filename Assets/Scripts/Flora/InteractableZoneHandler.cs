using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;
using Oculus.Interaction.Input;

namespace Oculus.Interaction
{
    public class InteractableZoneHandler : PointableElement
    {
        [Header("Zone Settings")]
        [SerializeField] private float zoneRadius = 0.15f;
        [SerializeField] private bool showGizmo = true;

        [Header("Events")]
        public UnityEvent onEnterZone;
        public UnityEvent onExitZone;
        public UnityEvent onPinch;
        public UnityEvent onRelease;

        private bool _isInZone = false;
        private bool _isSelected = false;

        public override void ProcessPointerEvent(PointerEvent evt)
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
                    if (_isSelected)
                    {
                        _isSelected = false;
                        onRelease?.Invoke();
                    }
                    if (_isInZone)
                    {
                        _isInZone = false;
                        onExitZone?.Invoke();
                    }
                    break;
            }

            base.ProcessPointerEvent(evt);
        }

        private void Awake()
        {
            GameObject handRefObject = GameObject.FindWithTag("HandRef");
            if (handRefObject == null) return;

            var handRefsReference = handRefObject.GetComponents<HandRef>();
            var handRefs = GetComponents<HandRef>();

            for (int i = 0; i < handRefs.Length && i < handRefsReference.Length; i++)
            {
                if (handRefs[i].Hand == null)
                    handRefs[i].InjectHand(handRefsReference[i].Hand);
            }
        }

        // Zonengröße als Sphere-Collider synchron halten
        private void OnValidate()
        {
            var col = GetComponent<SphereCollider>();
            if (col != null)
                col.radius = zoneRadius;
        }

        // Zone im Editor sichtbar machen
        private void OnDrawGizmosSelected()
        {
            if (!showGizmo) return;
            Gizmos.color = _isInZone ? Color.green : new Color(0f, 1f, 1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, zoneRadius);
        }
    }
}
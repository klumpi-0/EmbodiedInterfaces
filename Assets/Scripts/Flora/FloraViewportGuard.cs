using UnityEngine;

public class FloraViewportGuard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera xrCamera;
    [SerializeField] private GameObject floraMoveObject;

    [Header("Viewport Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float visibilityThreshold = 0.8f;
    [SerializeField] private float edgePadding = 0.15f;
    [SerializeField] private float cornerDepth = 1.5f;

    [Header("Snap Animation")]
    [SerializeField] private float snapDuration = 1.2f;
    [SerializeField] private float snapCurveStrength = 2f;
    [SerializeField] private float returnDuration = 2f;
    [SerializeField] private float returnCurveStrength = 2f;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool isVisible;
    [SerializeField] private GuardState currentState;

    private enum GuardState { Idle, Snapping, Snapped, Returning }
    private GuardState _state = GuardState.Idle;

    private Vector3 _snappedWorldPos;
    private Vector3 _trueTargetPosition;
    private float _animationTimer = 0f;
    private float _animationDuration = 0f;

    private Vector3[] corners;
    [SerializeField] GameObject[] cubes;

    private void Start()
    {
        if (xrCamera == null)
            xrCamera = Camera.main;

        if (floraMoveObject == null && FloraController.Instance != null)
            floraMoveObject = FloraController.Instance.floraMoveObject;

        if (floraMoveObject != null)
            _trueTargetPosition = floraMoveObject.transform.position;
    }

    private void Update()
    {
        if (floraMoveObject == null) return;
        if(FloraController.Instance.GetIsMoving()) { return; }
        // Timer selbst runterzählen statt auf Callback warten
        if (_state == GuardState.Snapping || _state == GuardState.Returning)
        {
            _animationTimer += Time.deltaTime;
            if (_animationTimer >= _animationDuration)
            {
                _animationTimer = 0f;
                if (_state == GuardState.Snapping)
                {
                    _snappedWorldPos = floraMoveObject.transform.position;
                    _state = GuardState.Snapped;
                }
                else if (_state == GuardState.Returning)
                {
                    _state = GuardState.Idle;
                }
            }
            currentState = _state;
            return;
        }

        // Echte Zielposition nur im Idle mitschreiben
        if (_state == GuardState.Idle)
            _trueTargetPosition = floraMoveObject.transform.position;

        Vector3 trueViewport = xrCamera.WorldToViewportPoint(_trueTargetPosition);
        bool trueIsOutOfView = IsOutOfView(trueViewport);
        isVisible = !trueIsOutOfView;
        currentState = _state;

        switch (_state)
        {
            case GuardState.Idle:
                if (trueIsOutOfView)
                    SnapToCorner();
                break;

            case GuardState.Snapped:
                if (!trueIsOutOfView)
                    ReturnFromCorner();
                else
                    UpdateCornerPosition();
                break;
        }
        //PlaceCubesAtConres();
    }

    private bool IsOutOfView(Vector3 viewportPos)
    {
        if (viewportPos.z < 0) return true;
        float margin = 1f - visibilityThreshold;
        return viewportPos.x < -margin || viewportPos.x > 1f + margin ||
               viewportPos.y < -margin || viewportPos.y > 1f + margin;
    }

    private void SnapToCorner()
    {
        _state = GuardState.Snapping;
        _animationTimer = 0f;
        _animationDuration = snapDuration;

        Vector3 cornerWorldPos = GetNearestCornerWorldPos();
        SmoothMover mover = floraMoveObject.AddComponent<SmoothMover>();
        mover.Init(cornerWorldPos, floraMoveObject.transform.rotation.eulerAngles, snapDuration, snapCurveStrength);
    }

    private void ReturnFromCorner()
    {
        _state = GuardState.Returning;
        _animationTimer = 0f;
        _animationDuration = returnDuration;

        SmoothMover mover = floraMoveObject.AddComponent<SmoothMover>();
        mover.Init(_trueTargetPosition, floraMoveObject.transform.rotation.eulerAngles, returnDuration, returnCurveStrength);
    }

    private void UpdateCornerPosition()
    {
        Vector3 newCorner = GetNearestCornerWorldPos();
        if (Vector3.Distance(newCorner, _snappedWorldPos) > 0.1f)
        {
            _state = GuardState.Snapping;
            _animationTimer = 0f;
            _animationDuration = snapDuration * 0.5f;
            _snappedWorldPos = newCorner;

            SmoothMover mover = floraMoveObject.AddComponent<SmoothMover>();
            mover.Init(newCorner, floraMoveObject.transform.rotation.eulerAngles, snapDuration * 0.5f, snapCurveStrength);
        }
    }

    private Vector3 GetNearestCornerWorldPos()
    {
        Vector3 realViewport = xrCamera.WorldToViewportPoint(_trueTargetPosition);

        float cornerX = realViewport.x < 0.5f ? edgePadding : 1f - edgePadding;
        float cornerY = realViewport.y < 0.5f ? edgePadding : 1f - edgePadding;

        Vector3 viewportCorner = new Vector3(cornerX, cornerY, cornerDepth);
        if (realViewport.z < 0)
            viewportCorner.x = 1f - cornerX;

        return xrCamera.ViewportToWorldPoint(viewportCorner);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || xrCamera == null) return;

        corners = new Vector3[]
        {
            xrCamera.ViewportToWorldPoint(new Vector3(edgePadding,      edgePadding,      cornerDepth)),
            xrCamera.ViewportToWorldPoint(new Vector3(1f - edgePadding, edgePadding,      cornerDepth)),
            xrCamera.ViewportToWorldPoint(new Vector3(1f - edgePadding, 1f - edgePadding, cornerDepth)),
            xrCamera.ViewportToWorldPoint(new Vector3(edgePadding,      1f - edgePadding, cornerDepth))
        };

        Gizmos.color = Color.cyan;
        for (int i = 0; i < 4; i++)
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);

        Gizmos.color = Color.yellow;
        if (floraMoveObject != null)
            Gizmos.DrawSphere(GetNearestCornerWorldPos(), 0.05f);
    }

    private void PlaceCubesAtConres()
    {
        for(int i = 0; i < corners.Length; i++)
        {
            cubes[i].transform.position = corners[i];
        }
    }
}
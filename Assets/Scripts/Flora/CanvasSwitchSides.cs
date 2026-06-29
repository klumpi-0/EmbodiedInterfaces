using UnityEngine;

/// <summary>
/// Script decides if Flora is left or right in Viewport. Based on that the canvas gets moved
/// </summary>
public class CanvasSwitchSides : MonoBehaviour
{
    [SerializeField] private Transform floraTransform;
    [SerializeField] private Transform referenceTransform;
    [SerializeField] private Transform canvasTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SwitchCanvasToSide(FloraIsOnRightSide());
    }

    private bool FloraIsOnRightSide()
    {
        Vector3 dirToTarget = (floraTransform.position - referenceTransform.position).normalized;
        float dot = Vector3.Dot(referenceTransform.right, dirToTarget);
        if (dot > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SwitchCanvasToSide(bool isRightSide)
    {
        canvasTransform.localEulerAngles = new Vector3(0, 180, 0);
        if (!isRightSide)
        {
            canvasTransform.localEulerAngles = new Vector3(0, 0, 0);

        }
    }
}

using UnityEngine;

/// <summary>
/// Script decides if Flora is left or right in Viewport. Based on that the canvas gets moved
/// </summary>
public class CanvasSwitchSides : MonoBehaviour
{
    [SerializeField] private Transform floraTransform;
    [SerializeField] private GameObject canvasObject;
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
        Vector3 viewPortPos = Camera.main.WorldToViewportPoint(floraTransform.position);
        if(viewPortPos.x < 0.5f) { return false; }
        else {  return true; }
    }

    private void SwitchCanvasToSide(bool isRightSide)
    {
        var pos = canvasObject.transform.position;
        var newX = pos.x;
        if (isRightSide) { newX = Mathf.Abs(newX); }
        else { newX = Mathf.Abs(newX) * -1; }
        pos.x = newX;
        canvasObject.transform.position = pos;
    }
}

using TMPro;
using UnityEngine;

public class ControlCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI bodyText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAllText(string header, string body)
    {
        SetHeader(header);
        SetBody(body);
    }

    public void SetHeader(string header)
    {
        this.headerText.text = header;
    }

    public void SetBody(string body)
    {
        this.bodyText.text = body;
    }
}

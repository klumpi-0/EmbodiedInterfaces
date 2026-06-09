using UnityEngine;
using UnityEngine.UI;

public class TestChangeImage : MonoBehaviour
{
    public Sprite sprite;
    public Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetImagesOnMixNMatch.Instance.SetImages(sprite, 0, 2);
        }
    }
}

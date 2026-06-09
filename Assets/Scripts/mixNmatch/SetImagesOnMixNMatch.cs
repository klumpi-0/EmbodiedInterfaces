using UnityEngine;
using UnityEngine.UI;

public class SetImagesOnMixNMatch : MonoBehaviour
{
    public static SetImagesOnMixNMatch Instance;

    [Header("LowCube")]
    [SerializeField] private GameObject imgLow01;
    [SerializeField] private GameObject imgLow02;
    [SerializeField] private GameObject imgLow03;
    [SerializeField] private GameObject imgLow04;

    [Header("MiddleCube")]
    [SerializeField] private GameObject imgMiddle01;
    [SerializeField] private GameObject imgMiddle02;
    [SerializeField] private GameObject imgMiddle03;
    [SerializeField] private GameObject imgMiddle04;

    [Header("HighCube")]
    [SerializeField] private GameObject imgHighe01;
    [SerializeField] private GameObject imgHighe02;
    [SerializeField] private GameObject imgHighe03;
    [SerializeField] private GameObject imgHighe04;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="cube">0 = low, 1 = middle, 2 = high</param>
    /// <param name="numImage"></param>
    public void SetImages(Image image, int cube, int numImage)
    {
        
    }

    private void GetTextureFromImage(Image img)
    {

    }
}

using UnityEngine;

public class FloraController : MonoBehaviour
{
    public static FloraController Instance;

    [SerializeField] private GameObject floraMoveObject;

    private void Awake()
    {
        if(Instance  == null)
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

    public void MoveFlora(Vector3 target)
    {

    }
}

using Oculus.Interaction;
using UnityEngine;

public class FloraGrabProcessor : MonoBehaviour
{
    public static FloraGrabProcessor Instance;

    [Header("References")]
    [SerializeField] private InteractableZoneHandler zoneHandler;
    [SerializeField] private MeshRenderer meshRenderer;

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
        zoneHandler.onEnterZone.AddListener(EnterZoneChanges);
        zoneHandler.onExitZone.AddListener(SetStartBubbleMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetStartBubbleMaterial()
    {
        SetColor(Color.white);
        SetPower(10);
    }

    private void EnterZoneChanges()
    {
        SetColor(Color.peachPuff);
        SetPower(1f);
    }

    private void SetColor(Color color)
    {
        meshRenderer.material.SetColor("_Color", color);
    }

    private void SetPower(float power)
    {
        meshRenderer.material.SetFloat("_Power", power);
    }
}

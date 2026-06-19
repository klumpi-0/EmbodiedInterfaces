using UnityEngine;

public class BeeHover : SpawnedObject
{
    [SerializeField] private ParticleSystem beeParticleSystem;

    public override void ActivateSpecialThing()
    {
        Debug.Log("Activating stuff");
        ActivateParticleSystem();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivateParticleSystem()
    {
        beeParticleSystem.Play();
    }
}

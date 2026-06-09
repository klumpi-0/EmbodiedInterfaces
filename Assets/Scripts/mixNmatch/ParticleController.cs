using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        MixNMatchController.Instance.foundMatchEvent.AddListener(StartParticles);
    }

    public void StartParticles()
    {
        particles.Play();
    }
}

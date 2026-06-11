using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleSystemTester : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystems = GetAllParticleSystems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<ParticleSystem> GetAllParticleSystems()
    {
        var array = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        return array.ToList();
    }

    public void ChangeAmount(float newAmount)
    {
        foreach(ParticleSystem p in particleSystems)
        {
            var emission = p.emission;
            emission.rateOverTime = (int)newAmount;
        }
    }

    public void ChangeTime(float newTime)
    {
        foreach(ParticleSystem p in particleSystems)
        {
            var main = p.main;
            main.duration = newTime;
        }
    }

    public void ChangeSize(float newSize)
    {
        foreach (ParticleSystem p in particleSystems)
        {
            var main = p.main;
            main.startSize = newSize;
        }

    }
}

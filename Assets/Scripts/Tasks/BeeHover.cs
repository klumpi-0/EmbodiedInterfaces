using UnityEngine;

public class BeeHover : SpawnedObject
{
    [SerializeField] private ParticleSystem beeParticleSystem;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip beeClip;
    [SerializeField]
    [Range(0f, 1f)] private float chanceForParticles;

    [SerializeField]
    [Range(0f, 1f)] private float chanceForAudio;
    [SerializeField]
    [Range(0f, 1f)] private float volume;
    [SerializeField] private GameObject soundObject;

    public override void ActivateSpecialThing()
    {
        //if (!ExistingObjectWitBeeSourcetag())
        //{
        //    CreateBeeSoundObject();
        //}
        if(Random.Range(0f, 1f) < chanceForAudio)
        {
            //ActivateAudio();
        }
        if(Random.Range(0f, 1f) < chanceForParticles)
        {
            ActivateParticleSystem();
        }
        else
        {
            RemoveParticleSystem();
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

    private void ActivateParticleSystem()
    {
        beeParticleSystem.Play();
    }

    private void RemoveParticleSystem()
    {
        Destroy(beeParticleSystem);
    }

    private void ActivateAudio()
    {
        audioSource.clip = beeClip;
        audioSource.volume = 1f;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void CreateBeeSoundObject()
    {
        var obj = Instantiate(soundObject);
        foreach(Transform child in obj.transform)
        {
            AudioSource source = child.gameObject.GetComponent<AudioSource>();
            source.clip = beeClip;
            source.loop = true;
            source.volume = volume;
            source.Play();
        }
    }

    private bool ExistingObjectWitBeeSourcetag()
    {
        
        if (GameObject.FindGameObjectWithTag("BeeSource"))
        {
            return true;
        }
        return false;
    }
}

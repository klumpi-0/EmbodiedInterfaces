using System.Collections;
using UnityEngine;

public class MNM_AudioController : MonoBehaviour
{
    public static MNM_AudioController Instance;

    [Header("References")]
    [SerializeField] private AudioSource mnm_audioSource;
    [SerializeField] private MixNMatchController mnm_Controller;

    [Header("Audios")]
    [SerializeField] private AudioClip lastPlayedAudioClip;
    [SerializeField] private AudioClip introText_clip;
    [SerializeField] private AudioClip finishSound_clip;
    [SerializeField] private AudioClip finishText_clip;
    [SerializeField] private AudioClip afterPlantAudio_clip;
    [SerializeField] private AudioClip moreInfo_01;
    [SerializeField] private AudioClip moreInfo_02;
    [SerializeField] private AudioClip moreInfo_03;

    [SerializeField] private AudioClip[] wrongFeedbackClips;

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
        mnm_Controller.foundMatchEvent.AddListener(PlayFinishSound);
        MixNMatchController.Instance.wrongFeedbackEvent.AddListener(PlayRandomWrongFeedback);
        MixNMatchController.Instance.plantedPlantEvent.AddListener(PlayAfterPlantAudio);
    }

    public void SetAudioFiles(AudioClip intro, AudioClip finish, AudioClip afterPlant, AudioClip more_01, AudioClip more_02, AudioClip more_03)
    {
        introText_clip = intro;
        finishText_clip = finish;
        afterPlantAudio_clip = afterPlant;
        moreInfo_01 = more_01;
        moreInfo_02 = more_02;
        moreInfo_03 = more_03;
    }

    /// <summary>
    /// Plays finish sound and first audio with overall information
    /// </summary>
    public void PlayFinishSound()
    {
        StartCoroutine(PlayFinishedAudioCoroutine());
    }
    public IEnumerator PlayFinishedAudioCoroutine()
    {
        PlayAudioClip(finishSound_clip);
        yield return new WaitForSeconds(finishSound_clip.length);
        PlayAudioClip(finishText_clip);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        mnm_audioSource.clip = clip;
        lastPlayedAudioClip = clip;
        mnm_audioSource.Play();
    }

    public void PlayIntroClip()
    {
        PlayAudioClip(introText_clip);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cubeNumber">0 = low, 1 = middle, 2 = high</param>
    public void PlayMoreInfoClip(int cubeNumber)
    {
        switch (cubeNumber)
        {
            case 0: PlayAudioClip(moreInfo_01); break;
            case 1: PlayAudioClip(moreInfo_02); break;
            case 2: PlayAudioClip(moreInfo_03); break;
        }
    }

    public void PlayRandomWrongFeedback()
    {
        int rand = Random.Range(0, wrongFeedbackClips.Length);
        PlayAudioClip(wrongFeedbackClips[rand]);
    }

    private void PlayAfterPlantAudio()
    {
        PlayAudioClip(afterPlantAudio_clip);
    }

    public void PlayAudioClipDelayed(AudioClip clip, float delay)
    {
       StartCoroutine(PlayAudioDelayedEnum(clip, delay));
    }

    private IEnumerator PlayAudioDelayedEnum(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAudioClip(clip);
    }
}

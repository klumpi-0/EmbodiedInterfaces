using System.Collections;
using UnityEngine;

public class MNM_AudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource mnm_audioSource;
    [SerializeField] private MixNMatchController mnm_Controller;

    [Header("Audios")]
    [SerializeField] private AudioClip introText_clip;
    [SerializeField] private AudioClip finishSound_clip;
    [SerializeField] private AudioClip finishText_clip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mnm_Controller.foundMatchEvent.AddListener(PlayFinishSound);
    }


    public void PlayFinishSound()
    {
        //PlayAudioClip(finishSound_clip);
        //PlayAudioClipAfterCurrent(finishText_clip);
        StartCoroutine(PlayFinishedAudioCoroutine());
    }
    public IEnumerator PlayFinishedAudioCoroutine()
    {
        PlayAudioClip(finishSound_clip);
        yield return new WaitForSeconds(finishSound_clip.length);
        PlayAudioClip(finishText_clip);
    }

    private void PlayAudioClip(AudioClip clip)
    {
        mnm_audioSource.clip = clip;
        mnm_audioSource.Play();
    }

    private void PlayAudioClipAfterCurrent(AudioClip clip)
    {
        float remainingTime = mnm_audioSource.clip.length - mnm_audioSource.time;
        mnm_audioSource.clip = clip;
        mnm_audioSource.PlayDelayed(remainingTime);
    }
}

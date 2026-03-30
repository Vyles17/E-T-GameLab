using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void PlaySfx(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(playSFX(audioClip, volume));        
    }
    //public void PauseSfx(AudioClip audioClip)
    //{
    //    AudioSource audioSource = gameObject.AddComponent<AudioSource>();
    //}
    public void StopSfx()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.Stop();
        Destroy(audioSource);
    }
    IEnumerator playSFX(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(audioSource);
    }
}
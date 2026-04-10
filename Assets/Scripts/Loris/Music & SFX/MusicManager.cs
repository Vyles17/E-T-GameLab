using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [HideInInspector] public AudioSource audioSource;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void PlayBg(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(PlayBgMusic(audioClip, volume));
    }

    public void PlayFinal(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(PlayWinOrGO(audioClip, volume));
    }

    IEnumerator PlayBgMusic(AudioClip audioClip, float volume = 1f)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        if (GameManager.Instance.isWinning || GameManager.Instance.isGameOver)
        {
            Destroy(audioSource);
            yield break;
        }
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(audioSource);
    }
    IEnumerator PlayWinOrGO(AudioClip audioClip, float volume = 1f)
    {
        AudioSource AS = gameObject.AddComponent<AudioSource>();
        AS.clip = audioClip;
        AS.volume = volume;
        AS.Play();
        yield return new WaitForSeconds(AS.clip.length);
        Destroy(AS);
    }
}
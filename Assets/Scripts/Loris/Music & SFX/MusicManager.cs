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

    public void PlaySfx(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(playSFX(audioClip, volume));
    }

    IEnumerator playSFX(AudioClip audioClip, float volume = 1f)
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
}
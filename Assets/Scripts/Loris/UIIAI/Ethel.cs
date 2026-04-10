using UnityEngine;
using UnityEngine.EventSystems;

public class Ethel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioClip UIIAI;
    [SerializeField] float UiiaiDuration;

    AudioSource audioSource;
    StopEthel stopEthel;

    private void Awake()
    {
        stopEthel = GetComponentInChildren<StopEthel>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if ((GameManager.Instance.isPaused || stopEthel.stopUiiaing) && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        stopEthel.stopUiiaing = false;
        audioSource.clip = UIIAI;

        audioSource.Play();
    }
}

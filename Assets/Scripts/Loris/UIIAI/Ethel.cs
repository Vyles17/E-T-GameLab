using UnityEngine;
using UnityEngine.EventSystems;

public class Ethel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioClip UIIAI;
    [SerializeField] Animator _Ethel;

    AudioSource audioSource;
    //InteractUIIAI interactUIIAI;

    private void Awake()
    {
        //interactUIIAI = GetComponentInChildren<InteractUIIAI>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!GameManager.Instance.isPaused /*interactUIIAI.stopUiiaing*/)
        {
            float dist = Vector3.Distance(transform.position, Movement.Instance.transform.position);

            if (dist > GameManager.Instance.EthelDistance)
            {
                _Ethel.SetBool("isSpinning", false);
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //if (!interactUIIAI)
        //{
        //calcolo se il player è abbastanza vicino per interagire con l'oggetto
        float dist = Vector3.Distance(transform.position, Movement.Instance.transform.position);

        if (dist > GameManager.Instance.EthelDistance) return;

        Debug.Log("UIIAI");
            audioSource.clip = UIIAI;
            _Ethel.SetBool("isSpinning", true);
            audioSource.Play();
        //}
    }
}

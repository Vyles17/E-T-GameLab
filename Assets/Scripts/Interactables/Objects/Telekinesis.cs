using UnityEngine;
using UnityEngine.EventSystems;

public class Telekinesis : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] float teleTime;

    public bool interacted;

    [SerializeField] Vector3 startPoint;
    [SerializeField] Vector3 endPoint;
    private float teleTimer;
    [SerializeField] float transitionDuration;
    
    private void Start()
    {
        startPoint = transform.position;
    }
    private void Update()
    {
        //elapsedTime += Time.deltaTime;
        //float percentageComplate = elapsedTime / transitionDuration;

        //transform.position = Vector3.Lerp(startPoint, endPoint, percentageComplate);
    }
    protected virtual void FixedUpdate()
    {
        if (interacted)
        {
            teleTimer += Time.fixedDeltaTime;
            float percentageComplate = teleTimer / transitionDuration;

            transform.position = Vector3.Lerp(startPoint, endPoint, percentageComplate);

            if (transform.position == endPoint)
            { 
                interacted = false;
                //transform.position = startPoint;
                gameObject.SetActive(false);
            }
            //if (teleTimer > teleTime)
            //{
            //    //aggiungere Lerp
            //    teleTimer = 0;
            //}
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!interacted)
        {
            interacted = true;
            if (!Movement.Instance.freezed) 
            {
                Debug.Log("Initial Energy:" + Movement.Instance.currentEnergy);
                Movement.Instance.currentEnergy -= Movement.Instance.telekinesisEnergy;
                Debug.Log(Movement.Instance.currentEnergy + "- Initial Energy =" + Movement.Instance.currentEnergy);
            }
        }
    }
}

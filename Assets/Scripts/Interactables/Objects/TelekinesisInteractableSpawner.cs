using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class TelekinesisInteractableSpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] float teleTime;

    public bool interacted;

    private Vector3 startPoint;
    private float teleTimer;
    [SerializeField] float transitionDuration;

    //prefabs da spawnare
    [SerializeField] GameObject[] antennaPartsPrefabs;
    [SerializeField] GameObject powerCandyPrefab;
    [SerializeField] GameObject lifeCandyPrefab;

    //bool per non far respawnare nello stesso spawner un nuovo oggetto
    [HideInInspector] public bool justEmptiedInteractable = false;

    //percentuale serializzata di spawn dei pezzi di antenna o delle power candy
    [SerializeField] float antennaPartChance = 15f;
    [SerializeField] float powerCandyChance = 35f; //il restante è per le life candy

    private void Start()
    {
        startPoint = transform.position;
    }
    //private void Update()
    //{
    //    elapsedTime += Time.deltaTime;
    //    float percentageComplate = elapsedTime / transitionDuration;

    //    transform.position = Vector3.Lerp(startPoint, endPoint, percentageComplate);
    //}
    //protected virtual void FixedUpdate()
    //{
        
    //    if (interacted)
    //    {
    //        teleTimer += Time.fixedDeltaTime;
    //        float percentageComplate = teleTimer / transitionDuration;

    //        transform.position = Vector3.Lerp(startPoint, endPoint, percentageComplate);

    //        if (transform.position == endPoint)
    //        { 
    //            interacted = false;
    //            //transform.position = startPoint;
    //            gameObject.SetActive(false);
    //        }
    //    }
        
    //}
    public void OnPointerClick(PointerEventData eventData)
    {
        //se il tag non è Interactable, ritorno
        if (!transform.CompareTag("Interactable")) return;

        // avvio animazione lerp + spawno un oggetto (e il player lo prende automaticamente)
        StartCoroutine(SpawnRoutine());

        //setto il tag "Untagged" e lo segno come JustEmptied
        transform.tag = "Untagged";
        justEmptiedInteractable = true;

        //attivo un altro spawner al posto suo
        SpawnerManager.Instance.activateInteractableSpawner(gameObject);

        //scaliamo l'energia
        if (!interacted)
        {
            interacted = true;
            
            //usciamo dalla modalità ET
            GameManager.Instance.ExitETMode();

            if (!Movement.Instance.freezed)
            {
                Debug.Log("Initial Energy:" + Movement.Instance.currentEnergy);
                Movement.Instance.currentEnergy -= Movement.Instance.telekinesisEnergy;
                Debug.Log(Movement.Instance.currentEnergy + "- Initial Energy =" + Movement.Instance.currentEnergy);
            }
        }
    }

        //metodo per generare un oggetto
        GameObject ObjectSpawn()
    {
        // genera un numero casuale da 0 a 100
        float randomObjectChance = Random.Range(0f, 100f);

        //prepariamo il nostro prefab...
        GameObject prefabToSpawn;

        /* 
        //se il numero generato rientra nella percentuale dell'antenna
        if (randomObjectChance <= antennaPartChance)
        {
            //spawno uno dei pezzi di antenna che ci mancano
            //DA SETTARE (vyles del futuro, sotto cambia l' "if" in "else if"!!
        }
        */

        //se il numero generato rientra nella percentuale della power candy
        if (randomObjectChance > antennaPartChance && randomObjectChance <= antennaPartChance + powerCandyChance)
        {
            //spawno una power candy
            prefabToSpawn = powerCandyPrefab;
        }

        else
        {
            //altrimenti spawna una delle due caramelle
            prefabToSpawn = lifeCandyPrefab;
        }

        //creo l'oggetto nella posizione originale del parent 
        GameObject spawnedObj = Instantiate(prefabToSpawn, startPoint, Quaternion.identity);

        return spawnedObj;
    }

    //coroutine per fare l'animazione lerpata dell'oggetto + collezione dell'oggetto
    IEnumerator SpawnRoutine()
    {
        //creo l'oggetto
        GameObject spawnedObject = ObjectSpawn();

        //posizione dove arriva l'oggetto telecinesato lol (roccia, tronco, boh)
        Vector3 targetPos = startPoint + Vector3.up * 2f;

        transitionDuration = 3f;
        teleTimer = 0f;

        //la nostra animazione fluttuante
        while (teleTimer < transitionDuration)
        {
            transform.position = Vector3.Lerp(startPoint, targetPos, teleTimer / transitionDuration);

            teleTimer += Time.deltaTime;
            yield return null;
        }

        // una volta arrivati alla posizione finale.....
        spawnedObject.transform.position = targetPos;

        //VYLES DEL FUTURO AGGIUNGI QUI IL METODO SE L'OGGETTO E' UN PEZZO DI ANTENNA

        //se l'oggetto è una power Candy
        if (spawnedObject.CompareTag("PowerCandy"))
        {
            // la prendiamo, aggiorniamo l'inventario
            PowerCandyManager.Instance.AddCandy(1);

            //la distruggiamo
            Destroy(spawnedObject);
        }

        else if (spawnedObject.CompareTag("LifeCandy"))
        {
            // la prendiamo, aggiorniamo la stamina, e la distruggiamo
            Movement.Instance.currentEnergy += Movement.Instance.addEnergy;
            if (Movement.Instance.currentEnergy > Movement.Instance.maxEnergy)
            {
                Movement.Instance.currentEnergy = Movement.Instance.maxEnergy;
            }
            Destroy(spawnedObject);
        }

        // l'oggetto interagito aspetta 2 secondi
        yield return new WaitForSeconds(2f);

        //poi torna alla sua posizione originale
        transform.position = startPoint;

        //e disattiviamo l'effetto glow
        transform.GetChild(0).gameObject.SetActive(false);

        interacted = false;
    }

}

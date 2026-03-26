using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BloomingInteractableSpawner : MonoBehaviour, IPointerClickHandler
{
    // script per gli spawner interagibili in game che possono dare un pezzo di antenna

    //prefabs da spawnare
    [SerializeField] GameObject[] antennaPartsPrefabs;
    [SerializeField] GameObject powerCandyPrefab;
    [SerializeField] GameObject lifeCandyPrefab;

    //serve per cambiare la mesh del cespuglio
    MeshFilter meshFilter;
    //le mesh del cespuglio e del cespuglio in fiore da sostituire quando interagiamo
    public Mesh bush;
    public Mesh bloomedBush;
    //bool per non far respawnare nello stesso spawner un nuovo oggetto
    [HideInInspector] public bool justEmptiedInteractable = false;
    //percentuale serializzata di spawn dei pezzi di antenna o delle power candy
    [SerializeField] float antennaPartChance = 15f;
    [SerializeField] float powerCandyChance = 35f; //il restante è per le life candy
    //timer per l'animazione
    private float teleTimer;
    [SerializeField] float transitionDuration;

    public bool interacted;

    void Start()
    {
        //mi getto il component
        meshFilter = GetComponent<MeshFilter>();
    }

    //metodo per ottenere un oggetto (solo in spawner vuoti!) quando viene cliccato il prefab cespuglietto
    public void OnPointerClick(PointerEventData eventData)
    {
        //se il tag non è Interactable, ritorno
        if (!transform.CompareTag("Interactable")) return;

        //appena clicco, il cespuglio cambia mesh in un cespuglio fiorito
        meshFilter.mesh = bloomedBush;

        // avvio animazione lerp + spawno un oggetto (e il player lo prende automaticamente)
        StartCoroutine(SpawnRoutine());

        //setto il tag "Untagged" e lo segno come JustEmptied
        transform.tag = "Untagged";
        justEmptiedInteractable = true;

        //LORIS QUA E' DA METTERE L'IF PER IL FREEZE
        //scaliamo l'energia
        if (!interacted)
        {
            //usciamo dalla modalità ET
            GameManager.Instance.ExitETMode();
            if (!Movement.Instance.freezed)
            {
                Movement.Instance.currentEnergy -= Movement.Instance.telekinesisEnergy;
            }
        }
        //attivo un altro spawner al posto suo
        SpawnerManager.Instance.activateInteractableSpawner(gameObject);
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

        //creo l'oggetto nella posizione dello spawner (e nel suo parent spawner)
        GameObject spawnedObj = Instantiate(prefabToSpawn, transform.position, Quaternion.identity, transform);

        return spawnedObj;
    }

    //coroutine per fare l'animazione lerpata dell'oggetto + collezione dell'oggetto
    IEnumerator SpawnRoutine()
    {
        //creo l'oggetto
        GameObject spawnedObject = ObjectSpawn();

        //posizione da cui parte e dove arriva l'oggetto spawnato
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + Vector3.up * 2f;

        transitionDuration = 3f;
        teleTimer = 0f;

        //la nostra animazione fluttuante
        while (teleTimer < transitionDuration)
        {
            spawnedObject.transform.position = Vector3.Lerp(startPos, targetPos, teleTimer / transitionDuration);

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

        // ripristiniamo la mesh originale
        meshFilter.mesh = bush;

        //e disattiviamo l'effetto glow
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
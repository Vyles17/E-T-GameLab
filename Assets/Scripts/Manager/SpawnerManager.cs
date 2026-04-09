using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    // script che gestirà gli spawner

    [Header("Spawner Normali")]
    //lista di spawnerpoints normali
    [SerializeField] GameObject[] normalSpawners;
    //int di quanti spawner points normali vogliamo attivi per volta
    [SerializeField] int activeNormalSpawnersQuantity;
    //lista di spawner attivi
    List<GameObject> activeNormalSpawners = new List<GameObject>();

    [Header("Spawner Interagibili")]
    //lista di spawnerpoints interagibili
    [SerializeField] GameObject[] interactableSpawners;
    //int di quanti spawner points interagibili sono attivi per volta
    [SerializeField] int activeInteractableSpawnersQuantity;
    //lista di spawner attivi
    List<GameObject> activeInteractableSpawners = new List<GameObject>();

    public static SpawnerManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        //selezioniamo casualmente gli spawner normali attivi all'inizio
        ActivateSpawners(normalSpawners, activeNormalSpawnersQuantity, activeNormalSpawners);

        // e gli spawniamo una caramella dentro
        foreach (GameObject spawner in activeNormalSpawners)
        {
            spawner.GetComponent<NormalSpawner>().CandiesSpawn();
        }

        //selezioniamo casualmente gli spawner interactables attivi all'inizio
        ActivateSpawners(interactableSpawners, activeInteractableSpawnersQuantity, activeInteractableSpawners);

        // e a ognuno gli settiamo il tag "interactable"
        foreach (GameObject spawner in activeInteractableSpawners)
        {
            spawner.tag = "Interactable";
        }
    }

    //metodo per attivare gli spawners all'inizio (sia normali che interactables)
    void ActivateSpawners(GameObject[] spawnersType, int quantity, List<GameObject> activeSpawns)
    {
        // ci gettiamo lista così possiamo rimuovere spawners senza problemi dopo
        List<GameObject> spawnsList = new List<GameObject>(spawnersType);

        //attiviamo il numero scelto di spawner 
        for (int i = 0; i < quantity && spawnsList.Count > 0; i++)
        {
            //prendiamo uno spawner casuale
            int randomSpawner = Random.Range(0, spawnsList.Count);

            //lo aggiungiam alla lista degli spawner attivi
            activeSpawns.Add(spawnsList[randomSpawner]);

            // lo togliamo dalla lista per evitare duplicati (quando arrivo al numero che abbiamo scelto di spawner attivi, si ferma)
            spawnsList.RemoveAt(randomSpawner);
        }
    }

    //metodo per riempire uno spawner normale dopo che un altro è stato svuotato
    public void RespawnCandy(GameObject justEmptiedSpawner)
    {
        // nuova lista per gli spawner vuoti
        List<GameObject> emptySpawners = new List<GameObject>();

        //per ogni spawner negli spawner normali
        foreach (GameObject spawner in normalSpawners)
        {
            //se lo spawner non ha già caramelle e non è appena stato svuotato
            if (spawner.transform.childCount == 0 && spawner != justEmptiedSpawner)
            {
                //lo aggiungiamo alla lista di nuovi spawner papabili per il respawn caramella
                emptySpawners.Add(spawner);
            }
        }

        // scegliamo uno spawner vuoto casuale per il respawn
        GameObject respawnSpawner = emptySpawners[Random.Range(0, emptySpawners.Count)];
        respawnSpawner.GetComponent<NormalSpawner>().CandiesSpawn();

        // ora disattiviamo il bool JustEmptied
        justEmptiedSpawner.GetComponent<NormalSpawner>().justEmptied = false;
    }

    //metodo per attivare uno spawner interactable dopo che un altro è stato svuotato
    public void activateInteractableSpawner(GameObject justEmptiedSpawner)
    {
        // rimuovo lo spawner appena usato dalla lista degli attivi
        activeInteractableSpawners.Remove(justEmptiedSpawner);

        // nuova lista per gli spawner vuoti
        List<GameObject> emptySpawners = new List<GameObject>(interactableSpawners);

        //rimuoviamo dalla lista degli spanwer "attivabili" quelli già attivi
        foreach (GameObject spawner in activeInteractableSpawners)
            emptySpawners.Remove(spawner);

        // scegliamo uno spawner vuoto casuale da attivare
        GameObject activeSpawner = emptySpawners[Random.Range(0, emptySpawners.Count)];

        // lo aggiungo alla lista degli spawner attivi
        activeInteractableSpawners.Add(activeSpawner);

        // cambiamo il tag (così che possa avere anche l'effetto glow degli interagibili)
        activeSpawner.tag = "Interactable";

        // resettiamo il justEmptied sullo spawner appena svuotato
        var bloom = justEmptiedSpawner.GetComponent<BloomingInteractableSpawner>();
        if (bloom != null) bloom.justEmptiedInteractable = false;

        var telekinesis = justEmptiedSpawner.GetComponent<TelekinesisInteractableSpawner>();
        if (telekinesis != null) telekinesis.justEmptiedInteractable = false;
    }
}

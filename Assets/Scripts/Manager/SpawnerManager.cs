using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] int activeInteractablesSpawnersQuantity;
    //lista di spawner attivi
    List<GameObject> activeInteractables = new List<GameObject>();

    //prefab pezzi Antenna
    [SerializeField] GameObject[] antennaPartsPrefabs;
    [SerializeField] float antennaPartsChance = 20f;
    bool antennaPartSpawned = false;

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
        //selezioniamo casualmente gli spawner normali attivi
        ActivateSpawners(normalSpawners, activeNormalSpawnersQuantity, activeNormalSpawners);

        // e gli spawniamo una caramella dentro
        foreach (GameObject spawner in activeNormalSpawners)
        {
            spawner.GetComponent<NormalSpawner>().CandiesSpawn();
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

        // scegli uno spawner vuoto casuale
        GameObject respawnSpawner = emptySpawners[Random.Range(0, emptySpawners.Count)];
        respawnSpawner.GetComponent<NormalSpawner>().CandiesSpawn();
        justEmptiedSpawner.GetComponent<NormalSpawner>().justEmptied = false;
    }


    //metodo per spawnare i pezzi di antenna (solo negli interactables!!)
    void AntennaSpawn()
    {

    }

}

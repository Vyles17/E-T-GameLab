using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SpawnerManager : MonoBehaviour
{
    // script che gestirà lo spawn degli oggetti

    [Header("Spawner Normali")]
    //lista di spawnerpoints normali
    [SerializeField] GameObject[] normalSpawners;
    //int di quanti spawner points normali sono attivi per volta
    [SerializeField] int activenormalSpawnersQuantity;
    //prefabs caramelle
    [SerializeField] GameObject powerCandyPrefab;
    [SerializeField] GameObject lifeCandyPrefab;

    [Header("Spawner degli Interagibili")]
    //lista di spawnerpoints interagibili
    [SerializeField] GameObject[] interactableSpawners;
    //int di quanti spawner points interagibili sono attivi per volta
    [SerializeField] int activeInteractablesSpawnersQuantity;
    //prefab pezzi Antenna
    [SerializeField] GameObject[] antennaPartsPrefabs;

    [Header("Percentuali di Spawn")]
    //percentuali di spawn
    [SerializeField] float powerCandyChance = 35f; //il restante numero della percentuale è per le lifeCandy
    [SerializeField] float specialItemChance = 30f;

    //lista di spawner attivi
    List<GameObject> activeNormals = new List<GameObject>();
    List<GameObject> activeInteractables = new List<GameObject>();

    bool antennaPartSpawned = false;

    void Start()
    {
        //ci settiamo gli spawner all'inizio
        ActivateSpawners(normalSpawners, activenormalSpawnersQuantity, activeNormals);

        //spawno solo negli spawner attivi
        CandiesSpawn();
    }

    //metodo per attivare gli spawners (sia normali che interactables)
    void ActivateSpawners(GameObject[] spawnersType, int quantity, List<GameObject> activeSpawns)
    {
        // ci gettiamo lista così possiamo rimuovere spawners senza problemi dopo
        List<GameObject> spawnsList = new List<GameObject>(spawnersType);

        //attiviamo il numero scelto di spawner 
        for (int i = 0; i < quantity && spawnsList.Count > 0; i++)
        {
            //prendiamo uno spawner casuale
            int randomSpawner = Random.Range(0, spawnsList.Count);
            GameObject chosenSpawn = spawnsList[randomSpawner];

            //lo aggiungiam alla lista degli spawner attivi
            activeSpawns.Add(chosenSpawn);

            // lo togliamo dalla lista per evitare duplicati (quando arrivo al numero che abbiamo scelto di spawner attivi, si ferma)
            spawnsList.RemoveAt(randomSpawner);
        }
    }

    //metodo per spawnare caramelle dagli spawner normali
    void CandiesSpawn()
    {
        // per ogni spawner normale attivo
        foreach (GameObject spawner in activeNormals)
        {
            // genera un numero casuale da 0 a 100
            float randomCandy = Random.Range(0f, 100f);

            GameObject prefabToSpawn;

            //se rientra nella percentuale della power candy, ne spawna una
            if (randomCandy < powerCandyChance)
            {
                prefabToSpawn = powerCandyPrefab;
            }
            else
            {
                //altrimenti spawna la life candy
                prefabToSpawn = lifeCandyPrefab;
            }

            //creo l'oggetto nella posizione dello spawner (e nel suo parent spawner)
            Instantiate(prefabToSpawn, spawner.transform.position, Quaternion.identity, spawner.transform);
        }
    }

}

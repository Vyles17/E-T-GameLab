using UnityEngine;

public class NormalSpawner : MonoBehaviour
{
    // script per gli spawner normali in game di sole caramelle
    
    //prefabs caramelle
    [SerializeField] GameObject powerCandyPrefab;
    [SerializeField] GameObject lifeCandyPrefab;
    //bool per non far respawnare nello stesso spawner una nuova caramella
    [HideInInspector] public bool justEmptied = false;
    //percentuale serializzata di spawn delle power candy
    [SerializeField] float powerCandyChance = 35f; //il restante numero della percentuale è per le lifeCandy

    //metodo per spawnare una caramella (solo in spawner vuoti!)
    public void CandiesSpawn()
    {
        //se il parent spawn ha già un figlio, vuol dire che è "occupato" allora non facciamo niente
        if (transform.childCount > 0) return; 

        // genera un numero casuale da 0 a 100
        float randomCandyChance = Random.Range(0f, 100f);

        //prepariamo il nostro prefab...
        GameObject prefabToSpawn;

        //se il numero generato rientra nella percentuale della power candy, ne spawna una
        if (randomCandyChance <= powerCandyChance)
        {
            prefabToSpawn = powerCandyPrefab;
        }

        else
        {
            //altrimenti spawna la life candy
            prefabToSpawn = lifeCandyPrefab;
        }

        // posizione piiù in alto del suo parent dove venir spawnato
        Vector3 spawnPosition = transform.position + Vector3.up * 1f;

        //creo l'oggetto nella posizione dello spawner (e nel suo parent spawner)
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
    }

}

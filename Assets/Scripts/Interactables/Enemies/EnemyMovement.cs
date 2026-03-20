using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour, IInteractable
{
    //script classe madre per il movimento dell'enemy

    public NavMeshAgent enemyAgent; //il nostro nemico
    public Transform targetPlayer; // il player
    public List<Transform> waypoints; // i waypoints per cui passeranno i nemici
    public int currentWaypoint = 0;

    public int triggerRadius = 30; //il raggio dell'overlap sphere entro il quale l'enemy si accorge del player
    [SerializeField] private LayerMask playerLayermask; //il layer del player

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        //se il player finisce nel raggio dell'enemy (e non è già in gabbia/ha caramelle)
        if (Physics.OverlapSphere(transform.position, triggerRadius, playerLayermask).Length > 0 && CanChasePlayer())
        {
            //l'enemy lo insegue
            GetET();
        }

        //altrimenti se ne va a zonzo arrrandom sperando di beccare il player
        else
        {
            SearchForET();
        }
    }

    void SearchForET()
    {
        //se non ci sono waypoints settati, non fa niente
        if (waypoints.Count == 0)
            return;
        
        //calcolo la distanza al prossimo waypoint
        float distanceToNextWaypoint = Vector3.Distance(waypoints[currentWaypoint].position, transform.position); 

        //se sono abbastanza vicino al waypoint
        if (distanceToNextWaypoint <= 3)
        {
            //scorro la lista di waypoints (mettendo quel +1, creo il loop)
            currentWaypoint = (currentWaypoint  + 1) % waypoints.Count;
        }

        //E ora vai, figlio mio.
        enemyAgent.SetDestination(waypoints[currentWaypoint].position);
    }

    void GetET()
    {
        //lo insegue
        enemyAgent.SetDestination(targetPlayer.position); 
    }

    //ci serve per capire se possiamo inseguirlo (se è in gabbia o se non ha caramelle, non lo inseguiamo)
    protected virtual bool CanChasePlayer()
    {
        return true; // di base, possiamo
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

    public void OnInteraction()
    {
        //aggiungere qui il fatto che si stunnino
    }

}

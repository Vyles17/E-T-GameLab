using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    //script per il movimento dell'enemy

    NavMeshAgent enemyAgent; //il nostro nemico
    public Transform targetPlayer; // il player
    public Transform cage; // la nostra gabbia
    private bool isCaged; //lo stato del player in cui non può muoversi dalla gabbia

    public int triggerRadius = 30; //il raggio dell'overlap sphere entro il quale l'enemy si accorge del player
    [SerializeField] private LayerMask playerLayermask; //il layer del player

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        //se il player finisce nel raggio dell'enemy (e non è già in gabbia)
        if (!isCaged && Physics.OverlapSphere(transform.position, triggerRadius, playerLayermask).Length > 0)
        {
            //l'enemy lo insegue
            GetET();

            //se raggiunge il player
            if (!enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                //porta il player nella gabbia
                CageET();
            }
        }

        //altrimenti se ne va a zonzo arrrandom sperando di beccare il player
        else
        {
            SearchForET();
        }
    }

    void SearchForET()
    {

    }

    void GetET()
    {
        enemyAgent.SetDestination(targetPlayer.position); //lo insegue
    }

    //poi magari trasferisco le trappole dei nemici in uno script a parte
    void CageET()
    {
        //gettiamo la posizione della gabbia, ma manteniamo la y del player
        Vector3 cagedETposition = new Vector3(cage.position.x, targetPlayer.position.y, cage.position.z);

        //ci gettiamo il suo rigidbody per "freezarlo" nella gabbia
        Rigidbody playerRB = targetPlayer.GetComponent<Rigidbody>();

        //e lo spostiamo e ingabbiamo
        playerRB.position = cagedETposition; 
        //playerRB.constraints = RigidbodyConstraints.FreezeAll;

        //settiamo lo status
        isCaged = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}

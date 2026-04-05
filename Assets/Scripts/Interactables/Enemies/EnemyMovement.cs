using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour, IPointerClickHandler
{
    //script classe madre per il movimento dell'enemy

    [Header("Agent Settings")]
    public NavMeshAgent enemyAgent; //il nostro nemico
    public Transform targetPlayer; // il player
    public List<Transform> waypoints; // i waypoints per cui passeranno i nemici
    public Transform baseWaypoint; //il waypoint casa madre dove tornano dopo aver fatto le loro malefatte
    public int currentWaypoint = 0;

    public int triggerRadius = 30; //il raggio dell'overlap sphere entro il quale l'enemy si accorge del player
    [SerializeField] private LayerMask playerLayermask; //il layer del player


    [Header("Stun Stats")]
    Rigidbody rb;
    private float speed;
    public bool stunned = false;
    [SerializeField] float stunTime;
    private float stunTimer;
    //Sound
    [SerializeField] AudioClip stunSfx;


    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        speed = enemyAgent.speed;
    }
    protected virtual void Update()
    {
        if (!stunned)
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
    }

    protected virtual void FixedUpdate()
    {
        if (stunned)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            stunTimer += Time.fixedDeltaTime;
            enemyAgent.speed = 0;

            if (stunTimer > stunTime)
            {
                rb.constraints = RigidbodyConstraints.None;
                stunned = false;
                enemyAgent.speed = speed;
                stunTimer = 0;
            }
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
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
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


    //Stun PowerUp
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!stunned)
        {
            //faccio partire il suono dello stun
            AudioManager.instance.PlaySfx(stunSfx);

            stunned = true;
            if (!Movement.Instance.freezed)
            {
                Debug.Log("Initial Energy:" + Movement.Instance.currentEnergy);
                Movement.Instance.currentEnergy -= Movement.Instance.stunEnergy;
                Debug.Log(Movement.Instance.currentEnergy + "- Initial Energy =" + Movement.Instance.currentEnergy);
            }

            //usciamo dalla modalità ET
            GameManager.Instance.ExitETMode();
        }
    }
}
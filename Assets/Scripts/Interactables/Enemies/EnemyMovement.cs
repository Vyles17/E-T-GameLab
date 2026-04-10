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

    [Header("Agent Legs settings")]
    [SerializeField] Transform leftLeg; //mesh gamba sx da far lerpare per il movimento
    [SerializeField] Transform rightLeg; //mesh gamba dx da far lerpare per il movimento
    [SerializeField] float maxAngle = 15f; // quanto si muovono le gambe
    private float animDuration = 0.4f;
    private float legsTimer;

    [Header("Stun Stats")]
    Rigidbody rb;
    private float speed;
    public bool stunned = false;
    [SerializeField] float stunTime;
    private float stunTimer;
  

    //Sound
    [SerializeField] AudioClip stunSfx;
    [SerializeField] AudioClip walkingSfx;

    AudioSource audioSource;


    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        speed = enemyAgent.speed;
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        audioSource.clip = walkingSfx;

        if (!stunned || Time.timeScale == 1)
        {
            HandleWalkingAudio();

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

            //lerp per l'animazione delle gambe del nemico
            legsTimer += Time.deltaTime;
            float anim = Mathf.PingPong(legsTimer / animDuration, 1f);
            float angle = Mathf.Lerp(-maxAngle, maxAngle, anim);

            leftLeg.localRotation = Quaternion.Euler(angle, 0, 0);
            rightLeg.localRotation = Quaternion.Euler(-angle, 0, 0);
        }

        else
        {
            // Se è stunnato o il gioco è in pausa, fermiamo l'audio (e non cammina)
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }
    void HandleWalkingAudio()
    {
        // Controlliamo se l'agente si sta effettivamente muovendo
        if (enemyAgent.velocity.magnitude > 0.1f)
        {
            audioSource.clip = walkingSfx;

            // IMPORTANTE: Play() viene chiamato solo se non sta già suonando
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Se è fermo stoppiamo l'audio
            audioSource.Stop();
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
        //calcolo se il player è abbastanza vicino per interagire con l'oggetto
        float dist = Vector3.Distance(transform.position, Movement.Instance.transform.position);

        if (dist > GameManager.Instance.enemyInteractionDistance)
        {
            return;
        }

        if (!stunned && Time.timeScale > 0)
        {
            //faccio partire il suono dello stun
            AudioManager.instance.PlaySfx(stunSfx);

            stunned = true;
            if (!Movement.Instance.freezed)
            {
                Movement.Instance.currentEnergy -= Movement.Instance.stunEnergy;
            }

            //usciamo dalla modalità ET
            GameManager.Instance.ExitETMode();
        }
    }
}
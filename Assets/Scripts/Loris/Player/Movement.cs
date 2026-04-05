using System;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Camera turning variables
    public Transform playerCamera;
    [SerializeField] float sensitivity;
    private float cameraPitch;

    //Movement variables
    Rigidbody rb;
    public Vector3 Direction;
    [SerializeField] float speed;
    private float currentSpeed;
    public static event Action OnGameOver;

    [Header("Energy Stats")]
    public int maxEnergy;
    public float currentEnergy;
    [SerializeField] public float addEnergy;
    public float detractEnergy;
    [SerializeField] float moveEnergy;
    [SerializeField] float sprintEnergy;
    public float stunEnergy;
    public float telekinesisEnergy;
    public float freezeEnergy;

    public bool freezed = false;
    private float freezeTimer;
    public float freezeTime;

    GameObject CollObj;

    [Header("Sounds")]
    //moving
    [SerializeField] AudioClip footStepsSfx;
    [SerializeField] AudioClip runnningStepsSfx;

    //items
    public AudioClip pickUpSfx;

    //Powerups
    [SerializeField] AudioClip freezeEnergySfx;

    public static Movement Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        rb = GetComponent<Rigidbody>();

    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentEnergy = maxEnergy;
        detractEnergy = moveEnergy;
        StartCoroutine(PlayFootsteps());
        StartCoroutine(PlayRun());
    }
    private void Update()
    {
        if (!UIManager.Instance.pauseMenuUI.activeSelf && !UIManager.Instance.GameOverMenuUI.activeSelf)
        {
            Camera();
            Move();
            if(freezed)
            {
                freezeTimer += Time.deltaTime;
                if(freezeTimer >= freezeTime)
                {
                    freezed = false;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        currentSpeed = speed;
        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= 3;
            detractEnergy = sprintEnergy;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            detractEnergy = moveEnergy;
        }
        //Movement velocity calculator (la aggiorni in Update perché deve tornare alla normalità una volta che non si preme lo sprint)
        Vector3 targetVelocity = Time.fixedDeltaTime * currentSpeed * Direction;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }
    private void Camera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX); //ruota il Player in Y

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0); //ruota la camera in X
    }
    private void Move()
    {
        float xmuv = Input.GetAxis("Horizontal");
        float zmuv = Input.GetAxis("Vertical");
        Vector3 localDirection = new(xmuv, 0, zmuv);

        Direction = transform.TransformDirection(localDirection); //permette al Player di seguire la Camera

        Direction.Normalize();
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && currentEnergy > 0 && !freezed)
        {
            currentEnergy -= detractEnergy;
        }

        if (currentEnergy <= 0)
        {
            OnGameOver?.Invoke();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlaySfx(pickUpSfx);

        CollObj = other.gameObject;

        //se abbiamo pigliato una caramella poteri
        if (CollObj.CompareTag("PowerCandy"))
        {
            GameObject candy = other.gameObject;
            GameObject spawner = candy.transform.parent.gameObject; // il parent è lo spawner

            // la prendiamo, aggiorniamo l'inventario, e la distruggiamo
            PowerCandyManager.Instance.AddCandy(1);
            Destroy(candy);

            // segnalo come appena svuotato
            spawner.GetComponent<NormalSpawner>().justEmptied = true;

            // diciamo al manager di spawnare una nuova caramella
            SpawnerManager.Instance.RespawnCandy(spawner);
        }

        //se abbiamo pigliato una caramella vita
        else if (CollObj.CompareTag("LifeCandy"))
        {
            GameObject candy = other.gameObject;
            GameObject spawner = candy.transform.parent.gameObject; // il parent è lo spawner

            // la prendiamo, aggiorniamo la stamina, e la distruggiamo
            currentEnergy += addEnergy;
            if(currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            Destroy(candy);

            // segnalo come appena svuotato
            spawner.GetComponent<NormalSpawner>().justEmptied = true;

            // diciamo al manager di spawnare una nuova caramella
            SpawnerManager.Instance.RespawnCandy(spawner);
        }
    }

    public void FreezeEnergy()
    {
        if (!freezed && PowerCandyManager.Instance.currentCandies > 0 && Time.timeScale > 0)
        {
            freezed = true;
            AudioManager.instance.PlaySfx(freezeEnergySfx);
            freezeTimer = 0;
            PowerCandyManager.Instance.RemoveCandy(1);
        }
    }
    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            if (Direction.magnitude > 0.1f && !Input.GetKey(KeyCode.LeftShift))
            {
                AudioManager.instance.PlaySfx(footStepsSfx);
            }
            yield return new WaitForSeconds(0.6f);
        }
    }
    IEnumerator PlayRun()
    {
        while (true)
        {
            if (Direction.magnitude > 0.1f && Input.GetKey(KeyCode.LeftShift))
            {
                AudioManager.instance.PlaySfx(runnningStepsSfx);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    // scusa loris te lo commento perchè devo fare tutto da OnTriggerEnter :p

    //private void TakeCandy(int candies)
    //{
    //    PowerCandyManager.Instance.AddCandy(1);
    //    Destroy(CollObj);
    //}
}

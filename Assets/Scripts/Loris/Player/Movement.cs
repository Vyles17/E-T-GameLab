using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Camera turning variables
    public Transform playerCamera;
    [SerializeField] float sensitivity;
    [HideInInspector] public float cameraPitch;

    //Movement variables
    [HideInInspector] public Rigidbody rb;
    public Vector3 Direction;
    [SerializeField] float speed;
    private float currentSpeed;

    public Transform myDistance;

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
    public AudioClip footStepsSfx;
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
        //all'inizio la camera � puntata davanti al player
        cameraPitch = 0f;
        playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
    private void Update()
    {
        if (GameManager.Instance.isPaused == false && GameManager.Instance.isGameOver == false && GameManager.Instance.isWinning == false && !UIManager.Instance.tutorialPanel.activeSelf)
        {
            Camera();
            Move();

            if (freezed)
            {
                freezeTimer += Time.deltaTime;

                UIManager.Instance.staminaIconTimer.fillAmount = 1f - (freezeTimer / freezeTime);

                if (freezeTimer >= freezeTime)
                {
                    freezed = false;
                }

                UIManager.Instance.staminaOutline.SetActive(false);
            }
        }

        if (currentEnergy <= 0)
        {
            GameManager.Instance.GameOver();
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
        //Movement velocity calculator (la aggiorni in Update perch� deve tornare alla normalit� una volta che non si preme lo sprint)
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
            GameManager.Instance.GameOver();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CollObj = other.gameObject;

        //se abbiamo pigliato una caramella poteri
        if (CollObj.CompareTag("PowerCandy"))
        {
            GameObject candy = other.gameObject;
            GameObject spawner = candy.transform.parent.gameObject; // il parent � lo spawner

            AudioManager.instance.PlaySfx(pickUpSfx);

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
            GameObject spawner = candy.transform.parent.gameObject; // il parent � lo spawner

            AudioManager.instance.PlaySfx(pickUpSfx);

            // la prendiamo, aggiorniamo la stamina, e la distruggiamo
            currentEnergy += addEnergy;
            if (currentEnergy > maxEnergy)
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

    public void FreezeMovement()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
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
            if (Direction.magnitude > 0.1f && !Input.GetKey(KeyCode.LeftShift) && currentEnergy > 0 && AntennaManager.Instance.antennaPiecesFound != AntennaManager.Instance.antennaPieces)
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
            if (Direction.magnitude > 0.1f && Input.GetKey(KeyCode.LeftShift) && currentEnergy > 0 && AntennaManager.Instance.antennaPiecesFound != AntennaManager.Instance.antennaPieces)
            {
                AudioManager.instance.PlaySfx(runnningStepsSfx);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}

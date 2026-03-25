using UnityEngine;

public class Movement : MonoBehaviour
{
    //Camera turning variables
    public Transform playerCamera;
    [SerializeField] float sensitivity;
    private float cameraPitch;

    //Movement variables
    Rigidbody rb;
    Vector3 Direction;
    [SerializeField] float speed;
    private float currentSpeed;

    public int maxEnergy;
    public float currentEnergy;
    public float detractEnergy;
    [SerializeField] float moveEnery;
    [SerializeField] float sprintEnergy;
    public float stunEnergy;

    GameObject CollObj;

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
        detractEnergy = moveEnery;
    }
    private void Update()
    {
        if (!UIManager.Instance.pauseMenuUI.activeSelf)
        {
            Camera();
            Move();
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
            detractEnergy = moveEnery;
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
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            currentEnergy -= moveEnery;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CollObj = other.gameObject;
        if (CollObj.CompareTag("PowerCandy"))
            TakeCandy(1);
    }

    private void TakeCandy(int candies)
    {
        PowerCandyManager.Instance.AddCandy(1);
        Destroy(CollObj);
    }
}

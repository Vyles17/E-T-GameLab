using UnityEngine;

public class Movement : MonoBehaviour
{
    //Camera turning variables
    public Vector2 turn;
    public Transform cameraPivot;
    public float Sensitivity = 2f;
    public float rotationSmoothTime = 0.1f;
    private float cameraYaw;
    private float cameraPitch;
    private float currentVelocity;

    //Movement variables
    Rigidbody rb;
    Vector3 Direction;
    [SerializeField] float Speed = 1.0f;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        Camera();
        Move();
    }
    private void FixedUpdate()
    {
        //Movement velocity calculator
        Vector3 vel = rb.linearVelocity;
        vel.x = Direction.x * Speed * Time.fixedDeltaTime;
        vel.z = Direction.z * Speed * Time.fixedDeltaTime;

        //Sprint
        if(Input.GetKey(KeyCode.LeftShift))
        {
            vel.x *= 3;
            vel.z *= 3;
        }
        rb.linearVelocity = vel;
    }
    private void Camera()
    {
        turn.x += Input.GetAxis("Mouse X") * Sensitivity;
        turn.y += Input.GetAxis("Mouse Y") * Sensitivity;

        cameraYaw += turn.x;
        cameraPitch -= turn.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -30f, 60f);

        cameraPivot.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
    }
    private void Move()
    {
        float xmuv = Input.GetAxis("Horizontal");
        float zmuv = Input.GetAxis("Vertical");
        Direction = new Vector3(xmuv, 0, zmuv).normalized;

        if (Direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + cameraPivot.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
    }
}

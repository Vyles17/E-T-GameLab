using System;
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

    GameObject CollObj;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    }
    private void OnTriggerEnter(Collider other)
    {
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
            //LORIS AGGIUNGI QUI LA COSA CHE GLI RICARICA LA STAMINA
            Destroy(candy);

            // segnalo come appena svuotato
            spawner.GetComponent<NormalSpawner>().justEmptied = true;

            // diciamo al manager di spawnare una nuova caramella
            SpawnerManager.Instance.RespawnCandy(spawner);
        }
    }

    // scusa loris te lo commento perchè devo fare tutto da OnTriggerEnter :p

    //private void TakeCandy(int candies)
    //{
    //    PowerCandyManager.Instance.AddCandy(1);
    //    Destroy(CollObj);
    //}
}

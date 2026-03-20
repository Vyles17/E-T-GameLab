using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody rb;
    Vector3 Direction;

    [SerializeField] float Speed = 1.0f;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }
    private void Update()
    {
        float xmuv = Input.GetAxis("Horizontal");
        float zmuv = Input.GetAxis("Vertical");
        
        Direction = new Vector3(xmuv, 0, zmuv);

        Direction.Normalize();
    }
    private void FixedUpdate()
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = Direction.x * Speed * Time.fixedDeltaTime;
        vel.z = Direction.z * Speed * Time.fixedDeltaTime;

        rb.linearVelocity = vel;
        
    }
}

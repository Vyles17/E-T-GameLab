using UnityEngine;

public class SpinObject : MonoBehaviour
{
    //script da dare agli oggetti pick-uppabili
    public virtual void FixedUpdate()
    {
        Spin();
    }

    private void Spin()
    {
        transform.Rotate(Vector3.up * 90 * Time.fixedDeltaTime);
    }
}
using UnityEngine;

public class StopEthel : MonoBehaviour
{
    //public int collidedCount;
    public bool stopUiiaing = false;

    private void OnTriggerExit(Collider other)
    {
        stopUiiaing = true;
    }
}

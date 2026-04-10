using UnityEngine;

public class StopUIIAI : MonoBehaviour
{
    public int collidedCount;
    public bool stopUiiaing = false;

    private void OnTriggerExit(Collider other)
    {
        collidedCount++;
        if (collidedCount == 2)
        {
            stopUiiaing = true;
            collidedCount = 0;
        }
    }
}

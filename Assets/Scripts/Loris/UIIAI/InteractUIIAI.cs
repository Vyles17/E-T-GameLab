using UnityEngine;

public class InteractUIIAI : MonoBehaviour
{
    public bool stopUiiaing = true;
    private void OnTriggerEnter(Collider other)
    {
        stopUiiaing = false;
    }
    private void OnTriggerExit(Collider other)
    {
        stopUiiaing = true;
    }
}

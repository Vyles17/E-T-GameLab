using UnityEngine;

public interface IInteractable

    //se era pensata solo per far si che gli oggetti erano interagibili e nient'altro allora togliere (sostituita con IPointerClickHandler)
{
    void OnInteraction();
}

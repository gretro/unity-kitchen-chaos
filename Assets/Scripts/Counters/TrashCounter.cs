using Assets.Scripts;
using UnityEngine;

public class TrashCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    [Header("Events")]
    [SerializeField] private EventQueue eventQueue;

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        return true;
    }

    public bool HoldsObject()
    {
        return false;
    }

    public void Interact(Player source)
    {
        if (source.HoldsObject())
        {
            source.TransferObject(this);
        }
    }

    public void AlternateInteract(Player source) { }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        GameObject.Destroy(kitchenObject.gameObject);
        eventQueue.DispatchEvent(EventQueue.OnItemTrashed, this.gameObject, kitchenObject.ObjectType);
    }
}

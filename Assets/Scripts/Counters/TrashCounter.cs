using Assets.Scripts;
using UnityEngine;

public class TrashCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        return true;
    }

    public KitchenObject GetHeldObject()
    {
        return null;
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
    }
}

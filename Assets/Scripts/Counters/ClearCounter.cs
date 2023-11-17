using Assets.Scripts;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    [SerializeField]
    private Transform hookPoint;

    private KitchenObject heldObject = null;

    public void Interact(Player source)
    {
        Debug.Log("ClearCounter.Interact() called", source);

        if (this.HoldsObject())
        {
            Debug.Log("ClearCounter holds an object");
            var canTransfer = source.CanReceiveObject(this.heldObject);

            if (canTransfer)
            {
                source.ReceiveObject(this.heldObject);
                this.heldObject = null;
            }
            else
            {
                Debug.Log("Counter already holds an object and cannot receive another");
            }
        }
        else if (source.HoldsObject())
        {
            Debug.Log("Player holds an object");

            source.TransferObject(this);
        }
    }

    public void AlternateInteract(Player source) { }

    public KitchenObject GetHeldObject()
    {
        return this.heldObject;
    }

    public bool HoldsObject()
    {
        return this.heldObject != null;
    }

    public bool CanReceiveObject(KitchenObject _)
    {
        return !HoldsObject();
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject(kitchenObject))
        {
            Debug.LogError("ClearCounter already holds an object!");
            return;
        }

        this.heldObject = kitchenObject;
        kitchenObject.transform.SetParent(hookPoint);
        kitchenObject.transform.localPosition = Vector3.zero;
    }
}

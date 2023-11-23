using Assets.Scripts;
using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    [SerializeField]
    private Transform hookPoint;

    [Header("Events")]
    [SerializeField] private EventQueue eventQueue;

    private KitchenObject heldObject = null;

    public void Interact(Player source)
    {
        if (this.HoldsObject())
        {
            var canTransfer = source.CanReceiveObject(this.heldObject);

            if (canTransfer)
            {
                source.ReceiveObject(this.heldObject);
                this.heldObject = null;
            }
            else if (source.HoldsObject() && this.heldObject.TryGetComponent<IKitchenObjectHolder>(out var innerHolder))
            {
                if (innerHolder.CanReceiveObject(source.GetHeldObject()))
                {
                    source.TransferObject(innerHolder);
                }
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

    public bool HoldsObject()
    {
        return this.heldObject != null;
    }

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        if (!this.HoldsObject())
        {
            return true;
        }

        return this.heldObject.TryGetComponent<IKitchenObjectHolder>(out var innerHolder) && innerHolder.CanReceiveObject(kitchenObject);
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject(kitchenObject))
        {
            Debug.LogError("ClearCounter already holds an object!");
            return;
        }

        if (this.heldObject == null)
        {
            this.heldObject = kitchenObject;
            kitchenObject.transform.SetParent(hookPoint);
            kitchenObject.transform.localPosition = Vector3.zero;

            eventQueue.DispatchEvent(EventQueue.OnObjectDrop, this.gameObject, kitchenObject.ObjectType);
        }
        else
        {
            var innerObjectHolder = this.heldObject.GetComponent<IKitchenObjectHolder>();
            innerObjectHolder.ReceiveObject(kitchenObject);
        }
    }
}

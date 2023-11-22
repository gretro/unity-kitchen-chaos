using Assets.Scripts;
using UnityEngine;

public class DeliveryCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    [SerializeField]
    private DeliveryManager deliveryManager;

    public void Interact(Player source)
    {
        if (source.HoldsObject())
        {
            source.TransferObject(this);
        }
    }

    public void AlternateInteract(Player source) { }

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        return kitchenObject != null && kitchenObject.TryGetComponent<ServingPlate>(out var _);
    }

    public bool HoldsObject()
    {
        return false;
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject(kitchenObject))
        {
            return;
        }

        var servingPlate = kitchenObject.GetComponent<ServingPlate>();
        deliveryManager.DeliverOrder(servingPlate.GetIngredients());

        GameObject.Destroy(kitchenObject.gameObject);
    }
}

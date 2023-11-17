public interface IKitchenObjectHolder
{
    bool HoldsObject();

    KitchenObject GetHeldObject();

    bool CanReceiveObject(KitchenObject kitchenObject);

    void ReceiveObject(KitchenObject kitchenObject);
}

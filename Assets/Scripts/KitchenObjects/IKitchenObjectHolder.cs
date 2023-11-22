public interface IKitchenObjectHolder
{
    bool HoldsObject();

    bool CanReceiveObject(KitchenObject kitchenObject);

    void ReceiveObject(KitchenObject kitchenObject);
}

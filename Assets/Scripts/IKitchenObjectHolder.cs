using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectHolder
{
    bool HoldsObject();

    KitchenObject GetHeldObject();

    bool CanReceiveObject();

    void ReceiveObject(KitchenObject kitchenObject);
}

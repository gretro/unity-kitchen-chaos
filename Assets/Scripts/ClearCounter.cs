using Assets.Scripts;
using UnityEngine;

public partial class ClearCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    [SerializeField]
    private KitchenObjectSO toSpawn;

    [SerializeField]
    private Transform hookPoint;

    private KitchenObject heldObject;

    public void Interact(Player source)
    {
        if (this.HoldsObject())
        {
            if (!source.CanReceiveObject())
            {
                Debug.Log("Counter already holds an object and cannot receive another");
            }

            if (source.CanReceiveObject())
            {
                source.ReceiveObject(this.heldObject);
                this.heldObject = null;
            }
        } else
        {
            if (source.HoldsObject())
            {
                source.TransferObject(this);
            } else
            {
                this.SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        if (this.heldObject != null)
        {
            return;
        }

        var spawnedObj = Instantiate(toSpawn.prefab, hookPoint);
        if (spawnedObj.TryGetComponent(out KitchenObject kitchenObject))
        {
            kitchenObject.ObjectType = toSpawn;
            this.heldObject = kitchenObject;
        }

        spawnedObj.transform.localPosition = Vector3.zero;
    }

    public KitchenObject GetHeldObject()
    {
        return this.heldObject;
    }

    public bool HoldsObject()
    {
        return this.heldObject != null;
    }

    public bool CanReceiveObject()
    {
        return !HoldsObject();
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject())
        {
            Debug.LogError("ClearCounter already holds an object!");
            return;
        }

        this.heldObject = kitchenObject;
        kitchenObject.transform.SetParent(hookPoint);
        kitchenObject.transform.localPosition = Vector3.zero;
    }
}

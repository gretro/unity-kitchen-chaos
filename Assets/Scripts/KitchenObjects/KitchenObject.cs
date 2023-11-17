using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    public KitchenObjectSO ObjectType { get; set; }

    public static KitchenObject Spawn(KitchenObjectSO objectType, Transform parent)
    {
        var spawnedObj = GameObject.Instantiate(objectType.prefab, parent).GetComponent<KitchenObject>();
        spawnedObj.ObjectType = objectType;

        return spawnedObj;
    }
}

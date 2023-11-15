using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField]
    private KitchenObjectSO toSpawn;

    [SerializeField]
    private Transform spawnPoint;

    public void Interact()
    {
        var spawnedObj = GameObject.Instantiate(toSpawn.prefab, spawnPoint);
        spawnedObj.transform.localPosition = Vector3.zero;
    }
}

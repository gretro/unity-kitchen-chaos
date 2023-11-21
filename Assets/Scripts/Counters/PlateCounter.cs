using Assets.Scripts;
using UnityEngine;

public class PlateCounter : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject[] plates;

    [SerializeField]
    private float respawnTimeInSeconds = 10f;

    [SerializeField]
    private KitchenObjectSO plate;

    private int availablePlates = 0;
    private float respawnTimer = 0f;

    private void Start()
    {
        availablePlates = plates.Length;
    }

    private void Update()
    {
        if (availablePlates == plates.Length)
        {
            return;
        }

        respawnTimer += Time.deltaTime;

        if (respawnTimer >= respawnTimeInSeconds)
        {
            plates[availablePlates].SetActive(true);
            availablePlates++;

            respawnTimer = 0f;
        }
    }

    public void Interact(Player source)
    {
        if (availablePlates == 0)
        {
            Debug.Log("No plates available");
            return;
        }

        if (source.HoldsObject())
        {
            Debug.Log("Player already holds an object");
            return;
        }

        availablePlates--;
        plates[availablePlates].SetActive(false);

        var plate = KitchenObject.Spawn(this.plate, transform);
        source.ReceiveObject(plate);

        respawnTimer = 0f;
    }

    public void AlternateInteract(Player source) { }
}

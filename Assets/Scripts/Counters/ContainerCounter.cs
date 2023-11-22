using Assets.Scripts;
using UnityEngine;

public class ContainerCounter : MonoBehaviour, IInteractable
{
    private const string ANIMATION_OPEN_CLOSE = "OpenClose";

    [Header("Dispenser")]
    [SerializeField]
    private KitchenObjectSO dispensed;

    [Header("Visuals")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (dispensed == null)
        {
            Debug.LogError($"No dispensed object set on ContainerCounter {gameObject.name}");
            return;
        }

        spriteRenderer.sprite = dispensed.sprite;
    }

    public void Interact(Player source)
    {
        var dispensedObject = KitchenObject.Spawn(dispensed, null);

        if (!source.CanReceiveObject(dispensedObject))
        {
            Debug.Log("Player already holds an item and cannot receive another");

            GameObject.Destroy(dispensedObject.gameObject);
            return;
        }

        animator.SetTrigger(ANIMATION_OPEN_CLOSE);
        source.ReceiveObject(dispensedObject);
    }

    public void AlternateInteract(Player source) { }
}

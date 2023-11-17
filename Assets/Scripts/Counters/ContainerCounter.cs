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
        // In the case of the player, we never check the nature of the KitchenObject. We avoid an instanciation of the object if we don't need it.
        if (!source.CanReceiveObject(null))
        {
            Debug.Log("Player already holds an item and cannot receive another");
            return;
        }

        animator.SetTrigger(ANIMATION_OPEN_CLOSE);

        var dispensedObject = KitchenObject.Spawn(dispensed, null);
        source.ReceiveObject(dispensedObject);
    }

    public void AlternateInteract(Player source) { }
}

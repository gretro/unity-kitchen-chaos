using Assets.Scripts;
using UnityEngine;

public class CuttingCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    private static string CUT_TRIGGER = "Cut";

    [SerializeField]
    private Transform hookPoint;

    [SerializeField]
    private ProgressBarController progressBar;

    [SerializeField]
    private Animator animator;

    private KitchenObject heldObject;
    private int cutProgress = 0;

    private void Start()
    {
        this.progressBar.Hide();
    }

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        if (this.HoldsObject())
        {
            return false;
        }

        if (kitchenObject.TryGetComponent<Cuttable>(out var cuttable))
        {
            return !cuttable.IsCut;
        }

        return false;
    }

    public KitchenObject GetHeldObject()
    {
        return this.heldObject;
    }

    public bool HoldsObject()
    {
        return this.heldObject != null;
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject(kitchenObject))
        {
            Debug.LogWarning($"CuttingCounter is unable to receive object {kitchenObject.name}");
            return;
        }

        this.heldObject = kitchenObject;
        kitchenObject.transform.SetParent(hookPoint);
        kitchenObject.transform.localPosition = Vector3.zero;

        this.progressBar.Show();
    }

    public void Interact(Player source)
    {
        if (this.HoldsObject())
        {
            if (source.CanReceiveObject(this.heldObject))
            {
                source.ReceiveObject(this.heldObject);
                this.heldObject = null;
                this.cutProgress = 0;
                this.progressBar.Reset();
                this.progressBar.Hide();
            }
            else
            {
                Debug.Log("Player already holds an item and cannot receive another");
            }
        }
        else
        {
            if (source.HoldsObject())
            {
                source.TransferObject(this);
            }
        }
    }

    public void AlternateInteract(Player source)
    {
        if (!this.HoldsObject())
        {
            return;
        }

        var cuttable = heldObject.GetComponent<Cuttable>();
        if (cuttable.IsCut)
        {
            return;
        }

        this.animator.SetTrigger(CUT_TRIGGER);

        cutProgress++;
        progressBar.UpdateProgress(cutProgress, cuttable.GetRecipe().NumberOfCuts);

        if (cutProgress >= cuttable.GetRecipe().NumberOfCuts)
        {
            cuttable.Cut();
            this.progressBar.Hide();
        }
    }
}

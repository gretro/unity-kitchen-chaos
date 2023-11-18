using Assets.Scripts;
using UnityEngine;

public class StoveCounter : MonoBehaviour, IKitchenObjectHolder, IInteractable
{
    [SerializeField]
    private Transform holdPoint;

    [SerializeField]
    private GameObject[] cookingElements;

    [SerializeField]
    private ProgressBarController progressBar;

    private KitchenObject heldObject;

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        return !this.HoldsObject() && kitchenObject.TryGetComponent<Fryable>(out var _);
    }

    public KitchenObject GetHeldObject()
    {
        return this.heldObject;
    }

    public bool HoldsObject()
    {
        return heldObject != null;
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!this.CanReceiveObject(kitchenObject))
        {
            Debug.LogError("Cannot receive object");
        }

        this.heldObject = kitchenObject;
        this.heldObject.transform.SetParent(this.holdPoint);
        this.heldObject.transform.localPosition = Vector3.zero;

        var heldFryableObject = this.heldObject.GetComponent<Fryable>();
        heldFryableObject.StartCooking();
        heldFryableObject.FryableUpdated += this.OnFryableUpdated;

        this.EnableCookingElements();

        if (heldFryableObject.State != Fryable.FryState.Burned)
        {
            this.progressBar.Show();
        }
    }

    private void OnFryableUpdated(object sender, Fryable.FryableUpdatedEventArgs e)
    {
        if (e.State == Fryable.FryState.Burned)
        {
            this.progressBar.Reset();
            this.progressBar.Hide();

            var fryable = sender as Fryable;
            fryable.FryableUpdated -= this.OnFryableUpdated;
        }

        if (e.MaxProgress == 0f)
        {
            this.progressBar.UpdateProgress(1f, 1f);
        }
        else
        {
            this.progressBar.UpdateProgress(e.CurrentProgress, e.MaxProgress);
        }
    }

    private void EnableCookingElements()
    {
        foreach (var cookingElement in this.cookingElements)
        {
            cookingElement.SetActive(true);
        }
    }

    private void DisableCookingElements()
    {
        foreach (var cookingElement in this.cookingElements)
        {
            cookingElement.SetActive(false);
        }
    }

    public void Interact(Player source)
    {
        if (this.HoldsObject())
        {
            if (source.CanReceiveObject(this.heldObject))
            {
                source.ReceiveObject(this.heldObject);
                this.DisableCookingElements();

                var fryable = this.heldObject.GetComponent<Fryable>();
                fryable.StopCooking();
                fryable.FryableUpdated -= this.OnFryableUpdated;
                this.heldObject = null;

                this.progressBar.Reset();
                this.progressBar.Hide();
            }
            else
            {
                Debug.Log("Player cannot receive object");
            }
        }
        else
        {
            if (source.HoldsObject())
            {
                source.TransferObject(this);
            }
            else
            {
                Debug.Log("Player has no object for the Stove counter");
            }
        }
    }

    public void AlternateInteract(Player source) { }
}
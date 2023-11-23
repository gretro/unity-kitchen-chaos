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

    [Header("Events")]
    [SerializeField] private EventQueue eventQueue;

    private KitchenObject heldObject;
    private Fryable.FryState? lastFryState;

    private void Start()
    {
        this.progressBar.Hide();
    }

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        return !this.HoldsObject() && kitchenObject.TryGetComponent<Fryable>(out var _);
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
        this.lastFryState = null;
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
        if (e.State != lastFryState)
        {
            lastFryState = e.State;

            if (e.State == Fryable.FryState.Cooked)
            {
                this.eventQueue.DispatchEvent(EventQueue.OnStoveCooked, this.gameObject, heldObject.ObjectType);
            }
            else if (e.State == Fryable.FryState.Burned)
            {
                this.progressBar.Reset();
                this.progressBar.Hide();

                this.eventQueue.DispatchEvent(EventQueue.OnStoveBurned, this.gameObject, heldObject.ObjectType);

                var fryable = sender as Fryable;
                fryable.FryableUpdated -= this.OnFryableUpdated;
            }
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

        this.eventQueue.DispatchEvent(EventQueue.OnStoveStarted, this.gameObject);
    }

    private void DisableCookingElements()
    {
        foreach (var cookingElement in this.cookingElements)
        {
            cookingElement.SetActive(false);
        }

        this.eventQueue.DispatchEvent(EventQueue.OnStoveStopped, this.gameObject);
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
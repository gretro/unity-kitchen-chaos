using Assets.Scripts;
using Cinemachine.Utility;
using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectHolder
{
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 7f;
    [SerializeField]
    private float rotateSpeed = 10f;

    [Header("Controls")]
    [SerializeField]
    private GameInput gameInput;

    [Header("Interactions")]
    [SerializeField]
    private LayerMask countersLayerMask;
    [SerializeField]
    private Transform hookPoint;

    private float playerRadius = 0.7f;
    private float playerHeight = 2f;
    private float interactionDistance = 2f;

    public bool IsWalking { get; private set; }

    private IInteractable selectedInteractable;
    public IInteractable SelectedInteractable
    {
        get
        {
            return selectedInteractable;
        }
        private set
        {
            if (value != selectedInteractable)
            {
                selectedInteractable?.Unselect();
                
                selectedInteractable = value;
                selectedInteractable?.Select();
            }
        }
    }

    private KitchenObject heldObject;

    private void Start()
    {
        gameInput.OnInteractAction += OnInteractAction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement()
    {
        var moveVector = gameInput.GetMovementVectorNormalized();
        var moveDir = new Vector3(moveVector.x, 0, moveVector.y);

        var moveDistance = this.moveSpeed * Time.deltaTime;

        var isBlocked = Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance
        );

        if (isBlocked)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            if (moveVector.Abs() != Vector2.right)
            {
                var moveDirX = new Vector3(moveDir.x, 0, 0).normalized;

                isBlocked = Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirX,
                    moveDistance
                );

                // We can move on the X axis
                if (!isBlocked)
                {
                    transform.position += moveDirX * moveDistance;
                }
            }

            if (moveVector.Abs() != Vector2.up)
            {
                var moveDirZ = new Vector3(0, 0, moveDir.z).normalized;

                isBlocked = Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ,
                    moveDistance
                );

                // We can move on the Y axis
                if (!isBlocked)
                {
                    transform.position += moveDirZ * moveDistance;
                }
            }

        }
        else
        {
            transform.position += moveDir * moveDistance;
        }

        IsWalking = moveVector != Vector2.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void HandleInteractions()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
            {
                SelectedInteractable = interactable;
            } 
            else
            {
                SelectedInteractable = null;
            }
        } 
        else
        {
            SelectedInteractable = null;
        }
        
    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (SelectedInteractable != null)
        {
            SelectedInteractable.Interact(this);
        }
    }

    public bool HoldsObject()
    {
        return this.heldObject != null;
    }

    public KitchenObject GetHeldObject()
    {
        return this.heldObject;
    }

    public bool CanReceiveObject()
    {
        return !this.HoldsObject();
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject())
        {
            Debug.LogError("Player already holds an object!");
            return;
        }

        this.heldObject = kitchenObject;
        kitchenObject.transform.SetParent(hookPoint);
        kitchenObject.transform.localPosition = Vector3.zero;
    }

    public void TransferObject(IKitchenObjectHolder target)
    {
        if (!this.HoldsObject())
        {
            Debug.LogError("Player does not hold an object!");
            return;
        }

        if (!target.CanReceiveObject()) {             
            Debug.LogError("Target already holds an object!");
            return;
        }

        target.ReceiveObject(this.heldObject);
        this.heldObject = null;
    }
}

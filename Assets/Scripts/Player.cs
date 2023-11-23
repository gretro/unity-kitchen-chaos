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

    [Header("Events")]
    [SerializeField] private EventQueue eventQueue;

    private float playerRadius = 0.7f;
    private float playerHeight = 2f;
    private float interactionDistance = 2f;

    public bool IsWalking { get; private set; }

    private Selectable selection;

    public Selectable Selection
    {
        get
        {
            return selection;
        }
        private set
        {
            if (value != selection)
            {
                selection?.Unselect();

                selection = value;
                selection?.Select();
            }
        }
    }

    private KitchenObject heldObject;

    private void Start()
    {
        gameInput.OnInteractAction += OnInteractAction;
        gameInput.OnAlternateInteractAction += OnAlternateInteractAction;
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
            if (raycastHit.collider.gameObject.TryGetComponent<Selectable>(out var selectable))
            {
                Selection = selectable;
            }
            else
            {
                Selection = null;
            }
        }
        else
        {
            Selection = null;
        }

    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (Selection != null && Selection.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact(this);
        }
    }

    private void OnAlternateInteractAction(object sender, EventArgs e)
    {
        if (Selection != null && Selection.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.AlternateInteract(this);
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

    public bool CanReceiveObject(KitchenObject kitchenObject)
    {
        if (!this.HoldsObject())
        {
            return true;
        }

        if (kitchenObject.TryGetComponent<IKitchenObjectHolder>(out var receivingHolder) && receivingHolder.CanReceiveObject(this.heldObject))
        {
            return true;
        }

        return this.heldObject.TryGetComponent<IKitchenObjectHolder>(out var innerHolder) && innerHolder.CanReceiveObject(kitchenObject);
    }

    public void ReceiveObject(KitchenObject kitchenObject)
    {
        if (!CanReceiveObject(kitchenObject))
        {
            Debug.LogError($"Player cannot receive object {kitchenObject.name}");
            return;
        }

        if (this.heldObject == null)
        {
            this.heldObject = kitchenObject;
            kitchenObject.transform.SetParent(hookPoint);
            kitchenObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            if (this.heldObject.TryGetComponent<IKitchenObjectHolder>(out var innerHolder))
            {
                innerHolder.ReceiveObject(kitchenObject);
            }
            else if (kitchenObject.TryGetComponent<IKitchenObjectHolder>(out var receivedHolder))
            {
                receivedHolder.ReceiveObject(this.heldObject);

                this.heldObject = kitchenObject;
                kitchenObject.transform.SetParent(hookPoint);
                kitchenObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError($"Player cannot receive object {kitchenObject.name}");
            }
        }

        eventQueue.DispatchEvent(EventQueue.OnObjectPickup, this.gameObject, kitchenObject.ObjectType);
    }

    public void TransferObject(IKitchenObjectHolder target)
    {
        if (!this.HoldsObject())
        {
            Debug.Log("Player does not hold an object!");
            return;
        }

        if (!target.CanReceiveObject(this.heldObject))
        {
            Debug.Log($"Target is unable to receive the object {this.heldObject.name}");
            return;
        }

        target.ReceiveObject(this.heldObject);
        this.heldObject = null;
    }
}

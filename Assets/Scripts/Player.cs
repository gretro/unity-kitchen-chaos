using Cinemachine.Utility;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter SelectedCounter { get; set; }
    }

    [SerializeField]
    private float moveSpeed = 7f;

    [SerializeField]
    private float rotateSpeed = 10f;

    [SerializeField]
    private GameInput gameInput;

    [SerializeField]
    private LayerMask countersLayerMask;

    private float playerRadius = 0.7f;
    private float playerHeight = 2f;
    private float interactionDistance = 2f;

    public bool IsWalking { get; private set; }

    private ClearCounter selectedCounter;
    public ClearCounter SelectedCounter
    {
        get
        {
            return selectedCounter;
        }
        private set
        {
            selectedCounter = value;
            if (OnSelectedCounterChanged != null)
            {
                var evnt = new OnSelectedCounterChangedEventArgs()
                {
                    SelectedCounter = value,
                };
                OnSelectedCounterChanged.Invoke(this, evnt);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Player already exists!");

            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
            if (raycastHit.collider.gameObject.TryGetComponent<ClearCounter>(out var clearCounter))
            {
                if (SelectedCounter != clearCounter)
                {
                    SelectedCounter = clearCounter;
                }
            } 
            else
            {
                SelectedCounter = null;
            }
        } 
        else
        {
            SelectedCounter = null;
        }
        
    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (SelectedCounter != null)
        {
            SelectedCounter.Interact();
        }
    }
}
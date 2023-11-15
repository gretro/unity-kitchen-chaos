using Cinemachine.Utility;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 7f;

    [SerializeField]
    private float rotateSpeed = 10f;

    [SerializeField]
    private GameInput gameInput;

    private float playerRadius = 0.7f;
    private float playerHeight = 2f;

    public bool IsWalking { get; private set; }
    
    private void Update()
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

        } else
        {
            transform.position += moveDir * moveDistance;
        }

        IsWalking = moveVector != Vector2.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
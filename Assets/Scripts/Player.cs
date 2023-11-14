using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 7f;

    [SerializeField]
    private float rotateSpeed = 10f;

    [SerializeField]
    private GameInput gameInput;

    public bool IsWalking { get; private set; }
    
    private void Update()
    {
        var moveVector = gameInput.GetMovementVectorNormalized();

        IsWalking = moveVector != Vector2.zero;

        var moveDir = new Vector3(moveVector.x, 0, moveVector.y);
        transform.position += moveDir * (this.moveSpeed * Time.deltaTime);
        
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
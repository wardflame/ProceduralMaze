using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.0f;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;
    [SerializeField]
    private float smoothInputSpeed;

    [SerializeField, Tooltip("Mask for look raycast to hit.")]
    private LayerMask environmentMask;
    
    private Vector3 lookPos;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private Rigidbody playerRigidbody;



    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

    }

    private void Update()
    {
        LookAtMouse();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LookAtMouse()
    {
        Vector2 mouseVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);

        if (Physics.Raycast(ray, out var hit, 100, environmentMask))
        {
            lookPos = hit.point;
            Vector3 lookDir = lookPos - transform.position;
            lookDir.y = 0;
            transform.forward = lookDir;
        }
    }

    private void Move()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 moveVector = new Vector3(currentInputVector.x, 0, currentInputVector.y);

        playerRigidbody.MovePosition(transform.position + moveVector * moveSpeed * Time.fixedDeltaTime);
    }
}

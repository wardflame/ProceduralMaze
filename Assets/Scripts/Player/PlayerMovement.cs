using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask environmentMask;
    private Vector3 lookPos;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private Rigidbody playerRigidbody;

    public float moveSpeed = 1f;

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
        Vector3 moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        playerRigidbody.MovePosition(transform.position + moveVector * moveSpeed * Time.fixedDeltaTime);
    }
}

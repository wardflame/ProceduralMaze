using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;

    private Rigidbody playerRigidbody;

    #region Input/look smoothing fields
    // For movement input smoothing.
    [SerializeField]
    private float smoothInputSpeed;
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    // For player look smoothing.
    [SerializeField]
    private float smoothLookSpeed;
    private Vector3 currentLookVector;
    private Vector3 smoothLookVelocity;
    #endregion

    #region Player cursor look fields
    [SerializeField, Tooltip("Mask for player look raycast to hit.")]
    private LayerMask lookRaycastMask;
    private Vector3 lookPos;
    #endregion

    #region Input system
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    #endregion

    #region Animation fields
    public Animator animator;
    #endregion

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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

    #region Movement
    private void LookAtMouse()
    {
        Vector2 mouseVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);

        if (Physics.Raycast(ray, out var hit, 100, lookRaycastMask))
        {
            lookPos = hit.point;
            Vector3 lookDir = lookPos - transform.position;
            currentLookVector = Vector3.SmoothDamp(currentLookVector, lookDir, ref smoothLookVelocity, smoothLookSpeed);
            currentLookVector.y = 0;
            transform.forward = currentLookVector;
        }
    }

    private void Move()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 moveVector = new Vector3(currentInputVector.x, 0, currentInputVector.y);

        playerRigidbody.MovePosition(transform.position + moveVector * moveSpeed * Time.fixedDeltaTime);
    }
    #endregion

    #region Animation



    #endregion
}

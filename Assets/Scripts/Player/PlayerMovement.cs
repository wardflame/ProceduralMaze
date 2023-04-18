using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    #region The Cache
    private Camera mainCamera;
    #endregion

    #region Controller fields
    [SerializeField]
    private float moveSpeed = 5;
    private CharacterController charControl;
    #endregion

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
    private Vector3 lookDirection;
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
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();
        charControl = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void Update()
    {
        LookAtMouse();
        AnimatePlayer();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Movement
    private void LookAtMouse()
    {
        Vector3 mouseVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mouseVector);

        if (Physics.Raycast(ray, out var hit, 100, lookRaycastMask))
        {
            lookPos = hit.point;
            lookDirection = lookPos - transform.position;
            currentLookVector = Vector3.SmoothDamp(currentLookVector, lookDirection, ref smoothLookVelocity, smoothLookSpeed);
            currentLookVector.y = 0;
            currentLookVector.Normalize();
            transform.forward = currentLookVector;
        }        
    }

    private void Move()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 moveVector = new Vector3(currentInputVector.x, 0, currentInputVector.y);

        charControl.SimpleMove(moveVector * moveSpeed);
    }
    #endregion

    #region Animation
    private void AnimatePlayer()
    {
        Vector3 modifiedVector = SetMovementAngle();

        animator.SetFloat("VelocityX", modifiedVector.x);
        animator.SetFloat("VelocityZ", modifiedVector.z);
    }

    /// <summary>
    /// Adjust the animator controller's velocity on the
    /// X and Z axis to simulate the player adjusting running
    /// direction to the position of the cursor on the environment.
    /// </summary>
    /// <returns></returns>
    private Vector3 SetMovementAngle()
    {
        Vector3 inputVector = new Vector3(currentInputVector.x, 0, currentInputVector.y);
        float rotation = transform.rotation.eulerAngles.y;

        var vector = Quaternion.Euler(0, -rotation, 0) * inputVector;

        return vector;
    }
    #endregion
}

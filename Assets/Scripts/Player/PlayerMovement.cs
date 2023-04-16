using Unity.VisualScripting;
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
        animator = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
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
        Vector2 mouseVector = playerInputActions.Player.Look.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mouseVector);

        if (Physics.Raycast(ray, out var hit, 100, lookRaycastMask))
        {
            lookPos = hit.point;
            lookDirection = lookPos - transform.position;
            currentLookVector = Vector3.SmoothDamp(currentLookVector, lookDirection, ref smoothLookVelocity, smoothLookSpeed);
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
    private void AnimatePlayer()
    {
        Vector2 modifiedVector = SetMovementAngle();

        animator.SetFloat("VelocityX", modifiedVector.x);
        animator.SetFloat("VelocityZ", modifiedVector.y);
    }

    private Vector2 SetMovementAngle()
    {
        Vector2 modVector = currentInputVector;

        float currentRotation = transform.rotation.eulerAngles.y;

        Vector3 pointX, pointZ;
        pointX = transform.position;
        pointX.x += 0.1f;

        pointZ = transform.position;
        pointZ.z += 0.1f;

        Vector3 axis = Vector3.ClampMagnitude(lookDirection, 0.1f);

        float iLerpMinusX = Mathf.InverseLerp(pointX.x, pointZ.x * -1, axis.x);
        float iLerpX = Mathf.InverseLerp(pointX.x * -1, pointZ.x, axis.x);

        float iLerpMinusZ = Mathf.InverseLerp(pointZ.z, pointX.z, axis.z);
        float iLerpZ = Mathf.InverseLerp(pointZ.z * -1, pointX.z, axis.z);



        if (currentRotation >= 90 && currentRotation <= 270) modVector *= -1;

        return modVector; //new Vector2(0, 0);
    }
    #endregion
}

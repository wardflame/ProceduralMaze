using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Physics and speed fields
    [SerializeField]
    private float moveSpeed = 5;
    private Rigidbody playerRigidbody;
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

    /// <summary>
    /// Adjust the animator controller's velocity on the
    /// X and Z axis to simulate the player adjusting running
    /// direction to the position of the cursor on the environment.
    /// </summary>
    /// <returns></returns>
    private Vector2 SetMovementAngle()
    {
        Vector2 modVector = currentInputVector;
        float rot = transform.rotation.eulerAngles.y;

        Vector3 axis = Vector3.ClampMagnitude(lookDirection, 0.1f);

        float lerpX, lerpZ, axisX;
        lerpX = 0;
        lerpZ = 0;
        axisX = axis.x * 10; // Get axis.x as valid lerp t value.

        if (rot >= 180 && rot < 360) axisX *= -1;

        lerpX = Mathf.Lerp(modVector.x, modVector.y, axisX);
        lerpZ = Mathf.Lerp(modVector.y, modVector.x, axisX);

        if (rot >= 0 && rot < 180) lerpX *= -1;
        if (rot >= 90 && rot < 180) lerpZ *= -1;
        if (rot >= 180 && rot <= 270) lerpZ *= -1;

        Debug.Log(axis.x * 10);
        Debug.Log("X: " + lerpX);
        Debug.Log("Z: " + lerpZ);

        modVector.x = lerpX;
        modVector.y = lerpZ;

        return modVector; //new Vector2(0, 0);
    }
    #endregion
}

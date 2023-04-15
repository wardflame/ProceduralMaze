using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    public Animator animator;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (playerInputActions.Player.PrimaryAttack.IsPressed()) animator.SetBool("primaryAttack", true);
        else animator.SetBool("primaryAttack", false);

        if (playerInputActions.Player.SecondaryAttack.IsPressed()) animator.SetBool("secondaryAttack", true);
        else animator.SetBool("secondaryAttack", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, Player_Input.IOnFootActions
{
    private Player_Input playerInput;
    private Player_Input.OnFootActions onFootActions;

    private PlayerMotor motor;
    private PlayerLook look;

    private void Awake()
    {
        playerInput = new Player_Input();
        onFootActions = playerInput.onFoot;
        onFootActions.SetCallbacks(this);

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        look.ProcessLook(mouseDelta);
    }

    private void OnEnable()
    {
        onFootActions.Enable();
    }

    private void OnDisable()
    {
        onFootActions.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 movementInput = context.ReadValue<Vector2>();
        motor.SetMovement(movementInput);
    }
}
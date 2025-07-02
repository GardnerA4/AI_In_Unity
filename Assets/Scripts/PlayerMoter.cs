using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 input;
    public float moveSpeed = 5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Called by InputManager
    public void SetMovement(Vector2 movementInput)
    {
        input = movementInput;
    }

    private void Update()
    {
        // Convert 2D input into 3D world movement
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
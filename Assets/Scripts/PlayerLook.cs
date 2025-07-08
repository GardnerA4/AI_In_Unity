using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform cameraTransform;     // Reference to your camera
    public float sensitivity = 1f;
    public float verticalClamp = 80f;

    private float xRotation = 0f;         // Pitch (up/down)

    public void ProcessLook(Vector2 mouseDelta)
    {
        // Scale by sensitivity and deltaTime
        float mouseX = mouseDelta.x * sensitivity * 0.11f;
        float mouseY = mouseDelta.y * sensitivity * 0.11f;

        // Clamp vertical look
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // Rotate the camera (pitch)
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player (yaw)
        transform.Rotate(Vector3.up * mouseX);
    }
}

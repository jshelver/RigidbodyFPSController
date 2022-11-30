using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform playerOrientation;
    [SerializeField] Transform mainCameraTransform;

    [Header("Camera Settings")]
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float viewAngle = 85f;
    float mouseX, mouseY;
    float xRotation, yRotation;

    [Header("Wall Running Camera Settings")]
    [SerializeField] float wallRunningCameraSmoothTime = 0.25f;
    float wallRunningCameraVelocity;
    float currentCameraZAngle, targetCameraZAngle = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MovementStateManager.OnCameraZAngleChanged += HandleStateChanged;
    }

    void Update()
    {
        SetLookInput(InputManager.lookInput);
        HandleLook();
    }

    void LateUpdate()
    {
        UpdateCameraTransform();
    }

    private void SetLookInput(Vector2 lookInput)
    {
        // Get mouse inputs
        mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
    }

    private void HandleLook()
    {
        // Add input to rotation
        yRotation += mouseX;
        xRotation -= mouseY;
        // Clamp the vertical look angle
        xRotation = Mathf.Clamp(xRotation, -viewAngle, viewAngle);

        // Player model only rotates horizontally
        playerOrientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        // Camera rotates vertically because it is already being rotated horizontally with the player
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void UpdateCameraTransform()
    {
        mainCameraTransform.position = transform.position;
        mainCameraTransform.rotation = Quaternion.Euler(transform.localEulerAngles.x, playerOrientation.localEulerAngles.y, UpdateCameraZAngle());
    }

    private float UpdateCameraZAngle()
    {
        currentCameraZAngle = Mathf.SmoothDampAngle(currentCameraZAngle, targetCameraZAngle, ref wallRunningCameraVelocity, wallRunningCameraSmoothTime);
        return currentCameraZAngle;
    }

    private void HandleStateChanged(float _newCameraZAngle)
    {
        targetCameraZAngle = _newCameraZAngle;
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Script References")]
    public static PlayerControls playerControls;
    public static event Action<InputActionMap> actionMapChange;

    [Header("Game Input Variables")]
    [HideInInspector] public static Vector2 movementInput;
    [HideInInspector] public static Vector2 lookInput;
    [HideInInspector] public static bool jumpInput;
    [HideInInspector] public static bool runInput;
    [HideInInspector] public static bool crouchInput;
    [HideInInspector] public static bool testInput;

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Update()
    {
        // Storing input in variables (player action map)
        movementInput = playerControls.Player.Movement.ReadValue<Vector2>();
        lookInput = playerControls.Player.Look.ReadValue<Vector2>();
        jumpInput = playerControls.Player.Jump.triggered;
        runInput = playerControls.Player.RunToggle.IsPressed();
        crouchInput = playerControls.Player.Crouch.triggered;
        testInput = playerControls.Player.Test.triggered;
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    public static void SwitchActionMap(InputActionMap actionMap)
    {
        // If the desired action map is already enabled then return
        // if (actionMap.enabled) return;

        // Disables every action map
        playerControls.Disable();
        // Call the action map change event so scripts are aware of the change (optional)
        actionMapChange?.Invoke(actionMap);
        // Enable desired action map
        actionMap.Enable();
    }

}

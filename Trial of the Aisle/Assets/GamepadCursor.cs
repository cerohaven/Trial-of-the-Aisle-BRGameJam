using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadCursor : MonoBehaviour
{
    
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    private RectTransform canvasRectTransform;
    private Mouse CurrentMouse;

    [SerializeField]
    private float xpadding = 35f;
    [SerializeField]
    private float ypadding = 35f;

    [SerializeField] private string previousControlScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private Camera mainCamera;
    private bool previousMouseState;
    private Mouse virtualMouse;

    public RectTransform CanvasRectTransform { get => canvasRectTransform; }
    public PlayerInput GamepadPlayerInput { get => playerInput; set => playerInput = value; }
    public Mouse VirtualMouse { get => virtualMouse; }

    private void OnEnable()
    {
        if(playerInput == null)
        {
            Invoke(nameof(Initializing), 0.1f);
            return;
        }
       
    }

    private void Initializing()
    {
        mainCamera = Camera.main;
        CurrentMouse = Mouse.current;

        if (virtualMouse == null)
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        else if (!virtualMouse.added)
            InputSystem.AddDevice(virtualMouse);

        // Unpair any existing devices to avoid creating additional users
        playerInput.user.UnpairDevices();

        // Pair the virtual mouse with the current user
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        // Check if a gamepad is connected and pair it with the current user
        if (Gamepad.current != null)
        {
            InputUser.PerformPairingWithDevice(Gamepad.current, playerInput.user);
        }

        // Ensure that the keyboard and mouse are not paired to multiple users
        if (Keyboard.current != null && Mouse.current != null)
        {
            InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);
            InputUser.PerformPairingWithDevice(Mouse.current, playerInput.user);
        }

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;

        Debug.Log("ControlsChanged event attached.");
    }


    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added) InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.rightStick.ReadValue(); // Changed from leftStick to rightStick
        deltaValue *= cursorSpeed * Time.unscaledDeltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, xpadding, Screen.width - xpadding);
        newPosition.y = Mathf.Clamp(newPosition.y, ypadding, Screen.height - ypadding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
        if (previousMouseState != aButtonIsPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonIsPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode
            == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);

        cursorTransform.anchoredPosition = anchoredPosition;
    }

    public void OnControlsChanged(PlayerInput input)
    {
        if (playerInput == null) return;

        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            EnableCursor(false);
            Cursor.visible = true;
            CurrentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            EnableCursor(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, CurrentMouse.position.ReadValue());
            AnchorCursor(CurrentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }

    public void EnableCursor(bool enable)
    {
        cursorTransform.gameObject.SetActive(enable);
    }
    //potential solution to on controls changed not being called
    //private void Update() {
    //    if (playerInput == null) return;
    //    if (previousControlScheme != playerInput.currentControlScheme) {
    //        OnControlsChanged(playerInput);
    //    }
    //    previousControlScheme = playerInput.currentControlScheme;
    //}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class VirtualMouseUserInput : MonoBehaviour
{
    [SerializeField]
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

    private string previousControlScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private Camera mainCamera;
    private bool previousMouseState;
    private Mouse virtualMouse;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        CurrentMouse = Mouse.current;

        if (virtualMouse == null)
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        else if (virtualMouse.added)
            InputSystem.AddDevice(virtualMouse);

        // Pair the device to the user to use the playerinput component with the event system and the virtual mouse
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added) InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
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

    private void OnControlsChanged(PlayerInput input)
    {
        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            CurrentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, CurrentMouse.position.ReadValue());
            AnchorCursor(CurrentMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
        }
    }
}

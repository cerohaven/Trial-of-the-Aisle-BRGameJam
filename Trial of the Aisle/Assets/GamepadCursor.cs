
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
    public Mouse VirtualMouse { get => virtualMouse; set => virtualMouse = value; }

    public Mouse CursorMouse { get => CurrentMouse; set => CurrentMouse = value; }

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
            SwitchToGamepadControls();
        }

        // Ensure that the keyboard and mouse are not paired to multiple users
        if (Keyboard.current != null && Mouse.current != null)
        {
            InputUser.PerformPairingWithDevice(Keyboard.current, playerInput.user);
            InputUser.PerformPairingWithDevice(Mouse.current, playerInput.user);
        }

        //Setting the Cursor Mouse position to a certain value
        if (cursorTransform != null)
        {
            Vector2 position = new Vector2(Screen.width / 2, Screen.height / 2);

            
            CurrentMouse.WarpCursorPosition(position);
            InputState.Change(virtualMouse.position, position);
            cursorTransform.anchoredPosition = position;
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

        Vector2 navigateValue = GameManager.Instance.PlayerInputHandler.PlayerInput.actions["Navigate"].ReadValue<Vector2>();
        Vector2 deltaValue = Vector2.zero;

        if(navigateValue != Vector2.zero)
        {
            deltaValue = navigateValue;
        }


        deltaValue *= cursorSpeed * Time.unscaledDeltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, xpadding, Screen.width - xpadding);
        newPosition.y = Mathf.Clamp(newPosition.y, ypadding, Screen.height - ypadding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);


        if(GameManager.Instance.ControlScheme == ControlScheme.Gamepad)
        {
            //Reference the "Click" actions for what is considered them pressing the "Accept" button
            bool aButtonIsPressed = GameManager.Instance.PlayerInputHandler.PlayerInput.actions["Click"].WasPressedThisFrame();
            if (previousMouseState != aButtonIsPressed)
            {
                virtualMouse.CopyState<MouseState>(out var mouseState);
                mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
                InputState.Change(virtualMouse, mouseState);
                previousMouseState = aButtonIsPressed;
            }
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
            SwitchToMouseAndKeyboardControls();
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            SwitchToGamepadControls();
        }
        else
        {
            GameManager.Instance.ControlScheme = ControlScheme.None;
            Debug.LogWarning("Current Control Scheme returned None");
        }
    }

    private void SwitchToMouseAndKeyboardControls()
    {

        EnableCursor(false);
        Debug.Log("Switched to Keyboard and Mouse Controls");
        CurrentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        previousControlScheme = mouseScheme;
        GameManager.Instance.ControlScheme = ControlScheme.Mouse;
        Cursor.visible = true;
    }
    private void SwitchToGamepadControls()
    {
        EnableCursor(true);
        Debug.Log("Switched to Gamepad Controls");
        InputState.Change(virtualMouse.position, CurrentMouse.position.ReadValue());
        AnchorCursor(CurrentMouse.position.ReadValue());
        previousControlScheme = gamepadScheme;
        GameManager.Instance.ControlScheme = ControlScheme.Gamepad;
        Cursor.visible = false;
    }
    public void EnableCursor(bool enable)
    {
        cursorTransform.gameObject.SetActive(enable);
    }
}

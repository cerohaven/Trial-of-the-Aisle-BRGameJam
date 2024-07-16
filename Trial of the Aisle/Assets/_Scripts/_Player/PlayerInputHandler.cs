using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{

    [SerializeField] private PlayerInput playerInput;


    [Header("Input System")]
    [SerializeField] private InputActionAsset actionAsset; // Use an InputActionAsset instead of individual references
    private InputAction moveInput;
    private InputAction dodgeInput;
    private InputAction pauseInput;
    private InputAction unPauseInput;
    private InputAction interactInput;


    private bool dodgePressed = false;

    //properties
    public PlayerInput PlayerInput { get => playerInput; }


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        // Initialize input actions from the asset
        moveInput = actionAsset.FindAction("Move");
        dodgeInput = actionAsset.FindAction("Dodge");
        unPauseInput = actionAsset.FindAction("UnPause");
        pauseInput = actionAsset.FindAction("Pause");

        interactInput = actionAsset.FindAction("Interact");
    }

    private void OnEnable()
    {
        moveInput.Enable();
        dodgeInput.Enable();
        pauseInput.Enable();
        unPauseInput.Enable();
        interactInput.Enable();

        dodgeInput.started += OnDodge;
        pauseInput.performed += OnPause;
        unPauseInput.performed += OnUnPause;
    }

    private void OnDisable()
    {
        moveInput.Disable();
        dodgeInput.Disable();
        pauseInput.Disable();
        unPauseInput.Disable();
        interactInput.Disable();

        dodgeInput.started -= OnDodge;
        pauseInput.performed -= OnPause;
        unPauseInput.performed -= OnUnPause;
    }

    public Vector2 ReadMovementValue()
    {
        return moveInput.ReadValue<Vector2>();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        //Send an event where we 
        GameManager.Instance.EventSender.DodgeEventSender();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.CanMove) return;

        GameManager.Instance.EventSender.PauseGameEventSend();

    }

    private void OnUnPause(InputAction.CallbackContext context)
    {

        if (playerInput.currentActionMap.name != "UI") return;

        GameManager.Instance.EventSender.ResumeGameEventSend();
    }


    public void SwitchActionMap(bool _menu)
    {
        //a list of the action maps available
        if (_menu)
            PlayerInput.SwitchCurrentActionMap("UI");
        else
            PlayerInput.SwitchCurrentActionMap("Player");
    }

    public void ChangeControlScheme(PlayerInput p)
    {
        //Sends an event to all the interactable objects to update their sprite and text based on control
        GameManager.Instance.EventSender.ChangedControlSchemeEventSend(p.currentControlScheme);
    }

    public string GetCurrentControlScheme()
    {
        return PlayerInput.currentControlScheme;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance;

    //References
    //[SerializeField] private SO_InteractableObject SO_interactableObjetSender;
    [SerializeField] private SO_PauseMenuEventSender pauseMenuEvent;
    private PlayerInput playerInput;

    //Variables
    private Vector2 moveInput;
    private bool interactPressed;
    private bool breakGrassPressed;
    private bool freezePlayerMovement;

    //Properties
    public Vector2 m_MoveInput { get => moveInput;  }
    public bool m_InteractPressed { get => interactPressed; }
    public PlayerInput m_PlayerInput { get => playerInput; }
    public bool m_BreakGrassPressed { get => breakGrassPressed; }
    public bool m_FreezePlayerMovement { get => freezePlayerMovement; set => freezePlayerMovement = value; }

    private void Awake()
    {
        //Make this a singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        playerInput = GetComponent<PlayerInput>();


    }

    //-------------------
    // In-Game Action Map
    //-------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        //This might disrupt controlers and their OnMove for the UI though
        if (freezePlayerMovement)
            return;

        moveInput = context.ReadValue<Vector2>();
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            interactPressed = context.action.triggered;
        }
    }

    public void OnBreakGrass(InputAction.CallbackContext context)
    {
     
         breakGrassPressed = context.action.WasPressedThisFrame();
   
    }

    //---------------------------------------
    // Opened UI Action Map (for Controllers)
    //---------------------------------------

    public void OnUIMove(InputAction.CallbackContext context)
    {
       
    }
    public void OnUIConfirm(InputAction.CallbackContext context)
    {

    }
    public void OnUICancel(InputAction.CallbackContext context)
    {
        //Send an event to any currently active UI
        //if (context.started)
            //SO_interactableObjetSender.ClickedCancelButtonEventSend();
    }

    public void SwitchActionMap(bool _menu)
    {
        //a list of the action maps available
        if (_menu)
            playerInput.SwitchCurrentActionMap("Menu");
        else
            playerInput.SwitchCurrentActionMap("In-Game");


    }
    public void ChangeControlScheme(PlayerInput p)
    {
        //Sends an event to all the interactable objects to update their sprite and text based on control
        //SO_interactableObjetSender.ChangedControlSchemeEventSend(p.currentControlScheme);
    }

    public string GetCurrentControlScheme()
    {
        return playerInput.currentControlScheme;
    }


    public void OnPaused(InputAction.CallbackContext context)
    {
        //Send event to pause the game and switch input action maps 
        //NEW ONE
        if (context.performed)
            pauseMenuEvent.PauseGameEventSend();
    }

    public void OnResumeGame(InputAction.CallbackContext context)
    {
        if (context.performed)
            pauseMenuEvent.ResumeGameEventSend();

    }
}

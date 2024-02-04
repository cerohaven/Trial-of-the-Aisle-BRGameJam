using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SO_PauseMenuEventSender pauseEvent;
    [SerializeField] private SO_InteractableObject interactEvent;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera cam;
    public PlayerInput playerInput;

    [Header("Input System")]
    [SerializeField] private InputActionAsset actionAsset; // Use an InputActionAsset instead of individual references
    private InputAction moveInput;
    private InputAction dodgeInput;
    private InputAction pauseInput;
    private InputAction interactInput;

    [Header("Player Variables")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float dodgeSpeed = 10f;
    [SerializeField] private float dodgeCooldown = 2f;

    private Rigidbody2D rb;
    private Vector2 lastMoveDirection = Vector2.right;
    public bool isDodging = false;
    private float lastDodgeTime = -5f;
    private bool canMove = true;

    //properties
    public PlayerInput PlayerInput { get => playerInput; }
    public bool CanMove { get => canMove; set => canMove = value; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize input actions from the asset
        moveInput = actionAsset.FindAction("Move");
        dodgeInput = actionAsset.FindAction("Dodge");
        pauseInput = actionAsset.FindAction("Pause");
        interactInput = actionAsset.FindAction("Interact");
    }

    private void OnEnable()
    {
        moveInput.Enable();
        dodgeInput.Enable();
        pauseInput.Enable();
        interactInput.Enable();

        dodgeInput.performed += OnDodge;
        pauseInput.performed += OnPause;
    }

    private void OnDisable()
    {
        moveInput.Disable();
        dodgeInput.Disable();
        pauseInput.Disable();
        interactInput.Disable();

        dodgeInput.performed -= OnDodge;
        pauseInput.performed -= OnPause;
    }

    public void FixedUpdate()
    {
        if (!canMove) return;
        Movement();
        Animate();
    }

    public void Movement()
    {
        Vector2 movementInput = moveInput.ReadValue<Vector2>();
        if (movementInput.magnitude > 0)
        {
            lastMoveDirection = movementInput.normalized;
        }

        Vector2 movement = movementInput * (isDodging ? dodgeSpeed : moveSpeed);
        rb.velocity = movement;
    }

    public IEnumerator DodgeRoutine(Vector2 dodgeDirection)
    {
        isDodging = true;
        rb.AddForce(dodgeDirection * dodgeSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f); // Dodge duration

        rb.velocity = Vector2.zero; // Reset velocity after dodge
        isDodging = false;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!canMove || isDodging || Time.time - lastDodgeTime < dodgeCooldown) return;

        lastDodgeTime = Time.time;

        StartCoroutine(DodgeRoutine(lastMoveDirection));
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        pauseEvent.PauseGameEventSend();
    }

    private void Animate()
    {
        Vector2 movementInput = moveInput.ReadValue<Vector2>();
        animator.SetFloat("AnimMoveX", movementInput.x);
        animator.SetFloat("AnimMoveY", movementInput.y);
        animator.SetFloat("AnimMoveMagnitude", movementInput.sqrMagnitude);
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
        interactEvent.ChangedControlSchemeEventSend(p.currentControlScheme);
    }

    public string GetCurrentControlScheme()
    {
        return PlayerInput.currentControlScheme;
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Refernces 
    [SerializeField] private SO_PauseMenuEventSender pauseEvent;
    [SerializeField] private InputAction inputAction;
    [SerializeField] private SO_InteractableObject interactEvent;

    [Header("Input System")]
    [SerializeField] InputActionReference moveInput;
    [SerializeField] InputActionReference dodgeInput;
    [SerializeField] InputActionReference pauseInput;
    [SerializeField] InputActionReference interactInput;

    [Header("Player Variables")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float dodgeCooldown = 5f; // Cooldown time for dodging
    Vector2 mousePos;

    //Components
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    public Camera cam;

    private bool isDodging = false;
    private float lastDodgeTime = -5f; // Initialize to allow immediate dodge

    public PlayerInput PlayerInput { get => playerInput;}

    //Properties


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        dodgeInput.action.performed += OnDodge;
        pauseInput.action.performed += OnPause;
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        pauseEvent.PauseGameEventSend();
    }

    private void FixedUpdate()
    {
        Vector2 axis = moveInput.action.ReadValue<Vector2>();
        Vector2 movement = new Vector2(axis.x, axis.y) * (isDodging ? dodgeSpeed : moveSpeed);
        rb.velocity = movement;

        mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private void OnDestroy()
    {
        dodgeInput.action.performed -= OnDodge;
        pauseInput.action.performed -= OnPause;
    }
    
    private IEnumerator DodgeRoutine(Vector2 dodgeDirection)
    {
        isDodging = true;
        rb.velocity = dodgeDirection;

        yield return new WaitForSeconds(0.5f); // Dodge duration

        isDodging = false;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (isDodging || Time.time - lastDodgeTime < dodgeCooldown) return; // Check for cooldown

        lastDodgeTime = Time.time; // Update last dodge time


        Vector2 currentVelocity = rb.velocity;
        Vector2 dodgeDirection;

        if (currentVelocity != Vector2.zero)
        {
            dodgeDirection = currentVelocity.normalized * dodgeSpeed;
        }
        else
        {
            dodgeDirection = new Vector2(1, 0) * dodgeSpeed;
        }

        StartCoroutine(DodgeRoutine(dodgeDirection));
        Debug.Log("Dodging");
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
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera cam;
    public PlayerInput playerInput;

    [Header("Input System")]
    [SerializeField] private InputActionAsset actionAsset; // Use an InputActionAsset instead of individual references
    private InputAction moveInput;
    private InputAction dodgeInput;
    private InputAction pauseInput;
    private InputAction unPauseInput;
    private InputAction interactInput;

    [Header("Player Variables")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float dodgeSpeed = 10f;
    [SerializeField] private float dodgeCooldown = 2f;
    [SerializeField] private float dodgeTime;

    [Header("Dash Animation")]
    [SerializeField] private Transform dashTransform;
    [SerializeField] private Animator dashAnim;
    [SerializeField] private ParticleSystem dustParticles;

    [Header("Ghost Trail")]
    [SerializeField] private PlayerGhostTrail ghostTrail;

    [Header("Camera Shake")]
    [SerializeField] private ScreenShake cameraShake;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeStrength = 0.2f;

    private Rigidbody2D rb;
    private Vector2 lastMoveDirection = Vector2.right;
    public bool isDodging = false;
    private float lastDodgeTime = -5f;
    private bool canMove = true;

    //For some reason, when the player presses ESC to pause it also unpauses for the first time only, so this int will make sure to only
    //unpause when we have at least pasued once.
    private int pauseCount = 0;

    //properties
    public PlayerInput PlayerInput { get => playerInput; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float DodgeSpeed { get => dodgeSpeed; set => dodgeSpeed = value; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cameraShake = FindObjectOfType<ScreenShake>();

        // Initialize input actions from the asset
        moveInput = actionAsset.FindAction("Move");
        dodgeInput = actionAsset.FindAction("Dodge");
        unPauseInput = actionAsset.FindAction("UnPause");
        pauseInput = actionAsset.FindAction("Pause");

        interactInput = actionAsset.FindAction("Interact");

        ghostTrail = GetComponent<PlayerGhostTrail>();
    }

    private void OnEnable()
    {
        moveInput.Enable();
        dodgeInput.Enable();
        pauseInput.Enable();
        unPauseInput.Enable();
        interactInput.Enable();

        dodgeInput.performed += OnDodge;
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

        dodgeInput.performed -= OnDodge;
        pauseInput.performed -= OnPause;
        unPauseInput.performed -= OnUnPause;
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

        // Trigger camera shake
        cameraShake.Shake(shakeDuration, shakeStrength);

        if (ghostTrail != null)
        {
            for (int i = 0; i < ghostTrail.GhostNumber; i++) // Create ghosts based off ghost trail number
            {
                ghostTrail.CreateGhost();
                yield return new WaitForSeconds(0.1f); // Space out the creation of each ghost
            }
        }

        rb.AddForce(dodgeDirection * dodgeSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dodgeTime); // Dodge duration

        rb.velocity = Vector2.zero; // Reset velocity after dodge
        isDodging = false;
    }


    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!canMove || isDodging || Time.time - lastDodgeTime < dodgeCooldown) return;

        lastDodgeTime = Time.time;

        StartCoroutine(DodgeRoutine(lastMoveDirection));

        //Get the player's movement direction
        Vector2 movementDir = moveInput.ReadValue<Vector2>();
        movementDir.Normalize();

        SpriteRenderer sr = dashAnim.GetComponent<SpriteRenderer>();
        ParticleSystem pr = dustParticles.GetComponent<ParticleSystem>();
        if (movementDir.x < 0)
        {
            dustParticles.Play();
            sr.flipY = true;
        }
        else
        {
            dustParticles.Stop();
            sr.flipY = false;
        }

        movementDir *= -1;
        dashTransform.up = movementDir;
        dashAnim.Play("Base Layer.Dash", 0, 0.25f);
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        GameManager.Instance.EventSender.PauseGameEventSend();

        pauseCount++;
    }

    private void OnUnPause(InputAction.CallbackContext context)
    {
        pauseCount++;
        if (pauseCount < 3) return;

        if (playerInput.currentActionMap.name != "UI") return;

        GameManager.Instance.EventSender.ResumeGameEventSend();
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
        GameManager.Instance.EventSender.ChangedControlSchemeEventSend(p.currentControlScheme);
    }

    public string GetCurrentControlScheme()
    {
        return PlayerInput.currentControlScheme;
    }
}

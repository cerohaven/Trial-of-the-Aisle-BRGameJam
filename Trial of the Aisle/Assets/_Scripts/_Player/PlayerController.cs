using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera cam;
    private GameManager gameManager;
    private PlayerInputHandler playerInputHandler;

    [Header("Player Variables")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Camera Shake")]
    [SerializeField] public ScreenShake cameraShake;

    private Rigidbody2D rb;
    private Vector2 lastMoveDirection = Vector2.right;

    public float RegularMoveSpeed { get => regularMoveSpeed; set => regularMoveSpeed = value; }
    private float regularMoveSpeed;

    //For some reason, when the player presses ESC to pause it also unpauses for the first time only, so this int will make sure to only
    //unpause when we have at least pasued once.
    private int pauseCount = 0;


    //properties
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public Vector2 LastMoveDirection { get => lastMoveDirection; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cameraShake = FindObjectOfType<ScreenShake>();

        gameManager = GameManager.Instance;
        gameManager.PlayerTransform = transform;
        gameManager.IsInMainMenu = false;

        regularMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        gameManager.PlayerInputHandler.PlayerInput.SwitchCurrentActionMap("Player");
    }

    private void FixedUpdate()
    {
        if (!gameManager.CanMove) return;
        Movement();
        Animate();
    }

    private void Movement()
    {
        Vector2 movementInput = GameManager.Instance.PlayerInputHandler.ReadMovementValue();
        if (movementInput.magnitude > 0)
        {
            lastMoveDirection = movementInput.normalized;
        }

        Vector2 movement = movementInput * moveSpeed;
        rb.velocity = movement;
    }

    private void Animate()
    {
        Vector2 movementInput = GameManager.Instance.PlayerInputHandler.ReadMovementValue();
        animator.SetFloat("AnimMoveX", movementInput.x);
        animator.SetFloat("AnimMoveY", movementInput.y);
        animator.SetFloat("AnimMoveMagnitude", movementInput.sqrMagnitude);
    }
}

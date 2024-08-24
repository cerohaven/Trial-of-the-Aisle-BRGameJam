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
    private GameManager gameManager;
    private PlayerInputHandler playerInputHandler;

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
    private bool isDodging = false;
    private float lastDodgeTime = -5f;
    

    //For some reason, when the player presses ESC to pause it also unpauses for the first time only, so this int will make sure to only
    //unpause when we have at least pasued once.
    private int pauseCount = 0;

    //properties
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float DodgeSpeed { get => dodgeSpeed; set => dodgeSpeed = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cameraShake = FindObjectOfType<ScreenShake>();

        ghostTrail = GetComponent<PlayerGhostTrail>();

        gameManager = GameManager.Instance;
        gameManager.PlayerTransform = transform;
        gameManager.EventSender.dodgeEvent.AddListener(OnDodge);
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

        Vector2 movement = movementInput * (isDodging ? dodgeSpeed : moveSpeed);
        rb.velocity = movement;
    }

    private IEnumerator DodgeRoutine(Vector2 dodgeDirection)
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


    private void OnDodge()
    {
        if (!gameManager.CanMove || isDodging || Time.time - lastDodgeTime < dodgeCooldown) return;

        lastDodgeTime = Time.time;

        StartCoroutine(DodgeRoutine(lastMoveDirection));

        //Get the player's movement direction
        Vector2 movementDir = GameManager.Instance.PlayerInputHandler.ReadMovementValue();
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


    private void Animate()
    {
        Vector2 movementInput = GameManager.Instance.PlayerInputHandler.ReadMovementValue();
        animator.SetFloat("AnimMoveX", movementInput.x);
        animator.SetFloat("AnimMoveY", movementInput.y);
        animator.SetFloat("AnimMoveMagnitude", movementInput.sqrMagnitude);
    }

}

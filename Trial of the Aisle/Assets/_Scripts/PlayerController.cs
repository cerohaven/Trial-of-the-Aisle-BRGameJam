using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] InputActionReference moveInput;
    [SerializeField] InputActionReference dodgeInput;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float dodgeSpeed = 10f;
    [SerializeField] float dodgeCooldown = 5f; // Cooldown time for dodging
    Vector2 mousePos;

    private Rigidbody2D rb;
    public Camera cam;

    private bool isDodging = false;
    private float lastDodgeTime = -5f; // Initialize to allow immediate dodge

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        dodgeInput.action.performed += OnDodge;
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
}
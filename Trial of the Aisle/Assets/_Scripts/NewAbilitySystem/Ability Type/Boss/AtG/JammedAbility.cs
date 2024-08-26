using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SlowAbility", menuName = "Abilities/Alexander/Jammed Ability")]
public class JammedAbility : Ability
{
    public GameObject jamPrefab;
    public float jamThrowForce;

    public override void Activate(GameObject owner)
    {
        // Get the current control scheme
        ControlScheme controlScheme = GameManager.Instance.ControlScheme;

        Vector2 dir;

        if (controlScheme == ControlScheme.Gamepad)
        {
            Vector2 gamepadPosition = GameManager.Instance.GamepadCursor.VirtualMouse.position.ReadValue();
            Vector2 gamepadWorldPosition = Camera.main.ScreenToWorldPoint(gamepadPosition);
            dir = gamepadWorldPosition - (Vector2)owner.transform.position;
        }
        else
        {
            // Default to mouse position if the control scheme is not Gamepad
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dir = mouseWorldPosition - (Vector2)owner.transform.position;
        }

        dir.Normalize();
        // Instantiate the jam at the owner's position
        GameObject jam = Instantiate(jamPrefab, owner.transform.position, Quaternion.identity);

        // Apply force to the jam to throw it in the direction
        Rigidbody2D rb = jam.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(dir * jamThrowForce, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogError("Rigidbody2D component is missing from jamPrefab");
        }

        // Initialize the JamBehavior component
        JamBehaviour jamBehavior = jam.GetComponent<JamBehaviour>();
        if (jamBehavior != null)
        {
            jamBehavior.Initialize(3); // Set the max number of bounces
        }
    }
}

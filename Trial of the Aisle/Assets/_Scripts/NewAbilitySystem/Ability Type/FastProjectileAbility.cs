using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

[CreateAssetMenu(fileName = "FastProjectileAbility", menuName = "Abilities/General/Fast Projectile")]
public class FastProjectileAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed;

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

        // Instantiate the projectile at the owner's position
        GameObject projectile = Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);


        // Set the projectile's velocity towards the target position
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = dir * projectileSpeed;
        projectile.transform.up = dir;
    }
}

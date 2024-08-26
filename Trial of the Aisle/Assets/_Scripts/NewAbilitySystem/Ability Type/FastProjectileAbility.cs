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

        Vector3 dir;

        if (controlScheme == ControlScheme.Gamepad)
        {
            Vector3 gamepadPosition = GameManager.Instance.GamepadCursor.VirtualMouse.position.ReadValue();
            Vector3 gamepadWorldPosition = Camera.main.ScreenToWorldPoint(gamepadPosition);
            dir = gamepadWorldPosition - owner.transform.position;
        }
        else
        {
            // Default to mouse position if the control scheme is not Gamepad
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = owner.transform.position.z;
            dir = mouseWorldPosition - owner.transform.position;
        }
        dir.Normalize();
        // Ensure the z position is the same as the owner's position to avoid any unwanted 3D effects
        dir.z = owner.transform.position.z;

        // Instantiate the projectile at the owner's position
        GameObject projectile = Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);

        // Calculate the direction from the owner to the target position
        Vector2 direction = (dir - owner.transform.position).normalized;

        // Set the projectile's velocity towards the target position
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
        projectile.transform.up = direction;
    }
}

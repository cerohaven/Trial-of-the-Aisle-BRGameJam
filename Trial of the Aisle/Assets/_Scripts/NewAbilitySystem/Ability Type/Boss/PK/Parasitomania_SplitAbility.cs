using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

[CreateAssetMenu(fileName = "SpreadShotAbility", menuName = "Abilities/General/Spread Shot")]
public class SpreadShotAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float spreadAngle = 15f;

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

        dir.Normalize();; // Direction towards the target position

        // Instantiate projectiles with specified spread
        InstantiateProjectile(owner.transform.position, dir, 0); // Center projectile
        InstantiateProjectile(owner.transform.position, dir, -spreadAngle); // Left projectile
        InstantiateProjectile(owner.transform.position, dir, spreadAngle); // Right projectile
    }

    private void InstantiateProjectile(Vector3 position, Vector2 direction, float angleOffset)
    {
        // Calculate the rotation with the angle offset
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);

        // Calculate the new direction with the angle offset
        Vector2 newDirection = Quaternion.Euler(0, 0, angleOffset) * direction;

        // Set the projectile's velocity
        projectile.GetComponent<Rigidbody2D>().velocity = newDirection * projectileSpeed;

        // Ensure the projectile is oriented correctly
        projectile.transform.up = newDirection;
    }
}

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
        dir.z = owner.transform.position.z; // Aligns z-axis with the owner
        Vector2 direction = (dir - owner.transform.position).normalized; // Direction towards the target position

        // Instantiate projectiles with specified spread
        InstantiateProjectile(owner.transform.position, direction, 0); // Center projectile
        InstantiateProjectile(owner.transform.position, direction, -spreadAngle); // Left projectile
        InstantiateProjectile(owner.transform.position, direction, spreadAngle); // Right projectile
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

using UnityEngine;

[CreateAssetMenu(fileName = "SpreadShotAbility", menuName = "Abilities/General/Spread Shot")] // Enables creating instances in the Unity Editor.
public class SpreadShotAbility : Ability
{
    public GameObject projectilePrefab; // Prefab for projectiles to shoot.
    public float projectileSpeed; // Speed of the projectiles.
    public float spreadAngle = 15f; // Angle between each projectile.

    public override void Activate(GameObject owner) // Implements ability activation.
    {
        Debug.Log(owner);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convert mouse position to world coordinates.
        mousePosition.z = owner.transform.position.z; // Aligns z-axis with the owner.

        Vector2 direction = (mousePosition - owner.transform.position).normalized; // Direction towards the mouse position.

        // Instantiate projectiles with specified spread.
        InstantiateProjectile(owner.transform.position, direction, 0); // Center projectile.
        InstantiateProjectile(owner.transform.position, direction, -spreadAngle); // Left projectile.
        InstantiateProjectile(owner.transform.position, direction, spreadAngle); // Right projectile.
    }

    private void InstantiateProjectile(Vector3 position, Vector2 direction, float angleOffset) // Instantiate a projectile with an angle offset.
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

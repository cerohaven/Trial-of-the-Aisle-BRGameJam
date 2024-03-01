using UnityEngine;

[CreateAssetMenu(fileName = "SpreadShotAbility", menuName = "Abilities/Spread Shot")] // Enables creating instances in the Unity Editor.
public class SpreadShotAbility : Ability
{
    public GameObject projectilePrefab; // Prefab for projectiles to shoot.
    public float projectileSpeed; // Speed of the projectiles.
    public float spreadAngle = 15f; // Angle between each projectile.

    public override void Activate(GameObject owner) // Implements ability activation.
    {
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
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset); // Calculate rotation with spread.
        GameObject projectile = Instantiate(projectilePrefab, position, rotation); // Create the projectile.
        projectile.GetComponent<Rigidbody2D>().velocity = rotation * Vector2.right * projectileSpeed; // Set velocity in the direction of rotation.
    }
}

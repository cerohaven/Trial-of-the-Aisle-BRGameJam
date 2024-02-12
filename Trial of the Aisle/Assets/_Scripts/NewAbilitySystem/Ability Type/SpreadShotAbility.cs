using UnityEngine;

[CreateAssetMenu(fileName = "SpreadShotAbility", menuName = "Abilities/Spread Shot")]
public class SpreadShotAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float spreadAngle = 15f; // The angle between each projectile

    public override void Activate(GameObject owner)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = owner.transform.position.z; // Ensure the z position is the same as the owner's position

        // Calculate the direction from the owner to the mouse position
        Vector2 direction = (mousePosition - owner.transform.position).normalized;

        // Instantiate the center projectile aimed at the mouse position
        InstantiateProjectile(owner.transform.position, direction, 0);

        // Instantiate the left and right projectiles with spread aimed at the mouse position
        InstantiateProjectile(owner.transform.position, direction, -spreadAngle);
        InstantiateProjectile(owner.transform.position, direction, spreadAngle);
    }

    private void InstantiateProjectile(Vector3 position, Vector2 direction, float angleOffset)
    {
        // Create a rotation based on the direction with an added angle offset for the spread
        Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset);

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);

        // Set the projectile's velocity towards the calculated direction
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = rotation * Vector2.right * projectileSpeed; // Vector2.right is used as the forward direction
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "FastProjectileAbility", menuName = "Abilities/General/Fast Projectile")]
public class FastProjectileAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed;

    public override void Activate(GameObject owner)
    {
        // Convert mouse position to world position, taking into account the camera's position and perspective
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Ensure the z position is aligned with the game's playfield, typically the same as the owner's position to avoid 3D effects in a 2D game
        mouseWorldPosition.z = owner.transform.position.z;

        // Instantiate the projectile at the owner's position
        GameObject projectile = Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);

        // Calculate the direction vector from the owner to the mouse position
        Vector2 direction = (mouseWorldPosition - owner.transform.position).normalized;

        // Set the projectile's velocity in the calculated direction
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;

        // Rotate the projectile to face towards the mouse position
        projectile.transform.right = direction;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "FastProjectileAbility", menuName = "Abilities/General/Fast Projectile")]
public class FastProjectileAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed;

    public override void Activate(GameObject owner)
    {
        // Convert mouse position to world position
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Ensure the z position is the same as the owner's position to avoid any unwanted 3D effects
        mouseWorldPosition.z = owner.transform.position.z;

        // Instantiate the projectile at the owner's position
        GameObject projectile = Instantiate(projectilePrefab, owner.transform.position, Quaternion.identity);

        // Calculate the direction from the owner to the mouse position
        Vector2 direction = (mouseWorldPosition - owner.transform.position).normalized;

        // Set the projectile's velocity towards the mouse position
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;
        projectile.transform.up = direction;
    }
}

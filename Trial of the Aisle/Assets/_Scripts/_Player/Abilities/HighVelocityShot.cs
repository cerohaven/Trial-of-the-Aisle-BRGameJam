using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighVelocityShot : MonoBehaviour, IAbility
{
    public float Cooldown => 5f; // Example cooldown
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "High Velocity Shot";

    private Camera mainCamera;

    public void Awake()
    {
        mainCamera = Camera.main; // Cache the main camera reference
    }

    public void Activate(Transform firePoint, GameObject prefab, float force)
    {
        // Calculate direction towards the mouse cursor
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;

        // Create the explosive shot projectile
        GameObject shot = Instantiate(prefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();

        // Apply force in the direction of the mouse cursor
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public IEnumerator CooldownRoutine()
    {
        CanUse = false;
        yield return new WaitForSeconds(Cooldown);
        CanUse = true;
    }
}

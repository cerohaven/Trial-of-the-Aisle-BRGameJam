using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExplosiveShot : MonoBehaviour, IAbility
{
    public GameObject AbilityPrefab { get; private set; } // Assign in Inspector or Awake/Start
    public float AbilityForce { get; private set; } = 10f; // Example force value

    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;
    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;
    public float Cooldown => 10f; // Example cooldown for the explosive shot
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "Explosive Shot";

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main; 
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

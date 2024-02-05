using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighVelocityShot : MonoBehaviour, IAbility
{
    public float AbilityForce { get; private set; } = 20f; // Example force value
    public event Action<float, float> OnCooldownChanged;

    [SerializeField] private GameObject abilityPrefab;
    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;
    public GameObject AbilityPrefab => abilityPrefab;
    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;
    public float Cooldown => 5f; // Example cooldown
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "GrapesOfWrath";

    private Camera mainCamera;

    public void Awake()
    {
        mainCamera = Camera.main; // Cache the main camera reference
    }

    public void Activate(Transform firePoint, GameObject prefab, float force)
    {
        StartCoroutine(CooldownRoutine());
        if (!CanUse)
        {
            Debug.Log($"{AbilityName} cannot be used due to cooldown or other conditions.");
            return;
        }

        Debug.Log($"Activating {AbilityName}");

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
        float cooldownTimer = Cooldown;
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            OnCooldownChanged?.Invoke(cooldownTimer, Cooldown); // Notify subscribers about cooldown progress
            yield return null;
        }
        CanUse = true;
        OnCooldownChanged?.Invoke(cooldownTimer, Cooldown); // Notify subscribers that cooldown is complete
    }

}

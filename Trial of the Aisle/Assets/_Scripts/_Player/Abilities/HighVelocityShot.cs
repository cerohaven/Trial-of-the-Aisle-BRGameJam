using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HighVelocityShot : MonoBehaviour, IAbility
{
    public float AbilityForce { get; private set; } = 20f;
    public event Action<float, float> OnCooldownChanged;

    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;

    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;
    public float Cooldown => 2f;
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "GrapesOfWrath";

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Activate(Transform firePoint, float force)
    {
        if (!CanUse)
        {
            Debug.Log($"{AbilityName} cannot be used due to cooldown or other conditions.");
            return;
        }

        Debug.Log($"Activating {AbilityName}");
        StartCoroutine(CooldownRoutine());

        // Get the prefab from the AbilityPrefabManager
        GameObject prefab = AbilityPrefabManager.Instance.highVelocityShotPrefab;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;

        GameObject shot = Instantiate(prefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();
        Bullet bulletScript = shot.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Direction = direction;
        }

        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }


    public IEnumerator CooldownRoutine()
    {
        Debug.Log($"{AbilityName} cooldown started.");
        CanUse = false;
        float cooldownTimer = Cooldown;

        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            OnCooldownChanged?.Invoke(cooldownTimer, Cooldown);
            yield return null;
        }

        CanUse = true;
        Debug.Log($"{AbilityName} cooldown ended.");
        OnCooldownChanged?.Invoke(0, Cooldown);
    }
}

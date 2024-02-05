using System;
using System.Collections;
using UnityEditor.Playables;
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
    public string AbilityName { get; } = "HighVelocityShot";

    private Camera mainCamera;
    private Coroutine cooldownCoroutine; // Keep track of the cooldown coroutine

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

        // If a cooldown is already running, stop it before starting a new one
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(CooldownRoutine());

        // Get the prefab from the AbilityPrefabManager
        GameObject prefab = AbilityPrefabManager.Instance.explosiveShotPrefab;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePosition - (Vector2)firePoint.position).normalized;

        if (prefab != null)
        {
            GameObject shot = Instantiate(prefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();
            Bullet bulletScript = shot.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.Direction = direction;
            }

            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogError("HighVelocity prefab is missing in AbilityPrefabManager.");
        }
    }

    public IEnumerator CooldownRoutine()
    {
        Debug.Log($"{AbilityName} cooldown started.");
        CanUse = false; // Set the ability's CanUse flag to false to start the cooldown
        float cooldownTimer = Cooldown;

        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            OnCooldownChanged?.Invoke(cooldownTimer, Cooldown);
            yield return null;
        }

        CanUse = true; // Set the ability's CanUse flag back to true after the cooldown is complete
        Debug.Log($"{AbilityName} is ready to use again."); // Log when the ability is ready again

        cooldownCoroutine = null; // Reset the cooldown coroutine
    }

}

using System;
using System.Collections;
using UnityEngine;

public class HealingShot : MonoBehaviour, IAbility
{
    public float Cooldown => 5f;
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "HealingShot";

    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;

    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;

    public float AbilityForce => throw new NotImplementedException();

    public event Action<float, float> OnCooldownChanged;

    // This is the generic Activate method from IAbility
    public void Activate()
    {
        if (!CanUse)
        {
            Debug.Log($"{AbilityName} cannot be used due to cooldown or other conditions.");
            return;
        }

        Debug.Log($"Activating {AbilityName}");
        StartCoroutine(CooldownRoutine());

        // Direct healing logic here
        ApplyHealing();
    }

    private void ApplyHealing()
    {
        // Implement your healing logic here, for example:
        Debug.Log("Healing applied.");
        // PlayerHealth.Instance.Heal(amount); // Example healing application
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

    public void Activate(Transform firePoint, float force)
    {
        throw new NotImplementedException();
    }
}

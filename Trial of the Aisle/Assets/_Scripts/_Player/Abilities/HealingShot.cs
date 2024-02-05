using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingShot : MonoBehaviour, IAbility
{
    [SerializeField] private GameObject abilityPrefab;
    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;
    [SerializeField] private SO_AdjustHealth adjustHealthSO; // Reference to the SO_AdjustHealth ScriptableObject

    public string AbilityName { get; } = "Healing Shot";
    public float Cooldown { get; } = 10f; // Cooldown duration in seconds
    public bool CanUse { get; set; } = true;

    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;
    public float AbilityForce { get; } = 0f; // Not applicable for healing, but required by interface

    public event Action<float, float> OnCooldownChanged;

    // Mapping enum values to specific health amounts
    private Dictionary<ChangeHealth, float> healthChangeAmounts = new Dictionary<ChangeHealth, float>
    {
        { ChangeHealth.Small_Health, 10f },
        { ChangeHealth.Medium_Health, 20f },
        { ChangeHealth.Large_Health, 30f }
    };

    // Assuming you want to use Medium_Health for the healing shot
    private ChangeHealth healAmount = ChangeHealth.Medium_Health;

    public void Activate(Transform firePoint, float force)
    {
        if (!CanUse)
        {
            Debug.Log($"{AbilityName} cannot be used due to cooldown.");
            return;
        }

        Debug.Log($"Activating {AbilityName}");
        StartCoroutine(CooldownRoutine());

        if (adjustHealthSO != null)
        {
            // Use the mapped value for the selected heal amount
            float amountToHeal = healthChangeAmounts[healAmount];
            // Assuming AdjustPlayerHealth takes a float amount
            adjustHealthSO.AdjustPlayerHealth(amountToHeal);
            Debug.Log("Healing applied: " + amountToHeal);
        }
        else
        {
            Debug.LogError("AdjustHealth ScriptableObject reference is not set on HealingShot.");
        }
    }

    public IEnumerator CooldownRoutine()
    {
        Debug.Log($"{AbilityName} cooldown started.");
        float cooldownTimer = Cooldown;

        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            OnCooldownChanged?.Invoke(cooldownTimer, Cooldown);
            yield return null;
        }

        CanUse = true; // Set CanUse to true after the cooldown is complete
        Debug.Log($"{AbilityName} cooldown ended.");
        OnCooldownChanged?.Invoke(0, Cooldown);
    }

}

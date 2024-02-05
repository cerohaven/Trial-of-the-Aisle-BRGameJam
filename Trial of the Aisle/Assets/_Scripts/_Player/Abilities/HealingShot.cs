using System;
using System.Collections;
using UnityEngine;

public class HealingShot : MonoBehaviour, IAbility
{
    public float AbilityForce { get; private set; } = 10f;
    [SerializeField] private GameObject abilityPrefab;
    [SerializeField] private GameObject foregroundIcon;
    [SerializeField] private GameObject backgroundIcon;
    public event Action<float, float> OnCooldownChanged;

    public GameObject AbilityPrefab => abilityPrefab;
    public GameObject ForegroundIcon => foregroundIcon;
    public GameObject BackgroundIcon => backgroundIcon;
    public float Cooldown => 15f;
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "Lunch Break";

    [SerializeField] private SO_AdjustHealth adjustHealth; // Reference to the SO_AdjustHealth scriptable object
    [SerializeField] private float healAmount = 20f; // Amount to heal

    public void Activate(Transform firePoint, GameObject prefab, float force)
    {
        StartCoroutine(CooldownRoutine());
        Debug.Log("Healing ability activated");

        // Ensure SO_AdjustHealth is assigned in the Inspector
        if (adjustHealth != null)
        {
            adjustHealth.AdjustPlayerHealth(healAmount); // Directly adjust player health by the specified amount
        }
        else
        {
            Debug.LogError("SO_AdjustHealth reference not set on HealingShot.");
        }
    }

    public IEnumerator CooldownRoutine()
    {
        CanUse = false;
        float cooldownTimer = Cooldown;
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            OnCooldownChanged?.Invoke(cooldownTimer, Cooldown); // Notify about cooldown progress
            yield return null;
        }
        CanUse = true;
        OnCooldownChanged?.Invoke(0, Cooldown); // Notify cooldown is complete
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingShot : MonoBehaviour, IAbility
{
    public float Cooldown => 15f; // Cooldown for healing ability
    public bool CanUse { get; set; } = true;
    public string AbilityName { get; } = "Healing Shot";

    [SerializeField] private SO_AdjustHealth adjustHealth; // Now serialized for Inspector
    [SerializeField] private ChangeHealth healAmount; // Now serialized for Inspector


    // Constructor to set dependencies
    public HealingShot(SO_AdjustHealth adjustHealth, ChangeHealth healAmount)
    {
        this.adjustHealth = adjustHealth;
        this.healAmount = healAmount;
    }

    public void Activate(Transform firePoint, GameObject prefab, float force)
    {

        Debug.Log("Healing ability activated");

        if (prefab != null)
        {
            GameObject effect = GameObject.Instantiate(prefab, firePoint.position, Quaternion.identity);
            ParticleSystem ps = effect.GetComponentInChildren<ParticleSystem>();
            if (ps != null && !ps.main.playOnAwake)
            {
                ps.Play();
            }
            GameObject.Destroy(effect, 3f); // Destroy the effect after some time
        }

        // Directly heal the player or target
        adjustHealth.ChangePlayerHealthEventSend(healAmount, HealthType.Healing);
    }

    public IEnumerator CooldownRoutine()
    {
        CanUse = false;
        yield return new WaitForSeconds(Cooldown);
        CanUse = true;
    }
}


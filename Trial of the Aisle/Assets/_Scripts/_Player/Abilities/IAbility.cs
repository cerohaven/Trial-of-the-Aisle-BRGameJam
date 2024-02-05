using System;
using System.Collections;
using UnityEngine;

public interface IAbility
{
    event Action<float, float> OnCooldownChanged;
    string AbilityName { get; }
    float Cooldown { get; }
    GameObject ForegroundIcon { get; }
    GameObject BackgroundIcon { get; }
    GameObject AbilityPrefab { get; } // Prefab for the ability
    float AbilityForce { get; } // Force to be used with the ability

    bool CanUse { get; set; } // Property to indicate if the ability is ready for use

    void Activate(Transform firePoint, GameObject prefab, float force);
    IEnumerator CooldownRoutine();
}


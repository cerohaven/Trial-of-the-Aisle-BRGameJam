using System.Collections;
using UnityEngine;

public interface IAbility
{
    string AbilityName { get; }
    float Cooldown { get; }
    bool CanUse { get; set; }
    GameObject ForegroundIcon { get; }
    GameObject BackgroundIcon { get; }
    GameObject AbilityPrefab { get; } // Prefab for the ability
    float AbilityForce { get; } // Force to be used with the ability

    void Activate(Transform firePoint, GameObject prefab, float force);
    IEnumerator CooldownRoutine();
}
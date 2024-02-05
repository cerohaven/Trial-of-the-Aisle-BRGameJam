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
    float AbilityForce { get; } // Force to be used with the ability

    bool CanUse { get; set; } // Property to indicate if the ability is ready for use

    void Activate(Transform firePoint, float force);
    IEnumerator CooldownRoutine();
}


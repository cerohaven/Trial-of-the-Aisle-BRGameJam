using System.Collections;
using UnityEngine;

public interface IAbility
{
    string AbilityName { get; }
    float Cooldown { get; }
    bool CanUse { get; set; }
    void Activate(Transform firePoint, GameObject prefab, float force);
    IEnumerator CooldownRoutine();

}

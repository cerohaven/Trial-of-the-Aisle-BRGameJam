using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserbeamAbility", menuName = "Abilities/DD/M(oo)n Beam")]
public class MilkSprayAbility : Ability
{
    public GameObject milkSprayEffectPrefab; // Prefab with the milk spray effect
    public float sprayDuration = 5f; // Duration of the spray effect

    public override void Activate(GameObject owner)
    {
        Vector3 aimDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - owner.transform.position).normalized;
        aimDirection.z = 0; // Ensure the spray is aligned with the 2D plane

        GameObject sprayEffectInstance = Instantiate(milkSprayEffectPrefab, owner.transform.position, Quaternion.LookRotation(Vector3.forward, aimDirection));
        sprayEffectInstance.transform.SetParent(owner.transform);
        Destroy(sprayEffectInstance, sprayDuration);
    }
}
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserbeamAbility", menuName = "Abilities/DD/M(oo)n Beam")]
public class MilkSprayAbility : Ability
{
    public GameObject milkSprayEffectPrefab; // Prefab with the milk spray effect
    public float sprayDuration = 5f; // Duration of the spray effect

    public override void Activate(GameObject owner)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = owner.transform.position.z; // Ensure it's on the same plane as the player

        GameObject sprayEffectInstance = Instantiate(milkSprayEffectPrefab, mousePosition, Quaternion.identity);
        BeamFollowMouse beamScript = sprayEffectInstance.GetComponent<BeamFollowMouse>();
        if (beamScript != null)
        {
            beamScript.playerTransform = owner.transform;
        }

        Destroy(sprayEffectInstance, sprayDuration);
    }
}
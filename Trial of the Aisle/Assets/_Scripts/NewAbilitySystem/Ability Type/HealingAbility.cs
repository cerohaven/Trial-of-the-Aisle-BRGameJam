using UnityEngine;

[CreateAssetMenu(fileName = "HealingAbility", menuName = "Abilities/Healing Ability")]
public class HealingAbility : Ability
{
    public ChangeHealth changeHealthAmount; // The enum value specifying the amount of health to adjust
    public SO_AdjustHealth adjustHealthSO; // Reference to the Adjust Health Scriptable Object
    public GameObject healingEffectPrefab; // The prefab containing the healing particle effect

    public override void Activate(GameObject owner)
    {
        // Use the AdjustHealth SO to invoke the health adjustment event
        adjustHealthSO.ChangePlayerHealthEventSend(changeHealthAmount, HealthType.Healing);

        // Instantiate the healing effect prefab at the owner's position
        if (healingEffectPrefab != null)
        {
            GameObject effectInstance = Instantiate(healingEffectPrefab, owner.transform.position, Quaternion.identity);
            // Optionally, make the effect a child of the owner to keep the scene hierarchy organized
            effectInstance.transform.SetParent(owner.transform);

            // Destroy the effect after its duration
            // Assuming the particle system auto-destroys or you know the duration
            Destroy(effectInstance, 3f); // Adjust the time according to the effect's duration
        }
    }
}

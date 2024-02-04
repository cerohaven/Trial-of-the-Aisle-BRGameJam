using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> equippedAbilityComponents = new List<MonoBehaviour>(3);
    [SerializeField] private List<MonoBehaviour> unlockedAbilityComponents = new List<MonoBehaviour>();

    // Convert MonoBehaviour list to IAbility list for internal use
    public List<IAbility> EquippedAbilities => equippedAbilityComponents.Select(component => component as IAbility).ToList();
    public List<IAbility> UnlockedAbilities => unlockedAbilityComponents.Select(component => component as IAbility).ToList();

    public void UnlockAbility(MonoBehaviour newAbilityComponent)
    {
        IAbility newAbility = newAbilityComponent as IAbility;
        if (newAbility != null && !UnlockedAbilities.Contains(newAbility))
        {
            unlockedAbilityComponents.Add(newAbilityComponent);
            // Optionally, prompt the player to swap abilities here or just add it to the pool for later
        }
    }

    public void EquipAbility(MonoBehaviour abilityToEquipComponent, int slotIndex)
    {
        IAbility abilityToEquip = abilityToEquipComponent as IAbility;
        if (abilityToEquip != null && slotIndex < equippedAbilityComponents.Count && UnlockedAbilities.Contains(abilityToEquip))
        {
            equippedAbilityComponents[slotIndex] = abilityToEquipComponent; // Swap the ability in the specified slot
            // Update UI and any other necessary components here
        }
    }

   
}

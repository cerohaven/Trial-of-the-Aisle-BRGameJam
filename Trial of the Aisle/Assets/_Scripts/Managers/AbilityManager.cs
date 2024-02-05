using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> equippedAbilityComponents = new List<MonoBehaviour>(3);
    [SerializeField] private List<MonoBehaviour> unlockedAbilityComponents = new List<MonoBehaviour>();
    public PlayerInput playerInput;

    public event System.Action OnAbilitiesChanged; // Event to notify UI to update

    public List<IAbility> EquippedAbilities => equippedAbilityComponents.Select(component => component as IAbility).ToList();
    public List<IAbility> UnlockedAbilities => unlockedAbilityComponents.Select(component => component as IAbility).ToList();
    void Awake()
    {
        // If not assigned through the inspector, find the PlayerInput component
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput component not found on AbilityManager gameObject.");
            }
        }
    }
    public void UnlockAbility(MonoBehaviour newAbilityComponent)
    {
        if (newAbilityComponent is IAbility newAbility && !UnlockedAbilities.Contains(newAbility))
        {
            unlockedAbilityComponents.Add(newAbilityComponent);
            OnAbilitiesChanged?.Invoke(); // Notify UI to update
        }
    }

    public void EquipAbility(MonoBehaviour abilityToEquipComponent, int slotIndex)
    {
        if (abilityToEquipComponent is IAbility abilityToEquip && slotIndex < equippedAbilityComponents.Count && UnlockedAbilities.Contains(abilityToEquip))
        {
            equippedAbilityComponents[slotIndex] = abilityToEquipComponent;
            OnAbilitiesChanged?.Invoke(); // Notify UI to update
            Debug.Log($"Equipped {abilityToEquip.AbilityName} at slot {slotIndex}");
        }
        else
        {
            Debug.LogError($"Failed to equip ability at slot {slotIndex}. Ability is null or slot is out of range.");
        }
    }

    public IAbility GetEquippedAbility(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equippedAbilityComponents.Count && equippedAbilityComponents[slotIndex] != null)
        {
            return equippedAbilityComponents[slotIndex] as IAbility;
        }
        else
        {
            Debug.LogWarning($"Attempted to access an invalid ability slot: {slotIndex}");
            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesUI : MonoBehaviour
{
    public AbilityManager abilityManager;
    public Image[] abilityCooldownFills; // Array of Image components for ability cooldown fills

    // Reference points for all abilities in the UI Canvas
    public Transform ability1Reference;
    public Transform ability1BGReference;
    public Transform ability2Reference;
    public Transform ability2BGReference;
    public Transform ability3Reference;
    public Transform ability3BGReference;

    // Dictionary to store delegates for each ability to manage subscriptions
    private Dictionary<IAbility, Action<float, float>> cooldownDelegates = new Dictionary<IAbility, Action<float, float>>();

    private void Start()
    {
        abilityManager.OnAbilitiesChanged += HandleAbilitiesChanged;
        HandleAbilitiesChanged(); // Initial setup
    }

    private void HandleAbilitiesChanged()
    {
        UpdateAbilityIcons();
        SetupAbilityUI();
    }

    private void SetupAbilityUI()
    {
        UnsubscribeFromAbilityEvents(); // Unsubscribe to avoid duplicate subscriptions
        cooldownDelegates.Clear();

        var equippedAbilities = abilityManager.EquippedAbilities;
        for (int i = 0; i < equippedAbilities.Count; i++)
        {
            if (equippedAbilities[i] != null)
            {
                int abilityIndex = i; // Capture the current index
                Action<float, float> cooldownDelegate = (currentCooldown, totalCooldown) =>
                {
                    UpdateCooldownUI(abilityIndex, currentCooldown, totalCooldown);
                };

                equippedAbilities[i].OnCooldownChanged += cooldownDelegate;
                cooldownDelegates[equippedAbilities[i]] = cooldownDelegate; // Store the delegate reference
            }
        }
    }

    private void UpdateCooldownUI(int abilityIndex, float currentCooldown, float totalCooldown)
    {
        if (abilityIndex < abilityCooldownFills.Length && abilityCooldownFills[abilityIndex] != null)
        {
            abilityCooldownFills[abilityIndex].fillAmount = 1f - (currentCooldown / totalCooldown);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromAbilityEvents(); // Clean up event subscriptions
    }

    private void UnsubscribeFromAbilityEvents()
    {
        foreach (var kvp in cooldownDelegates)
        {
            kvp.Key.OnCooldownChanged -= kvp.Value;
        }
    }

    void UpdateAbilityIcons()
    {
        ClearExistingIcons(); // Clear existing icons before updating

        var equippedAbilities = abilityManager.EquippedAbilities;
        for (int i = 0; i < equippedAbilities.Count; i++)
        {
            if (equippedAbilities[i] != null)
            {
                InstantiateAbilityIcon(equippedAbilities[i].ForegroundIcon, i);
            }
        }
    }

    void InstantiateAbilityIcon(GameObject iconPrefab, int index)
    {
        Transform referencePoint = null;
        switch (index)
        {
            case 0:
                referencePoint = ability1Reference;
                break;
            case 1:
                referencePoint = ability2Reference;
                break;
            case 2:
                referencePoint = ability3Reference;
                break;
        }

        if (referencePoint != null)
        {
            var iconInstance = Instantiate(iconPrefab, referencePoint.position, Quaternion.identity, referencePoint);
            iconInstance.transform.localPosition = Vector3.zero;
            iconInstance.transform.localScale = Vector3.one;

            var iconImage = iconInstance.GetComponentInChildren<Image>();
            if (iconImage != null && index < abilityCooldownFills.Length)
            {
                abilityCooldownFills[index] = iconImage; // Assign the Image component for cooldown fill
            }
        }
    }
    void ClearExistingIcons()
    {
        ClearIconsInParent(ability1Reference);
        ClearIconsInParent(ability1BGReference);
        ClearIconsInParent(ability2Reference);
        ClearIconsInParent(ability2BGReference);
        ClearIconsInParent(ability3Reference);
        ClearIconsInParent(ability3BGReference);
    }

    void ClearIconsInParent(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}

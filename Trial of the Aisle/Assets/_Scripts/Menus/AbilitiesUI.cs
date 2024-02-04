using UnityEngine;

public class AbilitiesUI : MonoBehaviour
{
    public AbilityManager abilityManager;

    // Reference points for all abilities in the UI Canvas
    public Transform ability1Reference;
    public Transform ability1BGReference;
    public Transform ability2Reference;
    public Transform ability2BGReference;
    public Transform ability3Reference;
    public Transform ability3BGReference;

    private void Start()
    {
        abilityManager.OnAbilitiesChanged += UpdateAbilityIcons;
        UpdateAbilityIcons(); // Initial update
    }

    private void OnDestroy()
    {
        abilityManager.OnAbilitiesChanged -= UpdateAbilityIcons;
    }

    void UpdateAbilityIcons()
    {
        ClearExistingIcons(); // Clear existing icons before updating

        var equippedAbilities = abilityManager.EquippedAbilities;
        for (int i = 0; i < equippedAbilities.Count; i++)
        {
            if (equippedAbilities[i] != null)
            {
                switch (i)
                {
                    case 0:
                        InstantiateAbilityIcon(equippedAbilities[i].ForegroundIcon, ability1Reference);
                        InstantiateAbilityIcon(equippedAbilities[i].BackgroundIcon, ability1BGReference);
                        break;
                    case 1:
                        InstantiateAbilityIcon(equippedAbilities[i].ForegroundIcon, ability2Reference);
                        InstantiateAbilityIcon(equippedAbilities[i].BackgroundIcon, ability2BGReference);
                        break;
                    case 2:
                        InstantiateAbilityIcon(equippedAbilities[i].ForegroundIcon, ability3Reference);
                        InstantiateAbilityIcon(equippedAbilities[i].BackgroundIcon, ability3BGReference);
                        break;
                }
            }
        }
    }

    void InstantiateAbilityIcon(GameObject iconPrefab, Transform referencePoint)
    {
        var iconInstance = Instantiate(iconPrefab, referencePoint.position, Quaternion.identity, referencePoint);
        iconInstance.transform.localPosition = Vector3.zero; // Reset local position
        iconInstance.transform.localScale = Vector3.one; // Ensure correct scaling
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

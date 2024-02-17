using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Include DOTween namespace

public class AbilityUnlockUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Button abilityOneButton;
    [SerializeField] private Button abilityTwoButton;

    private Ability abilityOne;
    private Ability abilityTwo;

    // Reference to AbilityManager and PlayerAbilities
    private AbilityManager abilityManager;
    private PlayerAbilities playerAbilities;

    void Start()
    {
        unlockPanel.SetActive(false);

        // Find AbilityManager and PlayerAbilities instances in the scene
        abilityManager = FindObjectOfType<AbilityManager>();
        playerAbilities = FindObjectOfType<PlayerAbilities>();

        abilityOneButton.onClick.AddListener(() => OnAbilityButtonClicked(abilityOneButton, abilityOne, 0));
        abilityTwoButton.onClick.AddListener(() => OnAbilityButtonClicked(abilityTwoButton, abilityTwo, 1));
    }

    public void ShowUnlockedAbilities(Ability[] unlockedAbilities)
    {
        if (unlockedAbilities.Length >= 2)
        {
            abilityOne = unlockedAbilities[0];
            abilityTwo = unlockedAbilities[1];

            abilityOneButton.GetComponent<Image>().sprite = abilityOne.abilityIcon;
            abilityTwoButton.GetComponent<Image>().sprite = abilityTwo.abilityIcon;

            unlockPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Not enough abilities provided to ShowUnlockedAbilities.");
        }
    }

    private void OnAbilityButtonClicked(Button clickedButton, Ability selectedAbility, int slot)
    {
        // Shake the button using DOTween
        clickedButton.transform.DOShakePosition(0.5f, 5f, 10, 90, false, true);

        if (playerAbilities.equippedAbilities[slot] != selectedAbility)
        {
            // Swap the ability in the specified slot with the selected one
            playerAbilities.SwapAbility(slot, selectedAbility);
        }
        else
        {
            Debug.Log("Ability already equipped in this slot.");
        }

        unlockPanel.SetActive(false);
    }
}

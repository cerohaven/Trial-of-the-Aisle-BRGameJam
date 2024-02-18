using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewAbilitySelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Image abilityOneImage, abilityTwoImage;
    [SerializeField] private AbilityDatabase abilityDatabase;
    [SerializeField] private PlayerAbilities playerAbilities;

    private int selectedNewAbilityID = -1;

    void Start()
    {
        unlockPanel.SetActive(false); // Ensure the panel is hidden at the start
    }

    public void ShowAbilities(int abilityOneID, int abilityTwoID)
    {
        unlockPanel.SetActive(true); // Only show the panel when this method is called

        SetupAbilityImage(abilityOneImage, abilityOneID);
        SetupAbilityImage(abilityTwoImage, abilityTwoID);

        AddClickListener(abilityOneImage, abilityOneID);
        AddClickListener(abilityTwoImage, abilityTwoID);
    }

    private void SetupAbilityImage(Image abilityImage, int abilityID)
    {
        Ability ability = abilityDatabase.GetAbilityByID(abilityID);
        if (ability != null)
        {
            abilityImage.sprite = ability.abilityIcon;
            abilityImage.gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            Debug.LogWarning($"Ability with ID {abilityID} not found.");
            abilityImage.gameObject.GetComponent<Button>().interactable = false;
        }
    }

    private void AddClickListener(Image abilityImage, int abilityID)
    {
        Button button = abilityImage.GetComponent<Button>();
        button.onClick.RemoveAllListeners(); // Remove existing listeners to avoid stacking
        button.onClick.AddListener(() => SelectNewAbility(abilityID));
    }

    private void SelectNewAbility(int abilityID)
    {
        selectedNewAbilityID = abilityID;
        // Don't hide the panel here; wait for slot selection
    }

    void Update()
    {
        CheckForAbilitySlotSelection();
    }

    private void CheckForAbilitySlotSelection()
    {
        if (selectedNewAbilityID != -1) // If an ability has been selected
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerAbilities.SwapAbility(0, selectedNewAbilityID);
                FinishAbilitySelection();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerAbilities.SwapAbility(1, selectedNewAbilityID);
                FinishAbilitySelection();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerAbilities.SwapAbility(2, selectedNewAbilityID);
                FinishAbilitySelection();
            }
        }
    }

    private void FinishAbilitySelection()
    {
        selectedNewAbilityID = -1; // Reset selection
        unlockPanel.SetActive(false); // Hide the panel after selection is complete
    }
}

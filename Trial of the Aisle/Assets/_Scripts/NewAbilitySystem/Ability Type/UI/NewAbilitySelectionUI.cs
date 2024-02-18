using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class NewAbilitySelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Image abilityOneImage, abilityTwoImage;
    [SerializeField] private AbilityDatabase abilityDatabase;
    [SerializeField] private PlayerAbilities playerAbilities;

    private HashSet<int> swappedAbilities = new HashSet<int>();

    void Start()
    {
        unlockPanel.SetActive(false);
    }

    public void ShowAbilities(int abilityOneID, int abilityTwoID)
    {
        unlockPanel.SetActive(true);
        swappedAbilities.Clear(); // Reset for a new session

        SetupAbilityImage(abilityOneImage, abilityOneID);
        SetupAbilityImage(abilityTwoImage, abilityTwoID);
    }

    private void SetupAbilityImage(Image abilityImage, int abilityID)
    {
        Ability ability = abilityDatabase.GetAbilityByID(abilityID);
        if (ability != null)
        {
            abilityImage.sprite = ability.abilityIcon;
            abilityImage.GetComponent<Button>().interactable = !swappedAbilities.Contains(abilityID);

            Button button = abilityImage.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnAbilitySelected(abilityID, abilityImage));
        }
    }

    private void OnAbilitySelected(int abilityID, Image abilityImage)
    {
        if (!swappedAbilities.Contains(abilityID))
        {
            StartCoroutine(WaitForSlotSelection(abilityID, abilityImage));
        }
    }

    private IEnumerator WaitForSlotSelection(int abilityID, Image abilityImage)
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerAbilities.SwapAbility(2, abilityID);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerAbilities.SwapAbility(1, abilityID);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerAbilities.SwapAbility(0, abilityID);
                break;
            }
            yield return null;
        }

        swappedAbilities.Add(abilityID); // Mark as swapped
        abilityImage.GetComponent<Button>().interactable = false; // Disable further swaps for this ability
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && unlockPanel.activeSelf)
        {
            unlockPanel.SetActive(false);
        }
    }
}

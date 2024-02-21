using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro elements
using UnityEngine.EventSystems; // For event triggers
using System.Collections;
using System.Collections.Generic;

public class NewAbilitySelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Image abilityOneImage, abilityTwoImage;
    [SerializeField] private GameObject abilityOneTextPanel, abilityTwoTextPanel; // Panels containing header and description texts
    [SerializeField] private AbilityDatabase abilityDatabase;
    [SerializeField] private PlayerAbilities playerAbilities;

    private HashSet<int> swappedAbilities = new HashSet<int>();

    void Start()
    {
        unlockPanel.SetActive(false);
        abilityOneTextPanel.SetActive(false); // Initially hide the text panels
        abilityTwoTextPanel.SetActive(false);
    }

    public void ShowAbilities(int abilityOneID, int abilityTwoID)
    {
        unlockPanel.SetActive(true);
        swappedAbilities.Clear(); // Reset for a new session

        SetupAbilityUI(abilityOneImage, abilityOneID, abilityOneTextPanel);
        SetupAbilityUI(abilityTwoImage, abilityTwoID, abilityTwoTextPanel);
    }

    private void SetupAbilityUI(Image abilityImage, int abilityID, GameObject textPanel)
    {
        Ability ability = abilityDatabase.GetAbilityByID(abilityID);
        if (ability != null)
        {
            abilityImage.sprite = ability.abilityIcon;
            abilityImage.GetComponent<Button>().interactable = !swappedAbilities.Contains(abilityID);
            TextMeshProUGUI headerText = textPanel.transform.Find("Header").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = textPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();

            headerText.text = ability.abilityName;
            descriptionText.text = ability.abilityDescription;

            // Add mouse hover listeners
            AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerEnter, (data) => textPanel.SetActive(true));
            AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerExit, (data) => textPanel.SetActive(false));

            Button button = abilityImage.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnAbilitySelected(abilityID));
        }
    }

    private void OnAbilitySelected(int abilityID)
    {
        if (!swappedAbilities.Contains(abilityID))
        {
            StartCoroutine(WaitForSlotSelection(abilityID));
        }
    }

    private IEnumerator WaitForSlotSelection(int abilityID)
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerAbilities.SwapAbility(1, abilityID);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerAbilities.SwapAbility(2, abilityID);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerAbilities.SwapAbility(3, abilityID);
                break;
            }
            yield return null;
        }

        swappedAbilities.Add(abilityID); // Mark as swapped
    }

    private void AddEventTriggerListener(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && unlockPanel.activeSelf)
        {
            unlockPanel.SetActive(false);
        }
    }
}

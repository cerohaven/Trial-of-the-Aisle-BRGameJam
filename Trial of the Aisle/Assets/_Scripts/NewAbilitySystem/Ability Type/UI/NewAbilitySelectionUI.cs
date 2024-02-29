using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class NewAbilitySelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Image abilityOneImage, abilityTwoImage;
    [SerializeField] private GameObject abilityOneTextPanel, abilityTwoTextPanel; // Panels containing header and description texts
    [SerializeField] private PlayerAbilities playerAbilities;

    private Ability abilityOne;
    private Ability abilityTwo;
    private HashSet<Ability> swappedAbilities = new HashSet<Ability>();

    void Start()
    {
        unlockPanel.SetActive(false);
        abilityOneTextPanel.SetActive(false);
        abilityTwoTextPanel.SetActive(false);
    }

    public void ShowAbilities(Ability abilityOne, Ability abilityTwo)
    {
        this.abilityOne = abilityOne;
        this.abilityTwo = abilityTwo;

        unlockPanel.SetActive(true);
        swappedAbilities.Clear(); // Reset for a new session

        SetupAbilityUI(abilityOneImage, abilityOne, abilityOneTextPanel);
        SetupAbilityUI(abilityTwoImage, abilityTwo, abilityTwoTextPanel);
    }

    private void SetupAbilityUI(Image abilityImage, Ability ability, GameObject textPanel)
    {
        if (ability != null)
        {
            abilityImage.sprite = ability.abilityIcon;
            abilityImage.GetComponent<Button>().interactable = !swappedAbilities.Contains(ability);
            TextMeshProUGUI headerText = textPanel.transform.Find("Header").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = textPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();

            headerText.text = ability.abilityName;
            descriptionText.text = ability.abilityDescription;

            // Add mouse hover listeners
            AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerEnter, (data) => textPanel.SetActive(true));
            AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerExit, (data) => textPanel.SetActive(false));

            Button button = abilityImage.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnAbilitySelected(ability));
        }
    }

    private void OnAbilitySelected(Ability ability)
    {
        if (!swappedAbilities.Contains(ability))
        {
            StartCoroutine(WaitForSlotSelection(ability));
        }
    }

    private IEnumerator WaitForSlotSelection(Ability ability)
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayerAbilities.Instance.SwapAbility(0, ability);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayerAbilities.Instance.SwapAbility(1, ability);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayerAbilities.Instance.SwapAbility(2, ability);
                break;
            }
            yield return null;
        }

        swappedAbilities.Add(ability); // Mark as swapped
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

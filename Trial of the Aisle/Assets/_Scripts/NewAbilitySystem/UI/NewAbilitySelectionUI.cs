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
    [SerializeField] private Image bossCardImage;
    [SerializeField] private GameObject abilityOneTextPanel, abilityTwoTextPanel; // Panels containing header and description texts

    private LTDescr delayOne;
    private LTDescr delayTwo;

    public Ability abilityOne;
    public Ability abilityTwo;
    public HashSet<Ability> swappedAbilities = new HashSet<Ability>();

    private void Awake()
    {
        GameManager.Instance.UiInstances.Add(gameObject);
    }

    public void ShowAbilities(Ability _abilityOne, Ability _abilityTwo, Sprite bossCard)
    {
        abilityOne = _abilityOne;
        abilityTwo = _abilityTwo;

        Debug.Log(abilityOne, abilityTwo);

        if (abilityOne == null || abilityTwo == null)
        {
            Debug.LogWarning("Not enough new abilities specified for post-boss defeat selection.");
            return;
        }

        bossCardImage.sprite = bossCard;

        unlockPanel.SetActive(true);
        swappedAbilities.Clear(); // Reset for a new session

        SetupAbilityUI(abilityOneImage, abilityOne, abilityOneTextPanel);
        SetupAbilityUI(abilityTwoImage, abilityTwo, abilityTwoTextPanel);
    }

    private void SetupAbilityUI(Image abilityImage, Ability ability, GameObject textPanel)
    {
        // Check if the ability is null and handle accordingly
        if (ability == null)
        {
            abilityImage.enabled = false; // Disable the ability image if the ability is null
            textPanel.SetActive(false); // Hide the text panel as well
            return; // Exit the method as there's nothing more to setup
        }

        abilityImage.sprite = ability.abilityIcon;
        abilityImage.GetComponent<Button>().interactable = !swappedAbilities.Contains(ability);
        TextMeshProUGUI headerText = textPanel.transform.Find("Header").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = textPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();

        headerText.text = ability.abilityName;
        descriptionText.text = ability.abilityDescription;

        AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerEnter, (data) => OnAbilityPointerEnter((PointerEventData)data, textPanel));
        AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerExit, (data) => OnAbilityPointerExit((PointerEventData)data, textPanel));
    }

    private void OnAbilityPointerEnter(PointerEventData eventData, GameObject textPanel)
    {
        // Cancel any ongoing delay to prevent overlapping tooltips
        if (delayOne != null) LeanTween.cancel(delayOne.uniqueId);
        if (delayTwo != null) LeanTween.cancel(delayTwo.uniqueId);

        // Set delay based on which text panel is being hovered over
        if (textPanel == abilityOneTextPanel)
        {
            delayOne = LeanTween.delayedCall(0.1f, () => abilityOneTextPanel.SetActive(true));
        }
        else if (textPanel == abilityTwoTextPanel)
        {
            delayTwo = LeanTween.delayedCall(0.1f, () => abilityTwoTextPanel.SetActive(true));
        }
    }

    private void OnAbilityPointerExit(PointerEventData eventData, GameObject textPanel)
    {
        // Cancel the appropriate delay and hide the text panel
        if (textPanel == abilityOneTextPanel)
        {
            if (delayOne != null) LeanTween.cancel(delayOne.uniqueId);
            abilityOneTextPanel.SetActive(false);
        }
        else if (textPanel == abilityTwoTextPanel)
        {
            if (delayTwo != null) LeanTween.cancel(delayTwo.uniqueId);
            abilityTwoTextPanel.SetActive(false);
        }
    }

    private void AddEventTriggerListener(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>() ?? target.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && unlockPanel.activeSelf)
        {
            unlockPanel.SetActive(false);
        }
    }
}

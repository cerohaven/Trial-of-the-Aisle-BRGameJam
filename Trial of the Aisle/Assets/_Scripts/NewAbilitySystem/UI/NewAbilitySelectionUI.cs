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

        // Add mouse hover listeners
        AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerEnter, (data) => textPanel.SetActive(true));
        AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerExit, (data) => textPanel.SetActive(false));

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
        if (Input.GetKeyDown(KeyCode.Escape) && unlockPanel.activeSelf)
        {
            unlockPanel.SetActive(false);
            SceneTransitionController.Instance.LoadNextScene();
        }
    }
}

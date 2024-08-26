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
    private GameObject textPanelGO;

    public Ability abilityOne;
    public Ability abilityTwo;
    public HashSet<Ability> swappedAbilities = new HashSet<Ability>();
 

    private void Awake()
    {
        GameManager.Instance.UiInstances.Add(gameObject);
    }
    private void Start()
    {
        ShowAbilities(GameManager.Instance.BossProfile.Ability1, GameManager.Instance.BossProfile.Ability2, GameManager.Instance.BossProfile.PostBattleCanvasUI);
        GameManager.Instance.PlayerInputHandler.PlayerInput.SwitchCurrentActionMap("UI");
    }

    private void ShowAbilities(Ability _abilityOne, Ability _abilityTwo, Sprite bossCard)
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

    private void MouseHoverOverAbility(GameObject textPanel)
    {
        textPanel.SetActive(true);
        textPanelGO = textPanel;
        LeanTween.rotateZ(textPanel, 2f, 0.2f).setEaseOutBack().setOnComplete(OnHoverComplete);
    }

    private void OnHoverComplete()
    {
        LeanTween.rotateZ(textPanelGO, 0f, 0.1f).setEaseOutBack();

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
        Debug.DrawLine(new Vector3(Screen.width / 2.5f, 0, 0), new Vector3(Screen.width / 2.5f, 1280, 0));
        Debug.DrawLine(new Vector3(0, Screen.height / 1.3f, 0), new Vector3(1920, Screen.height / 1.3f, 0));
        if (Input.GetKeyDown(KeyCode.Escape) && unlockPanel.activeSelf)
        {
            unlockPanel.SetActive(false);
            GameManager.Instance.TransitionType = TransitionType.BossBattle;
            GameManager.Instance.Boss_BGM_Postbattle.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            GameManager.Instance.LoadNextScene();
        }
    }

}

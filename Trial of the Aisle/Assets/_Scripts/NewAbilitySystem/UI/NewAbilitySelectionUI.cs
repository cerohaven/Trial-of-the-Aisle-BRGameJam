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

    private RectTransform CanvasRect;
    private RectTransform[] abilityTextPanelRects = new RectTransform[2];

    private Vector2 offset; // if they're not past the bounds of the screen return back to original pos

    public Ability abilityOne;
    public Ability abilityTwo;
    public HashSet<Ability> swappedAbilities = new HashSet<Ability>();

    private readonly float blackBorderWidth = 100;
    private GameObject textPanelGO;
    private void Awake()
    {
        GameManager.Instance.UiInstances.Add(gameObject);

        CanvasRect = unlockPanel.GetComponent<RectTransform>();
        abilityTextPanelRects[0] = abilityOneTextPanel.GetComponent<RectTransform>();
        abilityTextPanelRects[1] = abilityTwoTextPanel.GetComponent<RectTransform>();
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
        AddEventTriggerListener(abilityImage.gameObject, EventTriggerType.PointerEnter, (data) => MouseHoverOverAbility(textPanel));
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
        //making sure the text panels stay within the bounds of the screen
        if (abilityOneTextPanel != null)
        {
            abilityTextPanelRects[0].anchoredPosition = Vector2.zero;
            abilityTextPanelRects[0].anchoredPosition -= KeepFullyOnScreen(abilityTextPanelRects[0], abilityOneTextPanel.transform.position);
            //KeepFullyOnScreenFlip(abilityTextPanelRects[0], abilityOneTextPanel.transform.position);
        }
        if(abilityTwoTextPanel != null)
        {
            abilityTextPanelRects[1].anchoredPosition = Vector2.zero;
            abilityTextPanelRects[1].anchoredPosition -= KeepFullyOnScreen(abilityTextPanelRects[1], abilityTwoTextPanel.transform.position);
            //KeepFullyOnScreenFlip(abilityTextPanelRects[1], abilityTwoTextPanel.transform.position);
        }
        

        if (Input.GetKeyDown(KeyCode.Escape) && unlockPanel.activeSelf)
        {
            unlockPanel.SetActive(false);
        }
    }

    //https://forum.unity.com/threads/keep-ui-objects-inside-screen.523766/
    //Only Offsets as much is needed to not go off screen
    Vector2 KeepFullyOnScreen(RectTransform panel, Vector3 newPos)
    {
        offset = Vector2.zero;

        float maxX = (CanvasRect.sizeDelta.x - panel.sizeDelta.x - blackBorderWidth * 2) * 0.5f;
        float maxY = (CanvasRect.sizeDelta.y - panel.sizeDelta.y - blackBorderWidth) * 0.5f;

        //offset the current local position based on how much we're off screen
        //If we don't add this code, the text box's position will stay in the corrected spot forever
        if (newPos.x > maxX)
        {
            offset.x = newPos.x - maxX;
        }
        if(newPos.y > maxY)
        {
            offset.y = newPos.y - maxY;
        }
        
        return offset;
    }

    //Flips the entire image and adjusts the anchor position
    private void KeepFullyOnScreenFlip(RectTransform panel, Vector3 newPos)
    {

        float maxX = (CanvasRect.sizeDelta.x - panel.sizeDelta.x) * 0.5f;
        float maxY = (CanvasRect.sizeDelta.y - panel.sizeDelta.y) * 0.5f;

        //offset the current local position based on how much we're off screen
        //If we don't add this code, the text box's position will stay in the corrected spot forever

        bool pastScreenX = newPos.x > maxX;
        bool pastScreenY = newPos.y > maxY;

        if (pastScreenX)
        {
            panel.anchoredPosition = new Vector2(-panel.sizeDelta.x + 130, panel.anchoredPosition.y);
        }

        if (pastScreenY)
        {
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -panel.sizeDelta.y + 80);
        }


    }

}

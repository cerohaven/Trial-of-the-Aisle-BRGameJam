using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions; // Assigned in the Inspector
    [SerializeField] private AbilityDatabase abilityDatabase; // Reference to the Ability Database

    public int[] equippedAbilityIDs = new int[3]; // Array of IDs for equipped abilities
    private float[] cooldowns; // Array of cooldowns for each ability

    public RectTransform[] abilitySlotsUIReference; // UI slots for ability icons
    private Image[] abilityIcons; // Dynamically created ability icon instances
    private Image[] cooldownOverlays; // Dynamically created cooldown overlay instances

    private void Awake()
    {
        // Find and enable the action map
        var playerActions = inputActions.FindActionMap("Player");
        playerActions.Enable();

        // Subscribe to input actions for ability activation
        playerActions.FindAction("AbilityOne").performed += _ => ActivateAbility(0);
        playerActions.FindAction("AbilityTwo").performed += _ => ActivateAbility(1);
        playerActions.FindAction("AbilityThree").performed += _ => ActivateAbility(2);
    }

    void Start()
    {
        cooldowns = new float[equippedAbilityIDs.Length];
        abilityIcons = new Image[equippedAbilityIDs.Length];
        cooldownOverlays = new Image[equippedAbilityIDs.Length];

        InitializeAbilityUI();
    }

    void Update()
    {
        UpdateCooldowns();
    }

    private void InitializeAbilityUI()
    {
        for (int i = 0; i < equippedAbilityIDs.Length; i++)
        {
            Ability ability = abilityDatabase.GetAbilityByID(equippedAbilityIDs[i]);

            // Ensure the slot has an Image component for the ability icon
            Image iconImage = abilitySlotsUIReference[i].GetComponent<Image>();
            if (iconImage == null)
            {
                iconImage = abilitySlotsUIReference[i].gameObject.AddComponent<Image>();
            }
            abilityIcons[i] = iconImage;

            if (ability != null)
            {
                // Set the ability icon sprite
                abilityIcons[i].sprite = ability.abilityIcon;
                abilityIcons[i].enabled = true; // Ensure the icon is visible

                // Setup or find an existing Image component for the cooldown overlay within this slot
                Image overlayImage = FindOrCreateOverlayImage(abilitySlotsUIReference[i]);

                // Set up the cooldown overlay properties
                overlayImage.sprite = ability.abilityIcon; // Use the ability icon for the overlay
                overlayImage.color = new Color(0.5f, 0.5f, 0.5f, 0.75f); // Semi-transparent grey
                overlayImage.type = Image.Type.Filled;
                overlayImage.fillMethod = Image.FillMethod.Radial360;
                overlayImage.fillClockwise = false;
                overlayImage.fillOrigin = (int)Image.Origin360.Top;
                overlayImage.fillAmount = 0; // Start fully transparent (no cooldown)
                cooldownOverlays[i] = overlayImage;
            }
            else
            {
                // If the ability is null, ensure the slot is disabled
                abilityIcons[i].enabled = false;
                if (abilitySlotsUIReference[i].Find("CooldownOverlay"))
                {
                    abilitySlotsUIReference[i].Find("CooldownOverlay").gameObject.SetActive(false);
                }
            }
        }
    }

    private Image FindOrCreateOverlayImage(RectTransform parentSlot)
    {
        Transform existingOverlay = parentSlot.Find("CooldownOverlay");
        if (existingOverlay != null)
        {
            return existingOverlay.GetComponent<Image>();
        }

        GameObject overlayObject = new GameObject("CooldownOverlay", typeof(Image));
        overlayObject.transform.SetParent(parentSlot, false);
        RectTransform overlayRT = overlayObject.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.sizeDelta = Vector2.zero; // Overlay covers the entire slot

        return overlayObject.GetComponent<Image>();
    }


    public void ActivateAbility(int slot)
    {
        if (slot >= 0 && slot < equippedAbilityIDs.Length)
        {
            Ability ability = abilityDatabase.GetAbilityByID(equippedAbilityIDs[slot]);
            if (ability != null && cooldowns[slot] <= 0)
            {
                ability.Activate(gameObject);
                cooldowns[slot] = ability.cooldownTime;
                cooldownOverlays[slot].fillAmount = 1; // Indicate cooldown start
            }
        }
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < equippedAbilityIDs.Length; i++)
        {
            Ability ability = abilityDatabase.GetAbilityByID(equippedAbilityIDs[i]);
            if (ability != null && cooldowns[i] > 0)
            {
                cooldowns[i] -= Time.deltaTime;
                cooldownOverlays[i].fillAmount = cooldowns[i] / ability.cooldownTime;
            }
            else
            {
                cooldownOverlays[i].fillAmount = 0;
            }
        }
    }

    public void SwapAbility(int slot, int newAbilityID)
    {
        if (slot >= 0 && slot < equippedAbilityIDs.Length)
        {
            equippedAbilityIDs[slot] = newAbilityID;

            Ability newAbility = abilityDatabase.GetAbilityByID(newAbilityID);
            if (newAbility != null)
            {
                abilityIcons[slot].sprite = newAbility.abilityIcon;
                cooldowns[slot] = 0;
                cooldownOverlays[slot].fillAmount = 0;

                // Update the cooldown overlay sprite to the new ability's icon
                cooldownOverlays[slot].sprite = newAbility.abilityIcon;
            }
        }
    }

}

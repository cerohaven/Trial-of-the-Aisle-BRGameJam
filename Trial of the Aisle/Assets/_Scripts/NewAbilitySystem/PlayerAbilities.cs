using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions; // Assigned in the Inspector
    [SerializeField] public AbilityDatabase abilityDatabase; // Reference to the Ability Database

    [SerializeField] public int[] equippedAbilityIDs = new int[3]; // Array of IDs for equipped abilities, can be -1 to indicate no ability equipped
    private Ability[] equippedAbilities = new Ability[3]; // Array of Ability references corresponding to the IDs
    private float[] cooldowns; // Array of cooldowns for each ability

    [SerializeField] private RectTransform[] abilitySlotsUIReference; // UI slots for ability icons
    private Image[] abilityIcons; // Dynamically created ability icon instances
    private Image[] cooldownOverlays; // Dynamically created cooldown overlay instances

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        var playerActions = inputActions.FindActionMap("Player");
        playerActions.Enable();

        playerActions.FindAction("AbilityOne").performed += _ => ActivateAbility(0);
        playerActions.FindAction("AbilityTwo").performed += _ => ActivateAbility(1);
        playerActions.FindAction("AbilityThree").performed += _ => ActivateAbility(2);

        cooldowns = new float[equippedAbilityIDs.Length];
        abilityIcons = new Image[equippedAbilityIDs.Length];
        cooldownOverlays = new Image[equippedAbilityIDs.Length];

        // Explicitly initialize equippedAbilities based on equippedAbilityIDs
        for (int i = 0; i < equippedAbilityIDs.Length; i++)
        {
            if (equippedAbilityIDs[i] >= 0)
            {
                equippedAbilities[i] = abilityDatabase.GetAbilityByID(equippedAbilityIDs[i]);
            }
            else
            {
                equippedAbilities[i] = null; 
            }
        }
    }

    void Start()
    {
        InitializeAbilityUI();
    }

    void Update()
    {
        UpdateCooldowns();
    }

    void InitializeAbilityUI()
    {
        for (int i = 0; i < equippedAbilities.Length; i++)
        {
            if (equippedAbilities[i] != null) // Ability is present
            {
                Ability ability = equippedAbilities[i];

                // Ensure the slot UI is active
                abilitySlotsUIReference[i].gameObject.SetActive(true);

                // Set or update the ability icon
                Image iconImage = abilitySlotsUIReference[i].GetComponent<Image>();
                if (iconImage == null)
                {
                    iconImage = abilitySlotsUIReference[i].gameObject.AddComponent<Image>();
                }
                abilityIcons[i] = iconImage;
                abilityIcons[i].sprite = ability.abilityIcon;
                abilityIcons[i].enabled = true;

                // Set or update the cooldown overlay
                Image overlayImage = FindOrCreateOverlayImage(abilitySlotsUIReference[i]);
                overlayImage.sprite = ability.abilityIcon; // Use the same icon for the overlay
                overlayImage.color = new Color(0.5f, 0.5f, 0.5f, 0.75f); // Semi-transparent overlay
                overlayImage.type = Image.Type.Filled;
                overlayImage.fillMethod = Image.FillMethod.Radial360;
                overlayImage.fillClockwise = false;
                overlayImage.fillOrigin = (int)Image.Origin360.Top;
                overlayImage.fillAmount = 0; // No cooldown initially
                cooldownOverlays[i] = overlayImage;
            }
            else // No ability is present in this slot
            {
                // Ensure the slot UI is inactive if there's no ability
                abilitySlotsUIReference[i].gameObject.SetActive(false);
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
        overlayRT.sizeDelta = Vector2.zero;
        return overlayObject.GetComponent<Image>();
    }

    public void ActivateAbility(int slot)
    {
        // Check for null in equippedAbilities to avoid null reference exceptions
        if (slot >= 0 && slot < equippedAbilities.Length && equippedAbilities[slot] != null && cooldowns[slot] <= 0)
        {
            equippedAbilities[slot].Activate(gameObject);
            cooldowns[slot] = equippedAbilities[slot].cooldownTime;
            cooldownOverlays[slot].fillAmount = 1; // Indicate cooldown start
        }
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < equippedAbilities.Length; i++)
        {
            // Again, check for null to avoid exceptions
            if (equippedAbilities[i] != null && cooldowns[i] > 0)
            {
                cooldowns[i] -= Time.deltaTime;
                cooldownOverlays[i].fillAmount = cooldowns[i] / equippedAbilities[i].cooldownTime;

                if (cooldowns[i] <= 0)
                {
                    cooldownOverlays[i].fillAmount = 0; // Reset the cooldown overlay
                }
            }
        }
    }

    public void SwapAbility(int slot, Ability newAbility)
    {
        if (slot >= 0 && slot < equippedAbilities.Length)
        {
            // Assign the new ability to the specified slot, even if the current ability is null
            equippedAbilities[slot] = newAbility;

            // Update the corresponding ability ID, handling the case where newAbility is null
            equippedAbilityIDs[slot] = newAbility != null ? newAbility.ID : -1;

            // Reset the cooldown for the newly swapped ability slot to ensure it's ready to use
            cooldowns[slot] = 0;

            // Re-initialize the UI elements to reflect the newly swapped abilities
            InitializeAbilityUI();
        }
    }


    public float GetCooldown(int slot)
    {
        return slot >= 0 && slot < cooldowns.Length ? cooldowns[slot] : -1;
    }
}

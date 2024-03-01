using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions; // Assigned in the Inspector
    [SerializeField] private AbilityDatabase abilityDatabase; // Reference to the Ability Database

    [SerializeField] private int[] equippedAbilityIDs = new int[3]; // Array of IDs for equipped abilities
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

        // Find and enable the action map
        var playerActions = inputActions.FindActionMap("Player");
        playerActions.Enable();

        // Subscribe to input actions for ability activation
        playerActions.FindAction("AbilityOne").performed += _ => ActivateAbility(0);
        playerActions.FindAction("AbilityTwo").performed += _ => ActivateAbility(1);
        playerActions.FindAction("AbilityThree").performed += _ => ActivateAbility(2);

        cooldowns = new float[equippedAbilityIDs.Length];
        abilityIcons = new Image[equippedAbilityIDs.Length];
        cooldownOverlays = new Image[equippedAbilityIDs.Length];
    }

    void Start()
    {
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
            equippedAbilities[i] = abilityDatabase.GetAbilityByID(equippedAbilityIDs[i]);
            Ability ability = equippedAbilities[i];

            if (ability != null)
            {
                Image iconImage = abilitySlotsUIReference[i].GetComponent<Image>() ?? abilitySlotsUIReference[i].gameObject.AddComponent<Image>();
                abilityIcons[i] = iconImage;
                abilityIcons[i].sprite = ability.abilityIcon;
                abilityIcons[i].enabled = true;

                Image overlayImage = FindOrCreateOverlayImage(abilitySlotsUIReference[i]);
                overlayImage.sprite = ability.abilityIcon;
                overlayImage.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
                overlayImage.type = Image.Type.Filled;
                overlayImage.fillMethod = Image.FillMethod.Radial360;
                overlayImage.fillClockwise = false;
                overlayImage.fillOrigin = (int)Image.Origin360.Top;
                overlayImage.fillAmount = 0;
                cooldownOverlays[i] = overlayImage;
            }
            else
            {
                abilityIcons[i].enabled = false;
                Transform cooldownOverlayTransform = abilitySlotsUIReference[i].Find("CooldownOverlay");
                if (cooldownOverlayTransform != null)
                {
                    cooldownOverlayTransform.gameObject.SetActive(false);
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
        overlayRT.sizeDelta = Vector2.zero;
        return overlayObject.GetComponent<Image>();
    }

    public void ActivateAbility(int slot)
    {
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
            equippedAbilities[slot] = newAbility;
            equippedAbilityIDs[slot] = newAbility.ID; // Update the ID array to match the new ability
            cooldowns[slot] = 0; // Reset cooldown for swapped ability
            InitializeAbilityUI(); // Re-initialize UI to reflect the swapped abilities
        }
    }

    public float GetCooldown(int slot)
    {
        return slot >= 0 && slot < cooldowns.Length ? cooldowns[slot] : -1;
    }
}

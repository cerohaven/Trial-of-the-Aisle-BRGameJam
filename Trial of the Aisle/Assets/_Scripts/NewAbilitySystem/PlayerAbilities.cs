using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions; // Assigned in the Inspector

    public Ability[] equippedAbilities = new Ability[3]; // Array of equipped abilities
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
        cooldowns = new float[equippedAbilities.Length];
        abilityIcons = new Image[equippedAbilities.Length];
        cooldownOverlays = new Image[equippedAbilities.Length];

        InitializeAbilityUI();
    }

    void Update()
    {
        UpdateCooldowns();
    }

    private void InitializeAbilityUI()
    {
        for (int i = 0; i < equippedAbilities.Length; i++)
        {
            if (equippedAbilities[i] != null)
            {
                // Ensure the slot has an Image component for the ability icon
                Image iconImage = abilitySlotsUIReference[i].GetComponent<Image>();
                if (iconImage == null)
                {
                    iconImage = abilitySlotsUIReference[i].gameObject.AddComponent<Image>();
                }
                abilityIcons[i] = iconImage;

                // Set the ability icon sprite
                abilityIcons[i].sprite = equippedAbilities[i].abilityIcon;
                abilityIcons[i].enabled = true; // Ensure the icon is visible

                // Setup or find an existing Image component for the cooldown overlay
                Image overlayImage = abilitySlotsUIReference[i].Find("CooldownOverlay")?.GetComponent<Image>();
                if (overlayImage == null)
                {
                    GameObject overlayObject = new GameObject("CooldownOverlay", typeof(Image));
                    overlayObject.transform.SetParent(abilitySlotsUIReference[i], false);
                    overlayImage = overlayObject.GetComponent<Image>();
                }

                // Use the same sprite for the overlay but control visibility with fillAmount
                overlayImage.sprite = equippedAbilities[i].abilityIcon;
                overlayImage.color = new Color(0.5f, 0.5f, 0.5f, 1); // Set the color to grey
                overlayImage.type = Image.Type.Filled;
                overlayImage.fillMethod = Image.FillMethod.Radial360;
                overlayImage.fillClockwise = false; // Adjust if necessary
                overlayImage.fillOrigin = (int)Image.Origin360.Top; // Adjust if necessary
                overlayImage.fillAmount = 0; // Start fully transparent (no cooldown)
                cooldownOverlays[i] = overlayImage;
            }
        }
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
            if (cooldowns[i] > 0)
            {
                cooldowns[i] -= Time.deltaTime;
                cooldownOverlays[i].fillAmount = cooldowns[i] / equippedAbilities[i].cooldownTime;
            }
            else
            {
                cooldownOverlays[i].fillAmount = 0;
            }
        }
    }

    public void SwapAbility(int slot, Ability newAbility)
    {
        if (slot >= 0 && slot < equippedAbilities.Length && newAbility != null)
        {
            equippedAbilities[slot] = newAbility;
            abilityIcons[slot].sprite = newAbility.abilityIcon; // Update the icon sprite
            cooldowns[slot] = 0; // Reset the cooldown
            cooldownOverlays[slot].fillAmount = 0; // Reset the cooldown overlay
        }
    }
}

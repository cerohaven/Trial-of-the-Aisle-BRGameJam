using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    [SerializeField]
    private Ability[] allAbilities; // Assign all possible abilities in the inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Method to get an Ability by name
    public Ability GetAbilityByName(string abilityName)
    {
        foreach (Ability ability in allAbilities)
        {
            if (ability.abilityName == abilityName)
            {
                return ability;
            }
        }
        Debug.LogWarning("Ability not found: " + abilityName);
        return null;
    }

    // Method to swap the player's ability with a new one
    public void SwapPlayerAbility(int slot, Ability newAbility)
    {
        PlayerAbilities playerAbilities = FindObjectOfType<PlayerAbilities>(); // Find the PlayerAbilities script in the scene
        if (playerAbilities != null)
        {
            playerAbilities.SwapAbility(slot, newAbility);
        }
        else
        {
            Debug.LogWarning("PlayerAbilities script not found in the scene.");
        }
    }
}

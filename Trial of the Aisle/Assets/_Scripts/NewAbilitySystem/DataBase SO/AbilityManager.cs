using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate.
        }
        else
        {
            Instance = this; // Assign singleton instance.
            DontDestroyOnLoad(gameObject); // Optionally, make this persist across scenes.
        }
    }

    // Method to swap the player's ability in the specified slot with a new ability.
    // The newAbility parameter is now a direct reference to an Ability object.
    public void SwapPlayerAbility(int slot, Ability newAbility)
    {
        PlayerAbilities playerAbilities = PlayerAbilities.Instance; // Access the singleton instance of PlayerAbilities.
        if (playerAbilities != null && newAbility != null)
        {
            playerAbilities.SwapAbility(slot, newAbility); // Swap the ability in the specified slot.
        }
        else
        {
            if (playerAbilities == null)
            {
                Debug.LogWarning("PlayerAbilities instance not found.");
            }
            if (newAbility == null)
            {
                Debug.LogWarning("Attempted to assign a null ability.");
            }
        }
    }
}

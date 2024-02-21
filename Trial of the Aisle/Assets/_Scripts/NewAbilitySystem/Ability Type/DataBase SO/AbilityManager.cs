using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    public AbilityDatabase abilityDatabase; // Assign in the Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SwapPlayerAbility(int slot, int abilityID)
    {
        // Check if the ability ID is valid by trying to get the ability from the database
        Ability newAbility = abilityDatabase.GetAbilityByID(abilityID);
        if (newAbility != null)
        {
            // Find the PlayerAbilities component in the scene
            PlayerAbilities playerAbilities = FindObjectOfType<PlayerAbilities>();
            if (playerAbilities != null)
            {
                // Use the ability ID to swap abilities, instead of the Ability object
                playerAbilities.SwapAbility(slot, abilityID);
            }
            else
            {
                Debug.LogWarning("PlayerAbilities script not found in the scene.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid ability ID: {abilityID}. Ability not found in database.");
        }
    }
}

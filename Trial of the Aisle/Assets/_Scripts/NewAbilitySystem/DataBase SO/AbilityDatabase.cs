using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityDatabase", menuName = "Abilities/ParentControllers/Ability Database")]
public class AbilityDatabase : ScriptableObject
{
    public List<Ability> abilities; // Stores all abilities in the database.

    private void OnValidate()
    {
        UpdateAbilityIDs();
    }

    public void UpdateAbilityIDs()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] != null)
            {
                abilities[i].ID = i; // Update the ID to match the index
            }
        }
    }

    // Retrieves an ability by its ID, ensuring the ID is within the valid range.
    public Ability GetAbilityByID(int id)
    {
        if (id >= 0 && id < abilities.Count) // Check for valid ID range.
        {
            return abilities[id]; // Return the ability at the specified ID.
        }
        Debug.LogWarning("Invalid ability ID: " + id); // Log a warning if the ID is out of range.
        return null; // Return null for invalid IDs.
    }
}

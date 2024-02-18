using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDatabase", menuName = "Abilities/Ability Database")]
public class AbilityDatabase : ScriptableObject
{
    public List<Ability> abilities; // List containing all abilities

    public Ability GetAbilityByID(int id)
    {
        if (id >= 0 && id < abilities.Count)
        {
            return abilities[id];
        }
        Debug.LogWarning("Invalid ability ID: " + id);
        return null;
    }
}

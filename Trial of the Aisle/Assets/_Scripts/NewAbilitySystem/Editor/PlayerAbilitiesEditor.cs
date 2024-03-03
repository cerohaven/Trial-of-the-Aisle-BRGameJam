using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(PlayerAbilities))]
public class PlayerAbilitiesEditor : Editor
{
    private string searchQuery = string.Empty;
    private List<Ability> filteredAbilities = new List<Ability>();
    private int selectedSlot = 0; // Default to slot 0

    public override void OnInspectorGUI()
    {
        // Draw the default inspector options
        DrawDefaultInspector();

        PlayerAbilities playerAbilities = (PlayerAbilities)target;

        if (playerAbilities.abilityDatabase == null)
        {
            EditorGUILayout.HelpBox("Ability Database is not assigned in PlayerAbilities.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Ability Search", EditorStyles.boldLabel);

        // Start checking for changes
        EditorGUI.BeginChangeCheck();
        searchQuery = EditorGUILayout.TextField("Search Abilities", searchQuery);
        // If the search query changed, update the filtered abilities list
        if (EditorGUI.EndChangeCheck())
        {
            filteredAbilities = FilterAbilities(playerAbilities.abilityDatabase, searchQuery);
        }

        // Slot Selection
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Select Slot", EditorStyles.boldLabel);
        selectedSlot = GUILayout.SelectionGrid(selectedSlot, new string[] { "Slot 0", "Slot 1", "Slot 2" }, 3);

        DisplayAbilities(playerAbilities, filteredAbilities);
    }

    private List<Ability> FilterAbilities(AbilityDatabase abilityDatabase, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return abilityDatabase.abilities;
        }

        // Check if the query is numeric and could be an index
        if (int.TryParse(query, out int index))
        {
            // If the query is a valid index, return a list with the ability at that index (if it exists)
            if (index >= 0 && index < abilityDatabase.abilities.Count)
            {
                return new List<Ability> { abilityDatabase.abilities[index] };
            }
            // If the index is out of range, return an empty list to indicate no match
            return new List<Ability>();
        }
        else
        {
            // If the query is not numeric, search by name
            return abilityDatabase.abilities.Where(ability => ability.name.ToLower().Contains(query.ToLower())).ToList();
        }
    }


    private void DisplayAbilities(PlayerAbilities playerAbilities, List<Ability> abilities)
    {
        // "None" button to deselect any ability for the selected slot
        if (GUILayout.Button("None"))
        {
            AssignNone(playerAbilities, selectedSlot);
        }

        foreach (Ability ability in abilities)
        {
            if (GUILayout.Button(ability.name))
            {
                AssignAbility(playerAbilities, ability, selectedSlot);
            }
        }
    }

    private void AssignNone(PlayerAbilities playerAbilities, int slot)
    {
        if (slot >= 0 && slot < playerAbilities.equippedAbilityIDs.Length)
        {
            playerAbilities.equippedAbilityIDs[slot] = -1; // Set to -1 to indicate no ability is equipped in this slot
            EditorUtility.SetDirty(playerAbilities); // Mark the PlayerAbilities script as dirty to ensure changes are saved
            Debug.Log($"Cleared ability assignment for Slot {slot} in PlayerAbilities");
        }
    }

    private void AssignAbility(PlayerAbilities playerAbilities, Ability selectedAbility, int slot)
    {
        if (slot >= 0 && slot < playerAbilities.equippedAbilityIDs.Length)
        {
            playerAbilities.equippedAbilityIDs[slot] = selectedAbility.ID; // Assign the selected ability's ID to the chosen slot
            EditorUtility.SetDirty(playerAbilities); // Mark the PlayerAbilities script as dirty to ensure changes are saved
            Debug.Log($"Assigned {selectedAbility.name} to Slot {slot} in PlayerAbilities");
        }
    }
}

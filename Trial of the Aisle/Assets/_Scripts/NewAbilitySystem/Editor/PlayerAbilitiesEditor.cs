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
    private AbilityType? filterAbilityType = null; // Nullable enum for ability type filter
    private BossPrefix? filterBossPrefix = null; // Nullable enum for boss prefix filter

    private bool showSearch = true;
    //private bool showAbilityTypeFilter = true;
    //private bool showBossPrefixFilter = true;

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

        // Collapsible Filters Group
        showSearch = EditorGUILayout.Foldout(showSearch, "Search and Filters", true);
        if (showSearch)
        {
            GUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            // Search Field
            EditorGUI.BeginChangeCheck();
            searchQuery = EditorGUILayout.TextField("Search Abilities", searchQuery);
            if (EditorGUI.EndChangeCheck())
            {
                filteredAbilities = FilterAbilities(playerAbilities.abilityDatabase);
            }

            // Ability Type Filter
            EditorGUI.BeginChangeCheck();
            filterAbilityType = (AbilityType?)EditorGUILayout.EnumPopup("Ability Type", filterAbilityType.HasValue ? filterAbilityType.Value : (AbilityType)(-1));
            if (EditorGUI.EndChangeCheck())
            {
                filteredAbilities = FilterAbilities(playerAbilities.abilityDatabase);
            }
            if (filterAbilityType == (AbilityType)(-1)) filterAbilityType = null;

            // Boss Prefix Filter
            EditorGUI.BeginChangeCheck();
            filterBossPrefix = (BossPrefix?)EditorGUILayout.EnumPopup("Boss Prefix", filterBossPrefix.HasValue ? filterBossPrefix.Value : (BossPrefix)(-1));
            if (EditorGUI.EndChangeCheck())
            {
                filteredAbilities = FilterAbilities(playerAbilities.abilityDatabase);
            }
            if (filterBossPrefix == (BossPrefix)(-1)) filterBossPrefix = null;

            // Clear Search and Filters Button
            GUILayout.Space(10);
            if (GUILayout.Button("Clear Search and Filters"))
            {
                searchQuery = string.Empty;
                filterAbilityType = null;
                filterBossPrefix = null;
                filteredAbilities = FilterAbilities(playerAbilities.abilityDatabase);
            }

            EditorGUI.indentLevel--;
            GUILayout.EndVertical();
        }

        // Slot Selection
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Select Slot", EditorStyles.boldLabel);
        selectedSlot = GUILayout.SelectionGrid(selectedSlot, new string[] { "Slot 0", "Slot 1", "Slot 2" }, 3);

        // "Clear Slot" button
        GUILayout.Space(15);
        if (GUILayout.Button("Clear Slot", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
        {
            AssignNone(playerAbilities, selectedSlot);
        }

        // Line buffer below "Clear Slot"
        GUILayout.Space(10);
        GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1));

        // Display filtered abilities
        GUILayout.Space(10);
        DisplayAbilities(playerAbilities, filteredAbilities);
    }

    private List<Ability> FilterAbilities(AbilityDatabase abilityDatabase)
    {
        var abilities = abilityDatabase.abilities;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            if (int.TryParse(searchQuery, out int index) && index >= 0 && index < abilities.Count)
            {
                abilities = new List<Ability> { abilities[index] };
            }
            else
            {
                abilities = abilities.Where(ability => ability.name.ToLower().Contains(searchQuery.ToLower())).ToList();
            }
        }

        if (filterAbilityType.HasValue)
        {
            abilities = abilities.Where(ability => ability.abilityType == filterAbilityType.Value).ToList();
        }

        if (filterBossPrefix.HasValue)
        {
            abilities = abilities.Where(ability => ability.bossPrefix == filterBossPrefix.Value).ToList();
        }

        return abilities;
    }

    private void DisplayAbilities(PlayerAbilities playerAbilities, List<Ability> abilities)
    {
        float windowWidth = EditorGUIUtility.currentViewWidth - 40; // Adjust for padding
        float baseIconSize = 80; // Base size for a standard width
        float baseButtonHeight = 35;
        float baseButtonWidth = 100;
        float scale = windowWidth / (4 * baseButtonWidth); // Adjust the scale based on window width
        scale = Mathf.Clamp(scale, 0.5f, 1.0f); // Limit scaling to between 50% and 100%

        float iconSize = baseIconSize * scale;
        float buttonHeight = baseButtonHeight * scale;
        float buttonWidth = baseButtonWidth * scale;
        int abilitiesPerRow = Mathf.FloorToInt(windowWidth / buttonWidth);

        if (abilitiesPerRow <= 0) abilitiesPerRow = 1; // Ensure at least one ability is displayed per row

        for (int i = 0; i < abilities.Count; i += abilitiesPerRow)
        {
            GUILayout.BeginHorizontal("box");

            for (int j = 0; j < abilitiesPerRow && i + j < abilities.Count; j++)
            {
                Ability ability = abilities[i + j];

                GUILayout.BeginVertical(GUILayout.Width(buttonWidth));
                GUILayout.Space(5);

                // Center the icon and text within the button
                if (GUILayout.Button("", GUILayout.Width(buttonWidth), GUILayout.Height(iconSize + buttonHeight)))
                {
                    AssignAbility(playerAbilities, ability, selectedSlot);
                }

                Rect lastRect = GUILayoutUtility.GetLastRect();

                // Draw the ability icon centered within the button
                if (ability.abilityIcon != null)
                {
                    Rect iconRect = new Rect(
                        lastRect.x + (lastRect.width - iconSize) / 2,
                        lastRect.y + (lastRect.height - iconSize - buttonHeight) / 2,
                        iconSize,
                        iconSize
                    );

                    Rect spriteRect = new Rect(
                        ability.abilityIcon.textureRect.x / ability.abilityIcon.texture.width,
                        ability.abilityIcon.textureRect.y / ability.abilityIcon.texture.height,
                        ability.abilityIcon.textureRect.width / ability.abilityIcon.texture.width,
                        ability.abilityIcon.textureRect.height / ability.abilityIcon.texture.height
                    );

                    GUI.DrawTextureWithTexCoords(iconRect, ability.abilityIcon.texture, spriteRect);
                }

                // Draw the ability name centered below the icon
                GUIStyle centeredTextStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = Mathf.FloorToInt(12 * scale) // Scale text size
                };
                GUI.Label(new Rect(lastRect.x, lastRect.y + iconSize, lastRect.width, buttonHeight), ability.name, centeredTextStyle);

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
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

    // Populate filteredAbilities list with all abilities from the ability database on editor script enable
    private void OnEnable()
    {
        PlayerAbilities playerAbilities = (PlayerAbilities)target;
        if (playerAbilities.abilityDatabase != null)
        {
            filteredAbilities = FilterAbilities(playerAbilities.abilityDatabase);
        }
    }
}

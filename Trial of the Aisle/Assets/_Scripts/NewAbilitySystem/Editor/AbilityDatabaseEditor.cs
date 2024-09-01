using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbilityDatabase))]
public class AbilityDatabaseEditor : Editor
{
    private string searchFilter = "";
    private AbilityType? filterAbilityType = null; // Nullable enum for ability type filter
    private BossPrefix? filterBossPrefix = null; // Nullable enum for boss prefix filter
    private Vector2 scrollPosition;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        AbilityDatabase database = (AbilityDatabase)target;

        if (GUILayout.Button("Auto-Populate Abilities"))
        {
            AutoPopulateAbilities(database);
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Ability Search", EditorStyles.boldLabel);

        // Search field to filter abilities by name
        searchFilter = EditorGUILayout.TextField("Search by Name", searchFilter);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Filter by Ability Type", EditorStyles.boldLabel);
        filterAbilityType = (AbilityType?)EditorGUILayout.EnumPopup("Ability Type", filterAbilityType.HasValue ? filterAbilityType.Value : (AbilityType)(-1));
        if (filterAbilityType == (AbilityType)(-1)) filterAbilityType = null; // Clear filter if "None" selected

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Filter by Boss Prefix", EditorStyles.boldLabel);
        filterBossPrefix = (BossPrefix?)EditorGUILayout.EnumPopup("Boss Prefix", filterBossPrefix.HasValue ? filterBossPrefix.Value : (BossPrefix)(-1));
        if (filterBossPrefix == (BossPrefix)(-1)) filterBossPrefix = null; // Clear filter if "None" selected

        GUILayout.Space(10);
        if (GUILayout.Button("Clear Filters"))
        {
            searchFilter = "";
            filterAbilityType = null;
            filterBossPrefix = null;
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Abilities", EditorStyles.boldLabel);

        // Begin a scrolling view inside GUI, for better handling of large lists
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        foreach (var ability in database.abilities)
        {
            if (ability == null) continue; // Skip null entries

            bool matchesSearch = string.IsNullOrEmpty(searchFilter) || ability.abilityName.ToLower().Contains(searchFilter.ToLower());
            bool matchesType = !filterAbilityType.HasValue || ability.abilityType == filterAbilityType.Value;
            bool matchesPrefix = !filterBossPrefix.HasValue || ability.bossPrefix == filterBossPrefix.Value;

            if (matchesSearch && matchesType && matchesPrefix)
            {
                GUILayout.BeginHorizontal("box");

                // Draw the ability icon using the specific sprite portion from the sprite sheet
                if (ability.abilityIcon != null)
                {
                    Rect spriteRect = new Rect(
                        ability.abilityIcon.textureRect.x / ability.abilityIcon.texture.width,
                        ability.abilityIcon.textureRect.y / ability.abilityIcon.texture.height,
                        ability.abilityIcon.textureRect.width / ability.abilityIcon.texture.width,
                        ability.abilityIcon.textureRect.height / ability.abilityIcon.texture.height
                    );

                    GUILayout.Box("", GUILayout.Width(50), GUILayout.Height(50)); // Reserve space for the sprite
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    GUI.DrawTextureWithTexCoords(lastRect, ability.abilityIcon.texture, spriteRect);
                }
                else
                {
                    GUILayout.Label("No Icon", GUILayout.Width(50), GUILayout.Height(50)); // Placeholder if no icon
                }

                EditorGUILayout.LabelField($"ID: {ability.ID}", GUILayout.Width(50));
                EditorGUILayout.LabelField($"Name: {ability.abilityName}");
                EditorGUILayout.LabelField($"Type: {ability.abilityType}", GUILayout.Width(100));
                EditorGUILayout.LabelField($"Boss Prefix: {ability.bossPrefix}", GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
    }

    private void AutoPopulateAbilities(AbilityDatabase database)
    {
        Undo.RecordObject(database, "Auto-Populate Abilities"); // Enable undo for this action
        database.abilities.Clear();
        var guids = AssetDatabase.FindAssets("t:Ability");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var ability = AssetDatabase.LoadAssetAtPath<Ability>(path);
            if (ability != null)
            {
                database.abilities.Add(ability);
            }
        }

        // Explicitly call UpdateAbilityIDs to ensure IDs are updated immediately after population
        database.UpdateAbilityIDs();

        EditorUtility.SetDirty(database); // Mark the database as dirty to ensure the changes are saved
    }
}

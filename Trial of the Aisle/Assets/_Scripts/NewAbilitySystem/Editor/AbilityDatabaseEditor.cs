using UnityEditor;
using UnityEngine;

/// <summary>
/// THIS MUST BE IN AN EDITOR FOLDER TO WORK
/// </summary>
[CustomEditor(typeof(AbilityDatabase))]
public class AbilityDatabaseEditor : Editor
{
    private string searchFilter = "";
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
        if (GUILayout.Button("Clear Search"))
        {
            searchFilter = ""; // Clear the search filter
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Abilities", EditorStyles.boldLabel);

        // Begin a scrolling view inside GUI, for better handling of large lists
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

        foreach (var ability in database.abilities)
        {
            if (ability == null) continue; // Skip null entries

            if (string.IsNullOrEmpty(searchFilter) || ability.abilityName.ToLower().Contains(searchFilter.ToLower()))
            {
                GUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField($"ID: {ability.ID}", GUILayout.Width(50));
                EditorGUILayout.LabelField($"Name: {ability.abilityName}");
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
        EditorUtility.SetDirty(database); // Mark the database as dirty to ensure the changes are saved
    }
}

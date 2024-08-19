using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


[CustomEditor(typeof(SO_ProjectilePattern))]
public class SO_ProjectilePattern_Editor : Editor
{
    SerializedProperty _projectilePatterns;
    SerializedProperty _patternNumberToUse;
    SerializedProperty _patternType;
    SerializedProperty _patternModStruct;
    SerializedProperty _foldouts;
    SerializedProperty _baseFoldout;
    SerializedProperty basePAT;

    private void OnEnable()
    {
        _projectilePatterns = serializedObject.FindProperty("_projectilePatterns");
        _patternNumberToUse = serializedObject.FindProperty("_patternNumberToUse");
        _foldouts = serializedObject.FindProperty("_foldouts");
        _baseFoldout = serializedObject.FindProperty("_baseFoldout");
        basePAT = serializedObject.FindProperty("basePAT");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SO_ProjectilePattern projectilePattern = (SO_ProjectilePattern)target;  

        #region GUI Styles
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.normal.textColor = Color.white;
        titleStyle.fontSize = 20;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleLeft;

        GUIStyle foldoutTitle = new GUIStyle();
        foldoutTitle.normal.textColor = Color.white;
        foldoutTitle.fontSize = 12;
        foldoutTitle.fontStyle = FontStyle.Bold;
        #endregion


        EditorGUILayout.PropertyField(_patternNumberToUse);

        #region Projectile Pattern Modifiers Title and Line
        GUILayout.Space(10);
        GUILayout.Label("Projectile Pattern Modifiers", titleStyle);

        Rect lineRect = GUILayoutUtility.GetLastRect();
        Rect separatorLine = new Rect(lineRect.x, lineRect.y + titleStyle.fontSize, lineRect.width, 2);
        EditorGUI.DrawRect(separatorLine, Color.white);
        GUILayout.Space(10);
        #endregion

        




        #region Base Pattern Struct
        bool baseVal = _baseFoldout.boolValue;
        
        _baseFoldout.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(baseVal, "Pattern 0: Base");

        Rect lastRect = new Rect();
        lastRect = GUILayoutUtility.GetLastRect();

        if (baseVal)
        {
            GUILayout.Space(5);
            EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 20, lastRect.width, 50), new Color(0.19f, 0.19f, 0.19f));

            for (int j = 0; j < basePAT.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label(basePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue, GUILayout.Width(150));
                EditorGUILayout.PropertyField(basePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue"), GUIContent.none, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        GUILayout.Space(10);
        for (int i = 0; i < _projectilePatterns.arraySize; i++)
        {


            //Setting the name of the foldout group
            _patternType = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternType");
            _patternModStruct = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternTypeModifiers");

            bool val = _foldouts.GetArrayElementAtIndex(i).boolValue;
            _foldouts.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(val, $"Pattern {i+1}:  {_patternType.GetEnumName<ProjectilePatterns>()}");
            

            lastRect = GUILayoutUtility.GetLastRect();

            //If the user has expanded this foldout, display the PatternModifier with a horizontal layoutgroup - string and then value
            if (val)
            {
                GUILayout.Space(10);
                //Draw the background based on how many elements we have in the list
                EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 20, lastRect.width, 20 + (_patternModStruct.arraySize * 28) ), new Color(0.19f, 0.19f, 0.19f));

                #region Pattern Type Field
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Pattern Type", GUILayout.Width(150));
                ProjectilePatterns myEnum = (ProjectilePatterns)EditorGUILayout.EnumPopup(projectilePattern.ProjectilePatternList[i].thisPatternType,  GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                #endregion

                if (projectilePattern.ProjectilePatternList[i].thisPatternType != myEnum )
                {
                    projectilePattern.ProjectilePatternList[i].thisPatternType = myEnum;
                    projectilePattern.SetPatternInfo(i);
                    EditorUtility.SetDirty(projectilePattern);
                }


                #region Pattern Struct
                for (int j = 0; j < _patternModStruct.arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(_patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue, GUILayout.Width(150));
                    EditorGUILayout.PropertyField(_patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modValue"), GUIContent.none, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                    serializedObject.ApplyModifiedProperties();
                }
                #endregion
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(10);
        }

        GUILayout.Space(20);


        #region Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add new Modifier"))
        {
            AddToList(projectilePattern);
        }
        if (GUILayout.Button("Remove Last Modifier"))
        {
            RemoveFromList(projectilePattern);
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Clear All"))
        {
            ClearAll(projectilePattern);
        }
        #endregion


        _patternNumberToUse.intValue = Mathf.Clamp(_patternNumberToUse.intValue, 0, _projectilePatterns.arraySize);



        serializedObject.ApplyModifiedProperties();



    }



    private void AddToList(SO_ProjectilePattern projectilePattern)
    {

        projectilePattern.Foldouts.Add(false);
        projectilePattern.ProjectilePatternList.Add(new ProjectilePattern());

        projectilePattern.SetPatternInfo(projectilePattern.ProjectilePatternList.Count-1);

    }

    private void RemoveFromList(SO_ProjectilePattern projectilePattern)
    {
        
        if (projectilePattern.ProjectilePatternList.Count<= 0) return;

        projectilePattern.Foldouts.RemoveAt(projectilePattern.Foldouts.Count - 1);
        projectilePattern.ProjectilePatternList.RemoveAt(projectilePattern.ProjectilePatternList.Count - 1);

        //Debug.Log(_projectilePatterns.arraySize);
        //Debug.Log(_foldouts.arraySize);

    }

    private void ClearAll(SO_ProjectilePattern projectilePattern)
    {
        projectilePattern.Foldouts.Clear();
        projectilePattern.ProjectilePatternList.Clear();

    }
}

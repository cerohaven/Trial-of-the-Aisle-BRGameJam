using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


[CustomEditor(typeof(SO_ProjectilePattern))]
public class SO_ProjectilePattern_Editor : Editor
{
    private readonly int sizeOfField = 20;

    SerializedProperty _projectilePatterns;
    SerializedProperty _patternNumberToUse;
    SerializedProperty _patternType;
    SerializedProperty _patternModStruct;
    SerializedProperty _foldouts;
    SerializedProperty _baseFoldout;
    SerializedProperty _removeSpecificPattern;

    SO_ProjectilePattern projectilePatternWAH;



    //Pattern Modifers
    SerializedProperty basePAT;
    SerializedProperty somePAT;
    SerializedProperty spreadPAT;
    SerializedProperty randomizeAnglePAT;
    SerializedProperty rapidPAT;
    SerializedProperty burstPAT;
    SerializedProperty randomizeSpawnOffsetPAT;
    //SerializedProperty rapidPAT;

    bool addNewArray;
    bool removeArray;
    bool clearArray;

    float maxLabelWidth = 150;
    float defaultWidth;
    readonly Color selectedPatternColour = new Color(0.1f, 0.45f, 0.45f);
    readonly Color regularPatternColour = new Color(0.19f, 0.19f, 0.19f);

    private void OnEnable()
    {
        _projectilePatterns = serializedObject.FindProperty("_projectilePatterns");
        _patternNumberToUse = serializedObject.FindProperty("_patternNumberToUse");
        _foldouts = serializedObject.FindProperty("_foldouts");
        _baseFoldout = serializedObject.FindProperty("_baseFoldout");
        _removeSpecificPattern = serializedObject.FindProperty("_removeSpecificPattern");

        basePAT = serializedObject.FindProperty("basePAT");
        somePAT = serializedObject.FindProperty("somePAT");
        spreadPAT = serializedObject.FindProperty("spreadPAT");
        randomizeAnglePAT = serializedObject.FindProperty("randomizeAnglePAT");
        rapidPAT = serializedObject.FindProperty("rapidPAT");
        burstPAT = serializedObject.FindProperty("burstPAT");
        randomizeSpawnOffsetPAT = serializedObject.FindProperty("randomizeSpawnOffsetPAT");
        //randomizeAnglePAT = serializedObject.FindProperty("randomizeAnglePAT");
        

        projectilePatternWAH = (SO_ProjectilePattern)target;
    }

    public override void OnInspectorGUI()
    {


        serializedObject.Update();

        if (addNewArray)
        {
            AddToList();
        }
        else if (removeArray)
        {
            RemoveFromList();
        }
        else if (clearArray)
        {
            ClearAll();
        }
        addNewArray = false;
        removeArray = false;
        clearArray = false;


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

        Color bgColourToUse = _patternNumberToUse.intValue == 0 ? selectedPatternColour : regularPatternColour;

        if (baseVal)
        {
            GUILayout.Space(5);
            EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 20, lastRect.width, 10 + (basePAT.arraySize * sizeOfField) + 10), bgColourToUse);

            for (int j = 0; j < basePAT.arraySize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                basePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue =
                       EditorGUILayout.FloatField(basePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue,
                                                   basePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue,
                                                   GUILayout.MaxWidth(250));
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

                bgColourToUse = _patternNumberToUse.intValue == i+1 ? selectedPatternColour : regularPatternColour;

                //Draw the background based on how many elements we have in the list
                EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 20, lastRect.width, 10 + ( _patternModStruct.arraySize * sizeOfField) + sizeOfField + 10), bgColourToUse);

                #region Pattern Type Field
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Pattern Type", GUILayout.Width(150));
                ProjectilePatterns myEnum = (ProjectilePatterns)EditorGUILayout.EnumPopup(projectilePatternWAH.ProjectilePatternList[i].thisPatternType,  GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Changing Struct Info When Enum Changes
                if (projectilePatternWAH.ProjectilePatternList[i].thisPatternType != myEnum) 
                {
                    _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternType").enumValueIndex = (int)myEnum;
   
                    UpdateModifierInfo(myEnum, _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternTypeModifiers"));

                }
                #endregion

                #region Pattern Struct
                for (int j = 0; j < _patternModStruct.arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    //GUILayout.Label(_patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue, GUILayout.Width(150));
                    //EditorGUILayout.PropertyField(_patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modValue"), GUIContent.none, GUILayout.Width(100));

                    defaultWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = maxLabelWidth;

                    _patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = 
                        EditorGUILayout.FloatField( _patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue,
                                                    _patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue,
                                                    GUILayout.MaxWidth(250));
                    EditorGUILayout.EndHorizontal();

                    EditorGUIUtility.labelWidth = defaultWidth;
                }
                #endregion
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(10);
        }

        GUILayout.Space(20);


        _patternNumberToUse.intValue = Mathf.Clamp(_patternNumberToUse.intValue, 0, _projectilePatterns.arraySize);

        if(_projectilePatterns.arraySize != 0) _removeSpecificPattern.intValue = Mathf.Clamp(_removeSpecificPattern.intValue, 1, _projectilePatterns.arraySize);


        #region Buttons
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_removeSpecificPattern);
        if (GUILayout.Button($"Remove Modifier {_removeSpecificPattern.intValue}"))
        {
            removeArray = true;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Add new Modifier"))
        {
            addNewArray = true;
        }
        if (GUILayout.Button("Clear All"))
        {
            clearArray = true;
            
        }
        #endregion

        EditorUtility.SetDirty(projectilePatternWAH);
        serializedObject.ApplyModifiedProperties();




    }

    private void UpdateModifierInfo(ProjectilePatterns pat,  SerializedProperty property)
    {

        property.ClearArray();

        switch (pat)
        {
            case ProjectilePatterns.Some:
                property.arraySize = somePAT.arraySize;
                for (int j = 0; j < property.arraySize; j++)
                {
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = somePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue;
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = somePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue;
                }
                break;

            case ProjectilePatterns.Spread:
                property.arraySize = spreadPAT.arraySize;
                for (int j = 0; j < property.arraySize; j++)
                {
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = spreadPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue;
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = spreadPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue;
                }
                break;

            case ProjectilePatterns.Randomize_Angle:
                property.arraySize = randomizeAnglePAT.arraySize;
                for (int j = 0; j < property.arraySize; j++)
                {
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = randomizeAnglePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue;
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = randomizeAnglePAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue;
                }
                break;
            case ProjectilePatterns.Rapid:
                property.arraySize = rapidPAT.arraySize;
                for (int j = 0; j < property.arraySize; j++)
                {
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = rapidPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue;
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = rapidPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue;
                }
                break;
            case ProjectilePatterns.Burst:
                property.arraySize = burstPAT.arraySize;
                for (int j = 0; j < property.arraySize; j++)
                {
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = burstPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue;
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = burstPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue;
                }
                break;
            case ProjectilePatterns.Randomize_Spawn_Offset:
                property.arraySize = randomizeSpawnOffsetPAT.arraySize;
                for (int j = 0; j < property.arraySize; j++)
                {
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = randomizeSpawnOffsetPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue;
                    property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = randomizeSpawnOffsetPAT.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue;
                }
                break;
        }

        
    }



    private void AddToList()
    {
        _projectilePatterns.arraySize++;
        _foldouts.arraySize++;
        _foldouts.GetArrayElementAtIndex(_foldouts.arraySize-1).boolValue = true;
        serializedObject.ApplyModifiedProperties();

        //Get the current information for the modifer we're using (SOME)
        _projectilePatterns.GetArrayElementAtIndex(_projectilePatterns.arraySize - 1).FindPropertyRelative("thisPatternType").enumValueIndex = 0;
        UpdateModifierInfo(ProjectilePatterns.Some, _projectilePatterns.GetArrayElementAtIndex(_projectilePatterns.arraySize-1).FindPropertyRelative("thisPatternTypeModifiers"));
       
    }

    private void RemoveFromList()
    {
        
        if (_projectilePatterns.arraySize <= 0) return;


        _projectilePatterns.DeleteArrayElementAtIndex(_removeSpecificPattern.intValue-1);
        _foldouts.DeleteArrayElementAtIndex(_removeSpecificPattern.intValue-1);


    }

    private void ClearAll()
    {
        _projectilePatterns.ClearArray();
        _foldouts.ClearArray();


    }
}

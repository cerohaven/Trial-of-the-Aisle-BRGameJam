using UnityEditor;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(SO_ProjectilePattern))]
public class SO_ProjectilePattern_Editor : Editor
{
    private readonly int sizeOfField = 20;
    private readonly float trashCanScaleOfBox = 0.6f; //0 - 1. 0 is 0 scale, 1 is the scale of the box;

    SerializedProperty _projectilePatterns;
    SerializedProperty _patternType;
    SerializedProperty _patternModStruct;
    SerializedProperty _patternFoldout;
    SerializedProperty _patternIsActive;
    SerializedProperty _baseFoldout;

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

    private int selectedElement;
    private bool triggerRemoveFromList;

    private readonly Color activeColour = new Color(0.1f, 0.35f, 0.55f, 0.5f); //blue
    private readonly Color regularPatternColour = new Color(0.19f, 0.19f, 0.19f); //darker
    private readonly Color regularPatternColour2 = new Color(0.25f, 0.25f, 0.25f); //lighter
    Color defaultGUIContentColour;
    Color defaultGUIBackgroundColour;
    private ReorderableList projPatternModList;

    private void OnEnable()
    {
        _projectilePatterns = serializedObject.FindProperty("_projectilePatterns");
        _baseFoldout = serializedObject.FindProperty("_baseFoldout");

        basePAT = serializedObject.FindProperty("basePAT");
        somePAT = serializedObject.FindProperty("somePAT");
        spreadPAT = serializedObject.FindProperty("spreadPAT");
        randomizeAnglePAT = serializedObject.FindProperty("randomizeAnglePAT");
        rapidPAT = serializedObject.FindProperty("rapidPAT");
        burstPAT = serializedObject.FindProperty("burstPAT");
        randomizeSpawnOffsetPAT = serializedObject.FindProperty("randomizeSpawnOffsetPAT");
        //randomizeAnglePAT = serializedObject.FindProperty("randomizeAnglePAT");

        projectilePatternWAH = (SO_ProjectilePattern)target;

        defaultGUIContentColour = GUI.contentColor;
        defaultGUIBackgroundColour = GUI.backgroundColor;

        //Reorderable List Callbacks
        #region Creating the Reorderable List and the Callbacks for it
        
        projPatternModList = new ReorderableList(serializedObject, _projectilePatterns, true, true, true, true);

        selectedElement = -1;
        
        projPatternModList.drawElementCallback =
            (Rect rect, int i, bool isActive, bool isFocused) =>
            {
                //Set the selected foldout
                if (projPatternModList.IsSelected(i))
                {
                    selectedElement = i;
                }

               
                //Setting the name of the foldout group
                _patternType = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternType");
                _patternModStruct = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternTypeModifiers");
                _patternIsActive = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternIsActive");
                _patternFoldout = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternIsFoldout");

                Rect foldoutRect = new Rect(rect.x + 30, rect.y + 2, rect.width - 60, rect.height);
                Rect foldoutSelectionRect = new Rect(foldoutRect.x, foldoutRect.y, foldoutRect.width, EditorGUIUtility.singleLineHeight);
                Rect isActiveRect = new Rect(rect.x , rect.y, rect.width, EditorGUIUtility.singleLineHeight + 1);
                Rect fullListRect = new Rect(rect.x - 20, rect.y, rect.width + 25, rect.height);
                Rect removeElementRect = _patternFoldout.boolValue ? new Rect(rect.x + rect.width - 20, rect.y + rect.height/2, 20, 20) : //true
                                                                     new Rect(rect.x + rect.width - 20, rect.y, 20, 20); //false
               
                float trashCanWidthHeight = removeElementRect.width * trashCanScaleOfBox;
                Rect trashCanRect = new Rect(removeElementRect.x + ((removeElementRect.width - trashCanWidthHeight) / 2), 
                                             removeElementRect.y + ((removeElementRect.height - trashCanWidthHeight) / 2),
                                             trashCanWidthHeight,
                                             trashCanWidthHeight);
                
                //Change the colour of the rect based on if the boolean is active or not
                if (_patternIsActive.boolValue == false)
                {
                    EditorGUI.DrawRect(fullListRect, regularPatternColour);
                }
                if(selectedElement == i)
                {
                    EditorGUI.DrawRect(fullListRect, activeColour);
                }

                //Display IsActive boolean
                EditorGUI.PropertyField(isActiveRect, _patternIsActive, new GUIContent(""));

                //Display the Remove Icon beside each element
                bool cancelRestOfDraw = false;
                if(GUI.Button(removeElementRect, ""))
                {
                    projPatternModList.onRemoveCallback.Invoke(projPatternModList);
                    cancelRestOfDraw = true;
                }
                if (cancelRestOfDraw) return;
                
                GUI.DrawTexture(trashCanRect, Resources.Load("TrashIcon") as Texture2D, ScaleMode.ScaleAndCrop);
                

                //Creating the Foldout Header Group
                bool val = _patternFoldout.boolValue;
                _patternFoldout.boolValue = EditorGUI.BeginFoldoutHeaderGroup(foldoutSelectionRect, val, $"Pattern {i + 1}:  {_patternType.GetEnumName<ProjectilePatterns>()}");

                //If the user has expanded this foldout, display the PatternModifier with a horizontal layoutgroup - string and then value
                if (val)
                {
                    foldoutRect.y += EditorGUIUtility.singleLineHeight + 3;
                    Rect patternTypeRect = new Rect(foldoutRect.x, foldoutRect.y, foldoutRect.width, EditorGUIUtility.singleLineHeight);
                    ProjectilePatterns myEnum = (ProjectilePatterns)EditorGUI.EnumPopup(patternTypeRect, new GUIContent("Pattern Type "), projectilePatternWAH.ProjectilePatternList[i].thisPatternType);


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
                        foldoutRect.y += EditorGUIUtility.singleLineHeight + 3;

                        Rect propertyRect = new Rect(foldoutRect.x, foldoutRect.y, foldoutRect.width, EditorGUIUtility.singleLineHeight + 1);

                        _patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue =
                            EditorGUI.FloatField(propertyRect, _patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue,
                                                        _patternModStruct.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue);

                    }
                    #endregion

                    foldoutRect.y += EditorGUIUtility.singleLineHeight;

                    
                }


                EditorGUI.EndFoldoutHeaderGroup();

                GUI.contentColor = defaultGUIContentColour;
                GUI.backgroundColor = defaultGUIBackgroundColour;

            };

        
        projPatternModList.drawElementBackgroundCallback = (Rect rect, int i, bool isActive, bool isFocused) =>
        {
            if (_projectilePatterns.GetArrayElementAtIndex(i) == null) return;

            _patternIsActive = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternIsActive");

            if (_patternIsActive.boolValue == false)
            {
                GUI.contentColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
                GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            }
            
        };
        projPatternModList.elementHeightCallback = (i) =>
        {
            //Check if the contents are folded or not for the sizes of the list elements
            int elementSizeBgCalculation = 0;
            _patternFoldout = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternIsFoldout");
            if (_patternFoldout.boolValue == true)
            {
                _patternModStruct = _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternTypeModifiers");
                elementSizeBgCalculation = 10 + (_patternModStruct.arraySize * 20) + 40;
            }
            else
            {
                elementSizeBgCalculation = (int)EditorGUIUtility.singleLineHeight+2;
            }
            

            return elementSizeBgCalculation;
        };

        projPatternModList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Projectile Pattern Modifiers Stack");
        };

        projPatternModList.onAddCallback = (projPatternModList) =>
        {
            AddToList();
        };

        projPatternModList.onRemoveCallback = (projPatternModList) =>
        {
            TriggerRemoveFromList();
        };


        #endregion
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        triggerRemoveFromList = false;


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
            EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 20, lastRect.width, 10 + (basePAT.arraySize * sizeOfField) + 10), regularPatternColour2);

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



        GUILayout.Space(30);


        selectedElement = -1;
        projPatternModList.DoLayoutList();

        GUI.contentColor = defaultGUIContentColour;
        GUI.backgroundColor = defaultGUIBackgroundColour;
        GUILayout.Space(20);

        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0) // Left mouse button
        {
            DeselectElement();
            currentEvent.Use();
        }

        #region Buttons
        if (GUILayout.Button("Clear All"))
        {
            ClearAll();
        }

        #endregion


        if(triggerRemoveFromList)
        {
            RemoveFromList();
        }
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
        int newIndex = _projectilePatterns.arraySize - 1;
        _projectilePatterns.GetArrayElementAtIndex(newIndex).FindPropertyRelative("thisPatternIsActive").boolValue = true;
        _projectilePatterns.GetArrayElementAtIndex(newIndex).FindPropertyRelative("thisPatternIsFoldout").boolValue = true;
        serializedObject.ApplyModifiedProperties();

        //Get the current information for the modifer we're using (SOME)
        _projectilePatterns.GetArrayElementAtIndex(_projectilePatterns.arraySize - 1).FindPropertyRelative("thisPatternType").enumValueIndex = 0;
        UpdateModifierInfo(ProjectilePatterns.Some, _projectilePatterns.GetArrayElementAtIndex(_projectilePatterns.arraySize-1).FindPropertyRelative("thisPatternTypeModifiers"));
       
    }

    //I need to remove from list this way because it was giving me an error before removing with the trash can icons
    //essentially when we press the icon it would perform RemoveFromList() but then it's still in the process of going through each element and so
    //get errors of the index being out of reach.
    //This way removes errors because we do everything we need to do with the List, THEN we remove it and update the serializedObject
    private void TriggerRemoveFromList()
    {
        triggerRemoveFromList = true;
    }
    private void RemoveFromList()
    {
        if (_projectilePatterns.arraySize <= 0) return;
      
        if (selectedElement < 0) return;

        
        _projectilePatterns.DeleteArrayElementAtIndex(selectedElement);


        EditorUtility.SetDirty(projectilePatternWAH);
        serializedObject.ApplyModifiedProperties();

    }

    private void DeselectElement()
    {
        projPatternModList.Deselect(selectedElement);
        selectedElement = -1;
    }

    private void ClearAll()
    {
        _projectilePatterns.ClearArray();

    }
}
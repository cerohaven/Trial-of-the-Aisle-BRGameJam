using UnityEditor;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

namespace ProjectilePatterns
{

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
        private float dragStartX; //for int fields
        private float dragStartValue; //the initial value when we start dragging the mouse for int fields

        private readonly Color activeColour = new Color(0.1f, 0.35f, 0.55f, 0.5f); //blue
        private readonly Color regularPatternColour = new Color(0.19f, 0.19f, 0.19f); //darker
        private readonly Color regularPatternColour2 = new Color(0.25f, 0.25f, 0.25f); //lighter
        Color defaultGUIContentColour;
        Color defaultGUIBackgroundColour;
        GUIStyle foldoutTitle = new GUIStyle();


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
                    Rect isActiveRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight + 1);
                    Rect fullListRect = new Rect(rect.x - 20, rect.y, rect.width + 25, rect.height);
                    Rect removeElementRect = _patternFoldout.boolValue ? new Rect(rect.x + rect.width - 20, rect.y + rect.height / 2, 20, 20) : //true
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
                    if (selectedElement == i)
                    {
                        EditorGUI.DrawRect(fullListRect, activeColour);
                    }

                    //Display IsActive boolean
                    EditorGUI.PropertyField(isActiveRect, _patternIsActive, new GUIContent(""));

                    //Display the Remove Icon beside each element
                    bool cancelRestOfDraw = false;
                    if (GUI.Button(removeElementRect, ""))
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

                            DrawModifier(j, propertyRect);

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
                if (_projectilePatterns.arraySize == 0) return;
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
                    elementSizeBgCalculation = (int)EditorGUIUtility.singleLineHeight + 2;
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

            CheckIfNeedRefresh();
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            triggerRemoveFromList = false;


            // ----- GUI STYLES ----- //
            #region GUI Styles
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.normal.textColor = Color.white;
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleLeft;

            foldoutTitle.normal.textColor = Color.white;
            foldoutTitle.fontSize = 12;
            foldoutTitle.fontStyle = FontStyle.Bold;
            #endregion

            // ----- TITLE AND LINE ELEMENTS ----- //
            #region Projectile Pattern Modifiers Title and Line
            GUILayout.Space(10);
            GUILayout.Label("Projectile Pattern Modifiers", titleStyle);

            Rect lineRect = GUILayoutUtility.GetLastRect();
            Rect separatorLine = new Rect(lineRect.x, lineRect.y + titleStyle.fontSize, lineRect.width, 2);
            EditorGUI.DrawRect(separatorLine, Color.white);
            GUILayout.Space(10);
            #endregion


            // ----- BASE PATTERN STRUCT ----- //
            #region Base Pattern Struct
            bool baseVal = _baseFoldout.boolValue;

            _baseFoldout.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(baseVal, "Pattern 0: Base");

            Rect lastRect = new Rect();
            lastRect = GUILayoutUtility.GetLastRect();


            if (baseVal)
            {
                bool targetPlayerVal = basePAT.GetArrayElementAtIndex(5).FindPropertyRelative("modValue").floatValue == 1 ? true : false;


                int backgroundRectSizeDecrease = targetPlayerVal == true ? 2 : 1;

                GUILayout.Space(5);
                EditorGUI.DrawRect(new Rect(lastRect.x, lastRect.y + 20, lastRect.width, 10 + ((basePAT.arraySize - backgroundRectSizeDecrease) * sizeOfField) + 10), regularPatternColour2);

                //Adding a "targetPlayer" toggle before the BasePat modifiers are displayed
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(20);
                bool isToggled = EditorGUILayout.Toggle("Target Player", targetPlayerVal, GUILayout.MaxWidth(250));
                basePAT.GetArrayElementAtIndex(5).FindPropertyRelative("modValue").floatValue = isToggled == true ? 1 : 0;
                EditorGUILayout.EndHorizontal();


                //The Rest of the Modifiers in the Base Pattern
                for (int j = 0; j < basePAT.arraySize; j++)
                {
                    if (j == 2 || j == 5) continue; //We don't want to display extra angle to designers, that's only really needed for code
                    if (targetPlayerVal == true && j == 0) continue; //if we are targetting the player and this iteration is on the "Angle", continue so we don't display the Angle

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(20);
                    if (j == 0) GUILayout.Space(20); //Add an indent to the angle to it's under the target player bln

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

            // ----- LIST ----- //
            selectedElement = -1;
            projPatternModList.DoLayoutList();

            GUI.contentColor = defaultGUIContentColour;
            GUI.backgroundColor = defaultGUIBackgroundColour;
            GUILayout.Space(20);


            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0) // Left mouse button
            {
                DeselectElement();
            }

            // ----- BUTTONS ----- //
            #region Buttons
            if (GUILayout.Button("Clear All"))
            {
                ClearAll();
            }
          

            #endregion




            if (triggerRemoveFromList)
            {
                RemoveFromList();
            }

            EditorUtility.SetDirty(projectilePatternWAH);
            serializedObject.ApplyModifiedProperties();




        }

        private void UpdateModifierInfo(ProjectilePatterns pat, SerializedProperty property)
        {

            property.ClearArray();
            PatternTypeMod[] patToUse = null;

            switch (pat)
            {
                case ProjectilePatterns.Some:
                    property.arraySize = somePAT.arraySize;
                    patToUse = SO_ProjectilePattern.Initialize_SomePAT();
                    break;

                case ProjectilePatterns.Spread:
                    property.arraySize = spreadPAT.arraySize;
                    patToUse = SO_ProjectilePattern.Initialize_SpreadPAT();
                    break;

                case ProjectilePatterns.Randomize_Angle:
                    property.arraySize = randomizeAnglePAT.arraySize;
                    patToUse = SO_ProjectilePattern.Initialize_RandomizeAnglePAT();
                    break;

                case ProjectilePatterns.Rapid:
                    property.arraySize = rapidPAT.arraySize;
                    patToUse = SO_ProjectilePattern.Initialize_RapidPAT();
                    break;

                case ProjectilePatterns.Burst:
                    property.arraySize = burstPAT.arraySize;
                    patToUse = SO_ProjectilePattern.Initialize_BurstPAT();
                    break;

                case ProjectilePatterns.Randomize_Spawn_Offset:
                    property.arraySize = randomizeSpawnOffsetPAT.arraySize;
                    patToUse = SO_ProjectilePattern.Initialize_RandomizeSpawnOffsetPAT();
                    break;

            }
            property.arraySize = patToUse.Length;

            for (int j = 0; j < property.arraySize; j++)
            {
                property.GetArrayElementAtIndex(j).FindPropertyRelative("modName").stringValue = patToUse[j].modName;
                property.GetArrayElementAtIndex(j).FindPropertyRelative("modValue").floatValue = patToUse[j].modValue;
                property.GetArrayElementAtIndex(j).FindPropertyRelative("modVariableType").enumValueIndex = (int)patToUse[j].modVariableType;
                property.GetArrayElementAtIndex(j).FindPropertyRelative("modTooltip").stringValue = patToUse[j].modTooltip;
            }
        }
        
        //public void RefreshContents()
        //{
        //    for (int i = 0; i < _projectilePatterns.arraySize; i++)
        //    {
        //        UpdateModifierInfo((ProjectilePatterns)_projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternType").enumValueIndex,
        //            _projectilePatterns.GetArrayElementAtIndex(i).FindPropertyRelative("thisPatternTypeModifiers"));
        //        Debug.Log("Refreshing");
        //    }

        //}
        private void DrawModifier(int modIndex, Rect rect)
        {
            //Creates a visual field in the inspector based on the mod variable type it's set to.
            //Changing the Field Type will help designers more intuitively interact with the UI and offer constraints on what values are available

            float valueOfModifier = 0f;
            string modName = _patternModStruct.GetArrayElementAtIndex(modIndex).FindPropertyRelative("modName").stringValue;
            float modValue = _patternModStruct.GetArrayElementAtIndex(modIndex).FindPropertyRelative("modValue").floatValue;
            string modTooltip = _patternModStruct.GetArrayElementAtIndex(modIndex).FindPropertyRelative("modTooltip").stringValue;
            ModVariableType modType = (ModVariableType)_patternModStruct.GetArrayElementAtIndex(modIndex).FindPropertyRelative("modVariableType").enumValueIndex;
            
            switch (modType)
            {
                case ModVariableType.Float:
                    valueOfModifier = EditorGUI.FloatField(rect, new GUIContent(modName, modTooltip), modValue);
                    break;

                case ModVariableType.Float_Slider_0_1:
                    valueOfModifier = EditorGUI.Slider(rect, new GUIContent(modName, modTooltip), modValue, 0f, 1f);
                    break;

                case ModVariableType.Float_Slider_0_360:
                    valueOfModifier = EditorGUI.Slider(rect, new GUIContent(modName, modTooltip), modValue, 0f, 360f);
                    break;

                case ModVariableType.Int:

                    valueOfModifier = EditorGUI.IntField(rect, new GUIContent(modName, modTooltip), (int)modValue);
                    valueOfModifier = Mathf.Max(valueOfModifier, 0f); //clamp to not go below 0
                    break;

                case ModVariableType.Int_Buttons:
                    GUIStyle style = new GUIStyle(GUI.skin.textField);
                    style.alignment = TextAnchor.MiddleCenter;
                    Event evnt = Event.current;
                    Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
                    Rect minusButtonRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, 20, 20);
                    Rect intFieldRect = new Rect(rect.x + labelRect.width + minusButtonRect.width, rect.y, 100, rect.height);
                    Rect plusButtonRect = new Rect(intFieldRect.x + intFieldRect.width, rect.y, 20, 20);

                    EditorGUI.LabelField(rect, new GUIContent(modName, modTooltip));

                    valueOfModifier = EditorGUI.IntField(intFieldRect, (int)modValue, style);

                    #region Drag Contents


                    //Gets the action we're doing with out mouse on this element
                    int dragControlID = GUIUtility.GetControlID(FocusType.Passive, labelRect);



                    //Have events for the mouse to check if the user is dragging over the Label and to affect the int value
                    switch (evnt.GetTypeForControl(dragControlID))
                    {
                        case EventType.MouseDown:
                            if (labelRect.Contains(evnt.mousePosition)) //if we clicked anywhere in the bounds of our label
                            {
                                GUIUtility.hotControl = dragControlID;
                                dragStartX = evnt.mousePosition.x;
                                dragStartValue = (int)modValue;
                                evnt.Use();
                            }
                            break;

                        case EventType.MouseDrag:
                            if (GUIUtility.hotControl == dragControlID)
                            {
                                float delta = evnt.mousePosition.x - dragStartX; //get diff between start of drag and current
                                int dragAmountSensitivity = (int)dragStartValue + Mathf.RoundToInt(delta * 0.2f);
                                valueOfModifier = dragAmountSensitivity;
                                evnt.Use();
                            }
                            break;

                        case EventType.MouseUp:
                            if (GUIUtility.hotControl == dragControlID)
                            {
                                GUIUtility.hotControl = 0;
                                evnt.Use();
                            }
                            break;
                    }
                    //Change the mouse cursor if it's over the Label Rect
                    if (labelRect.Contains(evnt.mousePosition))
                    {
                        EditorGUIUtility.AddCursorRect(new Rect(0, 0, 500, 500), MouseCursor.SlideArrow);
                    }
                    #endregion

                    if (GUI.Button(minusButtonRect, "-"))
                    {
                        valueOfModifier--;
                        Event.current.Use();
                    }
                    if (GUI.Button(plusButtonRect, "+"))
                    {
                        valueOfModifier++;
                        Event.current.Use();
                    }

                    valueOfModifier = Mathf.Max(valueOfModifier, 0f); //clamp to not go below 0
                    break;

                case ModVariableType.Bool:
                    bool visualValue = modValue == 1 ? true : false;
                    bool blnValue = EditorGUI.Toggle(rect, new GUIContent(modName, modTooltip), visualValue);
                    valueOfModifier = blnValue == true ? 1f : 0f;
                    break;


            }

            _patternModStruct.GetArrayElementAtIndex(modIndex).FindPropertyRelative("modValue").floatValue = valueOfModifier;


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
            UpdateModifierInfo(ProjectilePatterns.Some, _projectilePatterns.GetArrayElementAtIndex(_projectilePatterns.arraySize - 1).FindPropertyRelative("thisPatternTypeModifiers"));

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
            EditorUtility.SetDirty(projectilePatternWAH);
            serializedObject.ApplyModifiedProperties();
        }

        private void CheckIfNeedRefresh()
        {
            bool isUpToDate = projectilePatternWAH.IsPatternStructsUpdated();

            if (isUpToDate) return;

            projectilePatternWAH.RefreshContents();
        }
    }
}
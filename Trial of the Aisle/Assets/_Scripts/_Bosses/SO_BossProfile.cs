using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// I'm creating a tool to easily add more bosses into our game! This will contain all the universal data that
/// every boss shares, like names, attacks used, projectiles thrown, health, and more!
/// </summary>

[CreateAssetMenu(fileName="Boss Profile", menuName = "Boss Scriptable Objects/Boss Profile")]
public class SO_BossProfile : ScriptableObject
{

    [SerializeField] private string b_Name = "Boss Name";
    [SerializeField] private float b_MaxHealth = 250;
    [SerializeField] private Sprite b_BossProfilePicture;
    [SerializeField] private Texture2D b_BossProfileTexture;
    [SerializeField] private Color32 b_BossColourPalette = new Color32(255,255,255,255);

    [Separator()]
    [Title("Projectiles", TextAlignment.Center)]

    [SerializeField] private float b_BaseProjectileThrowSpeed;
    [SerializeField] private float b_BaseTimeBetweenProjectileAttacks;

    [SerializeField] private BossThrowProjectiles[] b_BossThrowProjectiles = new BossThrowProjectiles[1];

    [Separator()]
    [Title("Health", TextAlignment.Center)]

    [SerializeField] private BossHealthIncrements[] b_BossHealthIncrements = new BossHealthIncrements[2];

    [Separator()]
    [Title("Attacks", TextAlignment.Center)]

    [SerializeField] private BossAttacks[] b_BossAttacks = new BossAttacks[2];

    [SerializeField] private int b_maxAttackRepeatTimes = 2;

    [SerializeField] private Ability ability1;
    [SerializeField] private Ability ability2;
    [SerializeField] private Texture2D ability1Texture;
    [SerializeField] private Texture2D ability2Texture;

    public string m_Name { get => b_Name; set => b_Name = value; }
    public Sprite m_BossProfilePicture { get => b_BossProfilePicture;}
    public Texture2D m_BossProfileTexture { get => b_BossProfileTexture; set => b_BossProfileTexture = value; }
    public Texture2D Ability1Texture { get => ability1Texture; set => ability1Texture = value; }
    public Texture2D Ability2Texture { get => ability2Texture; set => ability2Texture = value; }
    public Ability Ability1 { get => ability1; set => ability1 = value; }
    public Ability Ability2 { get => ability2; set => ability2 = value; }
    public BossHealthIncrements[] B_BossHealthIncrements { get => b_BossHealthIncrements; set => b_BossHealthIncrements = value; }


    //Properties


}
#region 2D Arrays
[System.Serializable]
public class BossHealthIncrements
{
    //This class is in charge of creating an array of percentages where the boss' attacks will change
    //Eg. when the boss is down to last 25% of health, do something.
    [Range(0, 100)]
    public float healthPercent = 50;

    [Range(0, 2)]
    public float projectileSpeedMultiplier = 1;

    [Range(0, 2)]
    public float timeBetweenProjectileAttacksMultiplier = 1;
    
    [Tooltip("When the boss reaches this threshold, we can run custom code for a unique event to possibly trigger an animation, noise, etc.")]
    public SO_BossHealthIncrementEvent healthIncrementEvent;
}

[System.Serializable]
public class BossThrowProjectiles
{
    public GameObject projectilePrefab;
    [Range(0,1)]
    public float percentChanceToUse = 0.5f;
}

[System.Serializable]
public class BossAttacks
{
    public string attackName;

    [Range(0, 1)]
    public float percentChanceToUse = 0.5f;

    [Tooltip("Any special condition for this attack to play. Conditions like 'the boss must be at ___ health' or 'the player used their ability ___ imes'")]
    public SO_BossAttackCondition attackCondition;
}
#endregion

[CustomEditor(typeof(SO_BossProfile))]
[CanEditMultipleObjects]
public class SO_BossProfileEditor : Editor
{
    //SerializedProperties
    private SerializedProperty b_Name;
    private SerializedProperty b_MaxHealth;
    private SerializedProperty b_BossProfilePicture;
    private SerializedProperty b_BossColourPalette;

    private SerializedProperty b_BaseProjectileThrowSpeed;
    private SerializedProperty b_BaseTimeBetweenProjectileAttacks;

    private SerializedProperty b_BossThrowProjectiles;

    private SerializedProperty b_BossHealthIncrements;

    private SerializedProperty b_BossAttacks;

    private SerializedProperty b_maxAttackRepeatTimes;

    private SerializedProperty ability1;
    private SerializedProperty ability2;

    private SerializedProperty ability1Texture;
    private SerializedProperty ability2Texture;

    private void OnEnable()
    {
        b_Name = serializedObject.FindProperty("b_Name");
        b_MaxHealth = serializedObject.FindProperty("b_MaxHealth");
        b_BossProfilePicture = serializedObject.FindProperty("b_BossProfilePicture");
        b_BossColourPalette = serializedObject.FindProperty("b_BossColourPalette");

        b_BaseProjectileThrowSpeed = serializedObject.FindProperty("b_BaseProjectileThrowSpeed");
        b_BaseTimeBetweenProjectileAttacks = serializedObject.FindProperty("b_BaseTimeBetweenProjectileAttacks");

        b_BossThrowProjectiles = serializedObject.FindProperty("b_BossThrowProjectiles");

        b_BossHealthIncrements = serializedObject.FindProperty("b_BossHealthIncrements");

        b_BossAttacks = serializedObject.FindProperty("b_BossAttacks");

        b_maxAttackRepeatTimes = serializedObject.FindProperty("b_maxAttackRepeatTimes");

        ability1 = serializedObject.FindProperty("ability1");
        ability2 = serializedObject.FindProperty("ability2");

        ability1Texture = serializedObject.FindProperty("ability1Texture");
        ability2Texture = serializedObject.FindProperty("ability2Texture");

    }

    public override void OnInspectorGUI()
    {
       
        SO_BossProfile boss = (SO_BossProfile)target;

        //Updates the inspector if we make a change
        serializedObject.UpdateIfRequiredOrScript();

        if (boss == null) return;
        if (boss.IsDestroyed()) return;





        //Boss Information Title
        GUILayout.Space(20f);

        GUIStyle bossNameStyle = new GUIStyle();
        bossNameStyle.fontSize = 40;
        bossNameStyle.normal.textColor = b_BossColourPalette.colorValue;
        bossNameStyle.fontStyle = FontStyle.Bold;
        bossNameStyle.alignment = TextAnchor.MiddleCenter;
        string bossName = $"{b_Name.stringValue}";
        EditorGUILayout.LabelField(bossName, bossNameStyle);

        GUILayout.Space(30f);





        // BOSS INFORMATION -- NAME -- HEALTH -- PFP -- COLOUR //
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        //Create Texture
        Rect text = new Rect(20, 80, 100, 100);
        EditorGUI.DrawRect(text, Color.black);

        if (boss.m_BossProfilePicture != null)
        {
            //Get the rect of the sprite
            Rect spriteRect = boss.m_BossProfilePicture.rect;


            //Make a texture from a set pixel range in a sprite
            if (boss.m_BossProfilePicture.texture.isReadable == false)
            {
               
                boss.m_BossProfileTexture = boss.m_BossProfilePicture.texture;
            }
            else
            {
                //Now get the pixels from the texture2D where the sprite is
                Texture2D newTex = boss.m_BossProfilePicture.texture;
                var tests = newTex.GetPixels((int)spriteRect.position.x, (int)spriteRect.position.y, (int)spriteRect.width, (int)spriteRect.height);

                boss.m_BossProfileTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
                boss.m_BossProfileTexture.SetPixels(tests);
                boss.m_BossProfileTexture.Apply();

            }

            GUI.DrawTexture(text, boss.m_BossProfileTexture);
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        EditorStyles.label.alignment = TextAnchor.MiddleRight;
        EditorGUILayout.PropertyField(b_Name, new GUIContent("Name  "));
        EditorGUILayout.PropertyField(b_MaxHealth, new GUIContent("Health  "));
        EditorGUILayout.PropertyField(b_BossProfilePicture, new GUIContent("Profile Pic  "));
        EditorGUILayout.PropertyField(b_BossColourPalette, new GUIContent("Colour Palette "));
        EditorStyles.label.alignment = TextAnchor.MiddleLeft;


        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.Space(40f);





        // PROJECTILE INFORMATION //
        GUILayout.Space(30f);
        EditorGUILayout.PropertyField(b_BaseProjectileThrowSpeed, new GUIContent("Base Throw Speed "));
        EditorGUILayout.PropertyField(b_BaseTimeBetweenProjectileAttacks, new GUIContent("Base Attack Delay "));
        EditorGUILayout.PropertyField(b_BossThrowProjectiles, new GUIContent("Boss Throw Projectiles "));





        // HEALTH INFORMATION //
        GUILayout.Space(30f);
        EditorGUILayout.PropertyField(b_BossHealthIncrements, new GUIContent("Boss Health Increments"));
        
        Rect healthRect = GUILayoutUtility.GetLastRect();
        EditorGUI.DrawRect(new Rect(70, healthRect.y + healthRect.height + 20, healthRect.width - 60, 30), b_BossColourPalette.colorValue);

        float widthOfHealthBar = healthRect.width - 60;
        Rect healthBossPfp = new Rect(20, healthRect.y + healthRect.height + 10, 50, 50);
        GUI.DrawTexture(healthBossPfp, boss.m_BossProfileTexture);



        GUILayout.Space(120f);



        // ABILITITES INFORMATION //

        EditorGUILayout.PropertyField(b_BossAttacks, new GUIContent("Boss Attacks "));

        EditorGUILayout.PropertyField(b_maxAttackRepeatTimes, new GUIContent("Max Repeat Times"));


        GUILayout.Space(140f);


        Rect lastRect = GUILayoutUtility.GetLastRect();



        //These 2 rects determine the position of the texture2D in the inspector
        //for the second rect, I check to see the width of the inspector and set the position based on it
        Rect textAbility1 = new Rect(75, lastRect.y + 20, 100, 100);

        float x2 = Screen.width < 560 ? (Screen.width / 2 - 50) + 120 : 250 + 75 + 30;

        Rect textAbility2 = new Rect(x2, lastRect.y + 20, 100, 100);



        //Create Texture
        #region Ability 1 Texture2D

        EditorGUI.DrawRect(textAbility1, Color.black);

        if (boss.Ability1 != null)
        {
            //Get the rect of the sprite
            Rect spriteRect = boss.Ability1.abilityIcon.rect;
            

            //Make a texture from a set pixel range in a sprite. 
            //I need to do this since I want to get a sliced sprite from a texture2D, but you can only read the pixels
            //if the texture2D is readable. If it is, then I can get and set the pixels
            if(boss.Ability1.abilityIcon.texture.isReadable == false)
            {
                boss.Ability1Texture = boss.Ability1.abilityIcon.texture;
            }
            else
            {
                //Now get the pixels from the texture2D where the sprite is
                Texture2D newTex = boss.Ability1.abilityIcon.texture;
                var tests = newTex.GetPixels((int)spriteRect.position.x, (int)spriteRect.position.y, (int)spriteRect.width, (int)spriteRect.height);

                boss.Ability1Texture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
                boss.Ability1Texture.SetPixels(tests);
                boss.Ability1Texture.Apply();

            }


            GUI.DrawTexture(textAbility1, boss.Ability1Texture);
        }
        #endregion

        #region Ability 2 Texture2D
        

        EditorGUI.DrawRect(textAbility2, Color.black);

        if (boss.Ability2 != null)
        {
            //Get the rect of the sprite
            Rect spriteRect = boss.Ability2.abilityIcon.rect;


            //Make a texture from a set pixel range in a sprite. 
            //I need to do this since I want to get a sliced sprite from a texture2D, but you can only read the pixels
            //if the texture2D is readable. If it is, then I can get and set the pixels
            if (boss.Ability2.abilityIcon.texture.isReadable == false)
            {
                boss.Ability2Texture = boss.Ability2.abilityIcon.texture;
            }
            else
            {
                //Now get the pixels from the texture2D where the sprite is
                Texture2D newTex = boss.Ability2.abilityIcon.texture;
                var tests = newTex.GetPixels((int)spriteRect.position.x, (int)spriteRect.position.y, (int)spriteRect.width, (int)spriteRect.height);

                boss.Ability2Texture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
                boss.Ability2Texture.SetPixels(tests);
                boss.Ability2Texture.Apply();

            }


            GUI.DrawTexture(textAbility2, boss.Ability2Texture);
        }
        #endregion

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Ability 1 ", GUILayout.MaxWidth(250));
        GUILayout.Space(30f);
        GUILayout.Label("Ability 2 ", GUILayout.MaxWidth(250));
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(ability1, new GUIContent(""), GUILayout.MaxWidth(250));
        GUILayout.Space(30f);
        EditorGUILayout.PropertyField(ability2, new GUIContent(""), GUILayout.MaxWidth(250));
        EditorGUILayout.EndHorizontal();



        GUILayout.Space(50f);

        //If we made a change, apply it
        serializedObject.ApplyModifiedProperties();




        //I have to put this code after we apply the modified properties or else we get an error since the array of the property doesn't update on time
        for (int i = 0; i < b_BossHealthIncrements.arraySize; i++)
        {

            //Loop through each health percent and draw a rect

            float f = boss.B_BossHealthIncrements[Mathf.Clamp(i, 0, b_BossHealthIncrements.arraySize - 1)].healthPercent;

            boss.B_BossHealthIncrements[i].healthPercent = Mathf.Clamp(boss.B_BossHealthIncrements[i].healthPercent, 0, 100);
            Color c = b_BossColourPalette.colorValue;
            
            //Drawing the Ticks for the health percentages
            EditorGUI.DrawRect(new Rect((f * widthOfHealthBar / 100) + 70, healthRect.y + healthRect.height + 10, 8, 50), new Color(c.r - 0.3f, c.g - 0.3f, c.b - 0.3f));

            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = 16;
            textStyle.normal.textColor = Color.white;

            //Drawing the Number text for the health percentages
            EditorGUI.LabelField(new Rect((f * widthOfHealthBar / 100)+60, healthRect.y + healthRect.height + 62, 10, 50), $"{f.ToString("00")}%", textStyle);
        }


    }

    //Change the Icon preview for the Alien!
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        SO_BossProfile boss = (SO_BossProfile)target;

        if (boss == null || boss.m_BossProfileTexture == null)
            return null;

        // The texture we create must be a supported format: ARGB32, RGBA32, RGB24
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        EditorUtility.CopySerialized(boss.m_BossProfileTexture, tex);

        return tex;
    }
}
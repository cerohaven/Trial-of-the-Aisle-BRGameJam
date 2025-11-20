using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using ProjectilePatterns;
/// <summary>
/// I'm creating a tool to easily add more bosses into our game! This will contain all the universal data that
/// every boss shares, like names, attacks used, projectiles thrown, health, and more!
/// </summary>

[CreateAssetMenu(fileName="Boss Profile", menuName = "Boss Scriptable Objects/Boss Profile")]
public class SO_BossProfile : ScriptableObject
{
    [SerializeField] public string b_Name = "Boss Name";
    [SerializeField] private int b_MaxHealth = 250;
    [SerializeField] private Sprite b_BossProfilePicture;
    [SerializeField] private Texture2D b_BossProfileTexture;
    [SerializeField] private Color32 b_BossColourPalette = new Color32(255,255,255,255);

    [Separator()]
    [Title("Throw Projectiles", TextAlignment.Center)]
    [SerializeField] private BossThrowProjectiles[] b_BossThrowProjectiles = new BossThrowProjectiles[1];

    [Separator()]
    [Title("Boss Phases", TextAlignment.Center)]
    
    [Range(0, 30)]
    [SerializeField] private float b_BaseProjectileThrowSpeed;
    [SerializeField] private SO_ProjectilePattern b_BaseProjectileThrowPattern;

    [Range(0, 2)]
    [SerializeField] private float b_BaseTimeBetweenProjectileAttacks;
    [SerializeField] private float b_BaseMoveSpeed;

    [Tooltip("When the boss reaches this percentage of health, we can change the behaviours of attacks" )]
    [SerializeField] private BossHealthIncrements[] b_BossPhases = new BossHealthIncrements[1];
    [SerializeField] private SO_EntityDeathEvent_Boss b_BossDeathEvent;

    [Separator()]
    [Title("Attacks", TextAlignment.Center)]

    [SerializeField] private BossAttacks[] b_BossAttacks = new BossAttacks[2];

    [SerializeField] private int b_maxAttackRepeatTimes = 2;

    [SerializeField] private Ability ability1;
    [SerializeField] private Ability ability2;
    [SerializeField] private Texture2D ability1Texture;
    [SerializeField] private Texture2D ability2Texture;

    [SerializeField] private Sprite postBattleCanvasUI;
    private Texture2D postBattleCanvasTexture;

    public string m_Name { get => b_Name;}
    public int B_MaxHealth { get => b_MaxHealth; }
    public Sprite m_BossProfilePicture { get => b_BossProfilePicture;}
    public Texture2D m_BossProfileTexture { get => b_BossProfileTexture; set => b_BossProfileTexture = value; }

    public BossHealthIncrements[] B_BossPhases { get => b_BossPhases; }
    public BossAttacks[] B_BossAttacks { get => b_BossAttacks; }
    public float B_BaseProjectileThrowSpeed { get => b_BaseProjectileThrowSpeed; }
    public float B_BaseTimeBetweenProjectileAttacks { get => b_BaseTimeBetweenProjectileAttacks; }
    public float B_BaseMoveSpeed { get => b_BaseMoveSpeed; set => b_BaseMoveSpeed = value; }
    public BossThrowProjectiles[] B_BossThrowProjectiles { get => b_BossThrowProjectiles;}

    public Texture2D Ability1Texture { get => ability1Texture; set => ability1Texture = value; }
    public Texture2D Ability2Texture { get => ability2Texture; set => ability2Texture = value; }
    public Ability Ability1 { get => ability1; }
    public Ability Ability2 { get => ability2; }

    
    public Sprite PostBattleCanvasUI { get => postBattleCanvasUI;}
    public Texture2D PostBattleCanvasTexture { get => postBattleCanvasTexture; set => postBattleCanvasTexture = value; }
    public Color32 B_BossColourPalette { get => b_BossColourPalette;}
    public SO_EntityDeathEvent_Boss B_BossDeathEvent { get => b_BossDeathEvent; }
    public SO_ProjectilePattern B_BaseProjectileThrowPattern { get => b_BaseProjectileThrowPattern; set => b_BaseProjectileThrowPattern = value; }






    //Properties


}
#region 2D Arrays
[System.Serializable]
public class BossHealthIncrements
{
    //This class is in charge of creating an array of percentages where the boss' attacks will change
    //Eg. when the boss is down to last 25% of health, do something.
    [HideInInspector] public string phaseName = "Phase ";

    [Range(0, 100)]
    public float healthPercent = 50;

    [Range(0, 30)]
    public float throwSpeed = 1;


    [Range(0, 2)]
    public float attackDelay = 1;

    //public Vector2 minMaxProjectilesToThrow;
    public SO_ProjectilePattern projectilePattern;


    [Tooltip("When the boss reaches this threshold, we can run custom code for a unique event to possibly trigger an animation, noise, etc.")]
    public SO_EntityHealthEventBase phaseEvent;

    //[Header("Simultaneous Projectiles Thrown")]
    //[Range(1, 3)]
    //[Tooltip("If we want the projectiles thrown in this phase to be simultaneous, like a tri shot or split shot")]
    //public int projectilesThrownSimultaneously = 1;

    //[Range(0, 0.5f)]
    //[Tooltip("If we want a delay between the simultaneous shots for a staggered effect, or just launch them all at the same time")]
    //public float simultaneousDelay = 0;

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
    public SO_BossAttackConditionBase attackCondition;
}
#endregion


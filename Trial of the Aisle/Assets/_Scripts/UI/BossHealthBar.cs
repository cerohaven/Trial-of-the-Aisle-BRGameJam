using FMODUnity;
using NodeCanvas.Framework;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private Blackboard bossBlackboard;
    private SO_BossProfile bossProfile;

    [Header("Boss Hit Effect")]
    [SerializeField] private GameObject bossHitEffect;

    [Header("Heal Particle Effect")]
    [SerializeField] private GameObject healEffect;

    //References
    private SO_HealthAdjustments uiManager;
    private RectTransform bossRectTransform;

    private Image bossBarImg;

    //Variables
    [SerializeField] private float blueBarIncreaseSpeed;
    private float bossBarScaleX;
    private float bossHealth;
    private float maxHealth;
    private float maxBossBarScaleX;
    private bool canStartIncrease = false;  //called from the "BossIntroAnimation.cs" script to 
                                    //start increasing the boss bar at the beginning
    //Boss Damaged Colour Change
    [Header("Boss Hurt Colour Change")]
    [SerializeField] private float colourFlickerTime;
    [SerializeField] private float colourFlickerAmount;
    [SerializeField] private SpriteRenderer bossSr;
    private Color color = Color.white;
    private IEnumerator colourCoroutine;
    FMOD.Studio.EventInstance Boss_SFX_increaseHP;
    FMOD.Studio.EventInstance UI_BossHurt;
    FMOD.Studio.EventInstance SFX_heal;

    public bool CanStartIncrease { get => canStartIncrease; set => canStartIncrease = value; }

    private void Awake()
    {
        bossProfile = bossBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
        bossBarImg = GetComponent<Image>();

        bossRectTransform = GetComponent<RectTransform>();
        uiManager = GameObject.FindObjectOfType<SO_HealthAdjustments>();
        Boss_SFX_increaseHP = RuntimeManager.CreateInstance("event:/UI/GUI/HealthBarRaise");
        UI_BossHurt = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/Hurt");
        SFX_heal = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/Heal");
    }
 
    private void Start()
    {
        //bossBarImg.color = bossProfile.B_BossColourPalette;

        //maxBossBarScaleX = bossRectTransform.sizeDelta.x;
        //maxHealth = bossProfile.B_MaxHealth;
        //bossHealth = maxHealth;
        //bossBarScaleX = 0;
        //bossRectTransform.sizeDelta = new Vector2(0, bossRectTransform.sizeDelta.y);
    }

    private void Update()
    {
        //if (!uiManager.FinishedBossIntro && canStartIncrease)
            //IncreaseBossBar();

    }


    //IEnumerator BossColourFlicker()
    //{
    //    for(int i = 0; i < colourFlickerAmount; i++)
    //    {
    //        color.r = 1;
    //        color.g = 0;
    //        color.b = 0;
    //        bossSr.color = new Color(color.r, color.g, color.b, 1);

    //        yield return new WaitForSeconds(colourFlickerTime/2);
    //        color.r = 1;
    //        color.g = 1;
    //        color.b = 1;
    //        bossSr.color = new Color(color.r, color.g, color.b, 1);

    //        yield return new WaitForSeconds(colourFlickerTime / 2);
    //    }

    //    bossSr.color = Color.white;
    //    yield return null;
    //}
 

    //public void BossChangeHealth(float _bossChangedHealth, Vector2 _upDir)
    //{
    //    if (!gameObject.activeSelf) return;
    //    //Check to see if healing or damage is being passed
    //    bool isDamage = _bossChangedHealth < 0;

    //    if (isDamage)
    //    {
    //        colourCoroutine = BossColourFlicker();
    //        StopCoroutine(colourCoroutine);
    //        StartCoroutine(colourCoroutine);


    //        bool bossIsDefeated = bossHealth <= 0.25f;
    //        if (bossIsDefeated)
    //        {
    //            GameManager.Instance.EventSender.BossIsDefeatedSend();

    //            //disables the blue boss bar
    //            gameObject.SetActive(false);

    //        }
    //        UI_BossHurt.start();
    //        //AudioManager.instance.Play("ui_bossHurt");

    //        GameObject hit = Instantiate(bossHitEffect, bossBlackboard.transform);
    //        hit.transform.up = _upDir;


    //    }
    //    else
    //    {

    //        GameObject temp = Instantiate(healEffect, bossBlackboard.gameObject.transform.position, Quaternion.identity);
    //        temp.transform.localScale = Vector2.one * 3;
    //        SFX_heal.start();
    //        //AudioManager.instance.Play("heal");
    //    }

    //}

}

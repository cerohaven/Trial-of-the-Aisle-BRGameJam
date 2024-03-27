using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedSender;
    [SerializeField] private Blackboard bossBlackboard;
    private SO_BossProfile bossProfile;

    [Header("Boss Hit Effect")]
    [SerializeField] private GameObject bossHitEffect;

    [Header("Heal Particle Effect")]
    [SerializeField] private GameObject healEffect;

    //References
    private UIManager uiManager;
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

    public bool CanStartIncrease { get => canStartIncrease; set => canStartIncrease = value; }

    private void Awake()
    {
        bossProfile = bossBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
        bossBarImg = GetComponent<Image>();

        bossRectTransform = GetComponent<RectTransform>();
        uiManager = GameObject.FindObjectOfType<UIManager>();
    }
 
    private void Start()
    {
        bossBarImg.color = bossProfile.B_BossColourPalette;

        maxBossBarScaleX = bossRectTransform.sizeDelta.x;
        maxHealth = bossProfile.B_MaxHealth;
        bossHealth = maxHealth;
        bossBarScaleX = 0;
        bossRectTransform.sizeDelta = new Vector2(0, bossRectTransform.sizeDelta.y);
    }

    private void Update()
    {
        if (!uiManager.FinishedBossIntro && canStartIncrease)
            IncreaseBossBar();

    }


    IEnumerator BossColourFlicker()
    {
        for(int i = 0; i < colourFlickerAmount; i++)
        {
            color.r = 1;
            color.g = 0;
            color.b = 0;
            bossSr.color = new Color(color.r, color.g, color.b, 1);

            yield return new WaitForSeconds(colourFlickerTime/2);
            color.r = 1;
            color.g = 1;
            color.b = 1;
            bossSr.color = new Color(color.r, color.g, color.b, 1);

            yield return new WaitForSeconds(colourFlickerTime / 2);
        }

        bossSr.color = Color.white;
        yield return null;
    }
    private void IncreaseBossBar()
    {
        //Increase the blue boss bar until full. (Mini animation that plays at beginning)
        bossBarScaleX += blueBarIncreaseSpeed * Time.deltaTime;

        bossRectTransform.sizeDelta = new Vector2(bossBarScaleX, bossRectTransform.sizeDelta.y);

        //when the boss bar loads up all the way to being full, then commence the battle and keep it's scale at max
        if (bossRectTransform.sizeDelta.x >= maxBossBarScaleX)
        {
            uiManager.FinishedBossIntro = true;
            
            
            AudioManager.Instance.Stop("ui_bossBarIncrease");

           
        }
    }

    public void BossChangeHealth(float _bossChangedHealth, Vector2 _upDir)
    {
        //Check to see if healing or damage is being passed
        bool isDamage = _bossChangedHealth < 0;

        if (isDamage)
        {
            colourCoroutine = BossColourFlicker();
            StopCoroutine(colourCoroutine);
            StartCoroutine(colourCoroutine);

            UpdateHealthBar(_bossChangedHealth);

            bool bossIsDefeated = bossHealth <= 0.25f;
            if (bossIsDefeated)
            {
                bossDefeatedSender.BossIsDefeatedSend();

                //disables the blue boss bar
                gameObject.SetActive(false);

            }
            AudioManager.Instance.Play("ui_bossHurt");

            GameObject hit = Instantiate(bossHitEffect, bossBlackboard.transform);
            hit.transform.up = _upDir;

            bossBlackboard.SetVariableValue("bossPhase", SetBossPhase());


        }
        else
        {
            UpdateHealthBar(_bossChangedHealth);
            GameObject temp = Instantiate(healEffect, bossBlackboard.gameObject.transform.position, Quaternion.identity);
            temp.transform.localScale = Vector2.one * 3;
            AudioManager.Instance.Play("heal");
        }

    }

    private void UpdateHealthBar(float _health)
    {
        bossHealth += _health;
        bossHealth = Mathf.Clamp(bossHealth, 0, maxHealth);
        bossRectTransform.sizeDelta = new Vector2((bossHealth / maxHealth) * 100, bossRectTransform.sizeDelta.y);


        bossBlackboard.SetVariableValue("bossHealth", bossHealth);
    }

    private int SetBossPhase()
    {
        
        //Get percent health from max health
        float healthPercent = (bossHealth / maxHealth * 100);

        //loop through the phases

        //if lower than one, set that phase

        int bossPhases = bossProfile.B_BossPhases.Length;
        //For loop to see if it is > the current increment or not
        for (int i = bossPhases - 1; i >= 0; i--)
        {

            //eg. if increment is 65%, then we want to check if <25, then <50, then <75, then <100
            //Yes to <75 (i = 1), so we set the pill speed to be projectileSpeed[i]

            if (healthPercent > bossProfile.B_BossPhases[i].healthPercent)
                continue;
           

            //Play the special Health Increment event if this phase has one and it hasn't already played
            if (bossProfile.B_BossPhases[i].phaseEvent != null)
            {
                if(!bossProfile.B_BossPhases[i].phaseEvent.EventPlayed)
                    bossProfile.B_BossPhases[i].phaseEvent.OnHealthChange();
            }

            return i + 1;

           
        }

        return 0;
    }
}

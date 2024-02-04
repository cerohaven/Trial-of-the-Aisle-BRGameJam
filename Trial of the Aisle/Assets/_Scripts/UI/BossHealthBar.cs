using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedSender;
    [SerializeField] private Blackboard bossBlackboard;

    [Header("Boss Hit Effect")]
    [SerializeField] private GameObject bossHitEffect;

    [Header("Heal Particle Effect")]
    [SerializeField] private GameObject healEffect;

    //References
    private UIManager uiManager;
    private RectTransform bossRectTransform;

    //Variables
    [SerializeField] private float blueBarIncreaseSpeed;
    private float bossBarScaleX;
    private float bossHealth;
    private float maxHealth;
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
        bossRectTransform = GetComponent<RectTransform>();
        uiManager = GameObject.FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        maxHealth = bossRectTransform.sizeDelta.x;
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
        if (bossRectTransform.sizeDelta.x >= maxHealth)
        {
            uiManager.FinishedBossIntro = true;
            bossHealth = maxHealth;
            
            AudioManager.instance.Stop("ui_bossBarIncrease");
            bossBlackboard.SetVariableValue("bossHealth", bossHealth);
            bossBlackboard.SetVariableValue("bossMaxHealth", maxHealth);
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
            AudioManager.instance.Play("ui_bossHurt");

            GameObject hit = Instantiate(bossHitEffect, bossBlackboard.transform);
            hit.transform.up = _upDir;
            
        }
        else
        {
            UpdateHealthBar(_bossChangedHealth);
            GameObject temp = Instantiate(healEffect, bossBlackboard.gameObject.transform.position, Quaternion.identity);
            temp.transform.localScale = Vector2.one * 3;
            AudioManager.instance.Play("heal");
        }

    }

    private void UpdateHealthBar(float _health)
    {
        bossHealth += _health;
        bossHealth = Mathf.Clamp(bossHealth, 0, maxHealth);
        bossRectTransform.sizeDelta = new Vector2(bossHealth, bossRectTransform.sizeDelta.y);
        bossBlackboard.SetVariableValue("bossHealth", bossHealth);
    }
}

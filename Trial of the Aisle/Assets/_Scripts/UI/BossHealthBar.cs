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

    //References
    private UIManager uiManager;
    private RectTransform bossRectTransform;

    //Variables
    [SerializeField] private float blueBarIncreaseSpeed;
    private float bossBarScaleX;
    private float bossHealth;
    private float maxHealth;

    private void Awake()
    {
        bossRectTransform = GetComponent<RectTransform>();
        uiManager = GameObject.FindObjectOfType<UIManager>();
    }

    private void Start()
    {

        AudioManager.instance.Play("ui_bossBarIncrease");
        maxHealth = bossRectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (!uiManager.FinishedBossIntro)
            IncreaseBossBar();
    }

    private void IncreaseBossBar()
    {
        //Increase the blue boss bar until full. (Mini animation that plays at beginning)
        bossBarScaleX = bossRectTransform.sizeDelta.x;
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

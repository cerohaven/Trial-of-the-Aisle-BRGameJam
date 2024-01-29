using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedSender;

    //References
    private UIManager uiManager;
    private RectTransform bossRectTransform;

    //Variables
    [SerializeField] private float blueBarIncreaseSpeed;
    private float bossBarScaleX;
    

    private void Awake()
    {
        bossRectTransform = GetComponent<RectTransform>();
        uiManager = GameObject.FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        AudioManager.instance.Play("ui_bossBarIncrease");
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
        if (bossRectTransform.sizeDelta.x >= 100)
        {
            uiManager.FinishedBossIntro = true;

            AudioManager.instance.Stop("ui_bossBarIncrease");
        }
    }

    public void BossDamage(float bossHealthDecrement)
    {
        AudioManager.instance.Play("ui_bossHurt");

        //Decrement boss health when hit. Called from the UIManager Class
        bossRectTransform.sizeDelta += new Vector2(bossHealthDecrement, 0);

        float health = bossRectTransform.sizeDelta.x;
        bossDefeatedSender.UpdateBossHealthEventSend(health);

        bool bossIsDefeated = health <= 0.25f;
        if (bossIsDefeated)
        {
            bossDefeatedSender.BossIsDefeatedSend();

            //disables the blue boss bar
            gameObject.SetActive(false);
            
        }
    }

}

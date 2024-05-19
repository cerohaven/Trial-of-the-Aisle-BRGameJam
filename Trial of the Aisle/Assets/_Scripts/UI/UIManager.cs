using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    //References
    [SerializeField] private SO_AdjustHealth adjustHealth;

    [Separator()]
    [SerializeField] private float xSmallHealthAdjustment;
    [SerializeField] private float smallHealthAdjustment;
    [SerializeField] private float mediumHealthAdjustment;
    [SerializeField] private float largeHealthAdjustment;
    [SerializeField] private float xLargeHealthAdjustment;


    private PlayerHealthBar playerHealthBar;
    private BossHealthBar bossHealthBar;

    //Variables
    public bool bossBarVisible;

    private bool finishedBossIntro = false;
    public bool FinishedBossIntro
    {
        get => finishedBossIntro;
        set => finishedBossIntro = value;
    }

    private void Awake()
    {
        playerHealthBar = GameObject.FindObjectOfType<PlayerHealthBar>();
        bossHealthBar = GameObject.FindObjectOfType<BossHealthBar>();

        //Once event is called, run these methods
        adjustHealth.changePlayerHealthEvent.AddListener(AdjustPlayerHealth);
        adjustHealth.changeBossHealthEvent.AddListener(AdjustBossHealth);
    }

    private void AdjustPlayerHealth(ChangeHealth changeHealthState, HealthType healthType)
    {
        float healthAdjustment = GetHealthValue(changeHealthState, healthType);

        playerHealthBar.PlayerChangeHealth(healthAdjustment);
    }
    private void AdjustBossHealth(ChangeHealth changeHealthState, HealthType healthType, Vector2 projectileUpDir)
    {
        float healthAdjustment = GetHealthValue(changeHealthState, healthType);
        bossHealthBar.BossChangeHealth(healthAdjustment, projectileUpDir);
    }


    //This turns the input parameters of the health into a number that is sent to the Player or Boss Health Bar
    private float GetHealthValue(ChangeHealth _changeHealthState, HealthType _healthType)
    {
        float healthToReturn = 0;
        int healthType = 1;

        if(_healthType == HealthType.Damage)
        {
            healthType = -1;
        }
        else
        {
            healthType = 1;
        }

        switch(_changeHealthState)
        {
            case ChangeHealth.X_Small_Health:
                healthToReturn = xSmallHealthAdjustment * healthType;
                break;
            case ChangeHealth.Small_Health:
                healthToReturn = smallHealthAdjustment * healthType;
                break;
            case ChangeHealth.Medium_Health:
                healthToReturn = mediumHealthAdjustment * healthType;
                break;
            case ChangeHealth.Large_Health:
                healthToReturn = largeHealthAdjustment * healthType;
                break;
            case ChangeHealth.X_Large_Health:
                healthToReturn = xLargeHealthAdjustment * healthType;
                break;



        }
        return healthToReturn;

    }

    private float GetSpeedValue(ChangeSpeed _changeSpeedState, SpeedType _speedType)
    {
        float speedToReturn = 0;
        int speedType = 1;

        if (_speedType == SpeedType.Debuff)
        {
            speedType = -1;
        }
        else
        {
            speedType = 1;
        }

        switch (_changeSpeedState)
        {
            case ChangeSpeed.X_Small_Speed:
                speedToReturn = xSmallHealthAdjustment * speedType;
                break;
            case ChangeSpeed.Small_Speed:
                speedToReturn = smallHealthAdjustment * speedType;
                break;
            case ChangeSpeed.Medium_Speed:
                speedToReturn = mediumHealthAdjustment * speedType;
                break;
            case ChangeSpeed.Large_Speed:
                speedToReturn = largeHealthAdjustment * speedType;
                break;
            case ChangeSpeed.X_Large_Speed:
                speedToReturn = xLargeHealthAdjustment * speedType;
                break;



        }
        return speedToReturn;

    }
}

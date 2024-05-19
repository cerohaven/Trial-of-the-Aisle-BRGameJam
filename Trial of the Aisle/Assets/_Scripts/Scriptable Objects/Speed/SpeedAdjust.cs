using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedAdjust : MonoBehaviour
{
    [SerializeField] private SO_AdjustSpeed adjustSpeed;

    [Separator()]
    [SerializeField] private float xSmallSpeedAdjustment;
    [SerializeField] private float smallSpeedAdjustment;
    [SerializeField] private float mediumSpeedAdjustment;
    [SerializeField] private float largeSpeedAdjustment;
    [SerializeField] private float xLargeSpeedAdjustment;

    private void AdjustPlayerSpeed(ChangeSpeed changeSpeedState, SpeedType speedType)
    {
        float healthAdjustment = GetSpeedValue(changeSpeedState, speedType);

        //playerHealthBar.PlayerChangeHealth(healthAdjustment);
    }
    private void AdjustBossSpeed(ChangeSpeed changeSpeedState, SpeedType speedType, Vector2 projectileUpDir)
    {
        float healthAdjustment = GetSpeedValue(changeSpeedState, speedType);
        //bossHealthBar.BossChangeHealth(healthAdjustment, projectileUpDir);
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
                speedToReturn = xSmallSpeedAdjustment * speedType;
                break;
            case ChangeSpeed.Small_Speed:
                speedToReturn = smallSpeedAdjustment * speedType;
                break;
            case ChangeSpeed.Medium_Speed:
                speedToReturn = mediumSpeedAdjustment * speedType;
                break;
            case ChangeSpeed.Large_Speed:
                speedToReturn = largeSpeedAdjustment * speedType;
                break;
            case ChangeSpeed.X_Large_Speed:
                speedToReturn = xLargeSpeedAdjustment * speedType;
                break;



        }
        return speedToReturn;

    }

}

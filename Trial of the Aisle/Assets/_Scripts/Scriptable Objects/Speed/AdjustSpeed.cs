using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSpeed : MonoBehaviour
{
    [SerializeField] private SO_AdjustSpeed adjustSpeed;

    [Separator()]
    [SerializeField] private float xSmallSpeedAdjustment;
    [SerializeField] private float smallSpeedAdjustment;
    [SerializeField] private float mediumSpeedAdjustment;
    [SerializeField] private float largeSpeedAdjustment;
    [SerializeField] private float xLargeSpeedAdjustment;

    private PlayerController playerController;
    private SO_BossProfile bossProfile;

    private void Awake()
    {
        playerController = GameObject.FindObjectOfType<PlayerController>();
        bossProfile = GetComponent<SO_BossProfile>();

        //Once event is called, run these methods
        adjustSpeed.changePlayerSpeedEvent.AddListener(AdjustPlayerSpeed);
        adjustSpeed.changeBossSpeedEvent.AddListener(AdjustBossSpeed);
    }

    private void AdjustPlayerSpeed(ChangeSpeed changeSpeedState, SpeedType speedType)
    {
        float speedAdjustment = GetSpeedValue(changeSpeedState, speedType);
        playerController.MoveSpeed += speedAdjustment;
    }
    private void AdjustBossSpeed(ChangeSpeed changeSpeedState, SpeedType speedType, Vector2 projectileUpDir)
    {
        float speedAdjustment = GetSpeedValue(changeSpeedState, speedType);
        bossProfile.B_BaseMoveSpeed += speedAdjustment;
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

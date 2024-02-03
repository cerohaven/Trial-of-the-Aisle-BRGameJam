using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroAnimation : MonoBehaviour
{
    [SerializeField] private float timeOnScreen;
    [SerializeField] private Animator[] HUDanimators;


    [Header("Player and Boss References")]
    [SerializeField] private PlayerController pc;
    [SerializeField] private Blackboard bossBlackboard;

    private void Start()
    {
        Invoke("EndOfAnimation", timeOnScreen);
        FreezePlayerMovement();

        BackgroundMusicSelect.Instance.PlayBGMusic();
    }

    private void EndOfAnimation()
    {
        RemoveHUD();
        UnFreezePlayerMovement();
    }

    private void RemoveHUD()
    {
        bossBlackboard.SetVariableValue("canStartBossFight", true);
        for (int i = 0; i < HUDanimators.Length; i++)
        {
            HUDanimators[i].SetTrigger("Reverse");
        }
    }

    private void FreezePlayerMovement()
    {
        pc.CanMove = false;
    }

    private void UnFreezePlayerMovement()
    {
        pc.CanMove = true;
    }
}

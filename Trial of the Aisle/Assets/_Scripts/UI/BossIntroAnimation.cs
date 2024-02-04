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
    [SerializeField] private BossHealthBar bossHealthBar;

    private void Start()
    {
        Invoke("EndOfAnimation", timeOnScreen);
        FreezePlayerMovement();

        BackgroundMusicSelect.Instance.PlayBGMusic();
    }

    private void EndOfAnimation()
    {
        RemoveHUD();
        
    }

    private void RemoveHUD()
    {
        Invoke("StartFight", 1);
        for (int i = 0; i < HUDanimators.Length; i++)
        {
            HUDanimators[i].SetTrigger("Reverse");
        }
    }

    private void StartFight()
    {
        bossHealthBar.CanStartIncrease = true;
        AudioManager.instance.Play("ui_bossBarIncrease");

        bossBlackboard.SetVariableValue("canStartBossFight", true);
        
        UnFreezePlayerMovement();
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

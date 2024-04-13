using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

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
        StartCoroutine(playsound());
        Invoke("EndOfAnimation", timeOnScreen);
        FreezePlayerMovement();
    }

    private void EndOfAnimation()
    {
        RemoveHUD();
        
    }

    private void RemoveHUD()
    {
        Invoke("StartFight", 1.5f);
        for (int i = 0; i < HUDanimators.Length; i++)
        {
            HUDanimators[i].SetTrigger("Reverse");
        }
    }

    private void StartFight()
    {
        bossHealthBar.CanStartIncrease = true;
        RuntimeManager.PlayOneShot("event:/UI/GUI/HealthBarRaise");

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

    IEnumerator playsound()
    {
        yield return new WaitForSeconds(.7f);
        RuntimeManager.PlayOneShot("event:/Dialogue/Introductions/PK_Intro");
    }
}

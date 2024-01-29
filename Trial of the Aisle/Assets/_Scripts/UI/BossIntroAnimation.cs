using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroAnimation : MonoBehaviour
{
    [SerializeField] private float timeOnScreen;
    [SerializeField] private Animator[] HUDanimators;
    [SerializeField] private Blackboard bossBlackboard;

    private void Start()
    {
        Invoke("RemoveHUD", timeOnScreen);

        BackgroundMusicSelect.Instance.PlayBGMusic();
    }

    private void RemoveHUD()
    {
        bossBlackboard.SetVariableValue("canStartBossFight", true);
        for (int i = 0; i < HUDanimators.Length; i++)
        {
            HUDanimators[i].SetTrigger("Reverse");
        }

    }
}

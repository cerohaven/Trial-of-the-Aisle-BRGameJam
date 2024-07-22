using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BossIntroAnimation : MonoBehaviour
{
    public SO_BossProfile SO_BP;
    [SerializeField] private float timeOnScreen;
    [SerializeField] private Animator[] HUDanimators;

    [Header("Player and Boss References")]
    [SerializeField] private PlayerController pc;
    [SerializeField] private InitializeBoss initializeBoss;

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
        RuntimeManager.PlayOneShot("event:/UI/GUI/HealthBarRaise");

        if (initializeBoss == null)
        {
            initializeBoss = GameObject.FindObjectOfType<InitializeBoss>();
        }

        initializeBoss.StartBossBattle();
        
        UnFreezePlayerMovement();
    }

    private void FreezePlayerMovement()
    {
        GameManager.Instance.CanMove = false;
    }

    private void UnFreezePlayerMovement()
    {
        GameManager.Instance.CanMove = true;
    }

    IEnumerator playsound()
    {
        yield return new WaitForSeconds(.7f);

        if (SO_BP != null)
        {
            switch (SO_BP.b_Name)
            {
                case "The Pain Killer":
                    RuntimeManager.PlayOneShot("event:/Dialogue/Introductions/PK_Intro");
                    break;
                case "Alexander the Grape":
                    RuntimeManager.PlayOneShot("event:/Dialogue/Introductions/AtG_Intro");
                    break;
                case "Dairy Dominator":
                    RuntimeManager.PlayOneShot("event:/Dialogue/Introductions/DD_Intro");
                    break;
                case "Quickus Pickus Upis":
                    RuntimeManager.PlayOneShot("event:/Dialogue/Introductions/QPU_Intro");
                    break;
                default:
                    Debug.LogWarning("Default case reached with boss name: " + SO_BP.b_Name);
                    break;
            }
        }
        else
        {
            Debug.LogError("SO_BP is null.");
        }
    }
}

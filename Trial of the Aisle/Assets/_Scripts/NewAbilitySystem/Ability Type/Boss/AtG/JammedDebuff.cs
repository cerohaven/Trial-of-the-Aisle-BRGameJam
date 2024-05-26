using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammedDebuff : MonoBehaviour
{
    [SerializeField] private float bossSpeedJammed = 2.0f;
    public GameObject hitEffect;
    Blackboard bossBlackboard;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("Adjusting 1");
            bossBlackboard = collision.gameObject.GetComponent<Blackboard>();
            if (bossBlackboard != null)
            {
                bossBlackboard.SetVariableValue("bossSpeed", bossSpeedJammed);
                Invoke("ResetBossSpeed", 2.5f);
            }
            //adjustSpeed.ChangeBossSpeedEventSend(changeSpeedAmount, SpeedType.Debuff, transform.up);
            CinemachineShake.Instance.ShakeCamera();
        }

        if (collision.gameObject.CompareTag("Pill"))
        {
            Destroy(gameObject);
        }

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.5f);
        gameObject.SetActive(false);
        
    }
    
    void ResetBossSpeed()
    {
        //Reset the boss' speed variable
        SO_BossProfile bp = bossBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
        bossBlackboard.SetVariableValue("bossSpeed", bp.B_BaseMoveSpeed);
        Destroy(gameObject);
    }
}

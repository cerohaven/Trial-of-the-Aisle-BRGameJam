using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammedDebuff : MonoBehaviour
{
    public ChangeSpeed changeSpeedAmount; // The enum value specifying the amount of health to adjust
    [SerializeField] private SO_AdjustSpeed adjustSpeed;
    public GameObject hitEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            adjustSpeed.ChangeBossSpeedEventSend(changeSpeedAmount, SpeedType.Debuff, transform.up);
            CinemachineShake.Instance.ShakeCamera();
        }

        if (collision.gameObject.CompareTag("Pill"))
        {
            Destroy(gameObject);
        }

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.5f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ChangeHealth changeHealthAmount; // The enum value specifying the amount of health to adjust
    [SerializeField] private SO_AdjustHealth adjustHealth;
    public GameObject hitEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {


        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        
        Destroy(effect, 1.5f);
        Destroy(gameObject);

        if(collision.gameObject.CompareTag("Boss"))
        {
            adjustHealth.ChangeBossHealthEventSend(changeHealthAmount, HealthType.Damage, transform.up);
            CinemachineShake.Instance.ShakeCamera();
        }

        if (collision.gameObject.CompareTag("Pill"))
        {
            Destroy(gameObject);
        }
    }
}

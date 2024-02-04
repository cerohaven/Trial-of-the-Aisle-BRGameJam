using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private SO_AdjustHealth adjustHealth;
    public GameObject hitEffect;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        //Debug.Log("Bullet Collided");
        Destroy(effect, 1.5f);
        Destroy(gameObject);

        if(collision.gameObject.CompareTag("Boss"))
        {
            adjustHealth.ChangeBossHealthEventSend(ChangeHealth.Small_Health, HealthType.Damage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ChangeHealth changeHealthAmount; // The enum value specifying the amount of health to adjust
    public GameObject hitEffect;
    public EntityHealth entityHealth;

    private void Awake()
    {
        entityHealth = GameManager.Instance.BossTransform.GetComponent<EntityHealth>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            entityHealth.DamageEntity(changeHealthAmount);
            CinemachineShake.Instance.ShakeCamera();
        }

        if (collision.gameObject.CompareTag("Pill"))
        {
            Destroy(gameObject);
        }

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1.5f);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ChangeHealth changeHealthAmount; // The enum value specifying the amount of health to adjust
    [SerializeField] private SO_AdjustHealth adjustHealth;
    public GameObject hitEffect;
    private Vector2 direction;

    public Vector2 Direction { get => direction; set => direction = value; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log("Bullet Collided");
        Destroy(effect, 1.5f);
        Destroy(gameObject);

        if(collision.gameObject.CompareTag("Boss"))
        {
            adjustHealth.ChangeBossHealthEventSend(changeHealthAmount, HealthType.Damage, direction);
            CinemachineShake.Instance.ShakeCamera();
        }
    }
}

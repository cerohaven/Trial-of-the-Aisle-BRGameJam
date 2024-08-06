using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsManager : MonoBehaviour
{
    private EntityHealth entityHealth;

    private void Awake()
    {
        entityHealth = GetComponent<EntityHealth>();    
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            //hurt the player if they collide with the boss
            entityHealth.DamageEntity(ChangeHealth.Small_Health);

        }
    }
}

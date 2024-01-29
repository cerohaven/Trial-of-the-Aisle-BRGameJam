using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsManager : MonoBehaviour
{
    [SerializeField] private SO_AdjustHealth adjustHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            //hurt the player if they collide with the boss
            adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Small_Health, HealthType.Damage);

        }
    }
}

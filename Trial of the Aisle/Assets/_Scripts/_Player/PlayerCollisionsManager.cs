using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsManager : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            //hurt the player if they collide with the boss
            GameManager.Instance.EventSender.ChangePlayerHealthEventSend(ChangeHealth.Small_Health, HealthType.Damage);

        }
    }
}

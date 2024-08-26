using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamSlowDownPlayer : MonoBehaviour
{
    [SerializeField] private float disappearTime = 15f;
    [SerializeField] private bool isHot = false;

    private bool startToDisappear = false;
    private SpriteRenderer sr;
    private float fade;
    private EntityHealth playerHealth;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        fade = 1;
        Invoke("Disappear", disappearTime);
    }

    
    void Update()
    {
        if (!startToDisappear) return;
        fade -= Time.deltaTime;

        float a = fade;

        sr.color = new Color(1, 1, 1, a);

        if(a <= 0.05f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !startToDisappear)
        {
            
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.MoveSpeed /= 2;
            pc.DodgeSpeed /= 3;

            if(isHot)
            {
                if (playerHealth == null) playerHealth = collision.gameObject.GetComponent<EntityHealth>();
                InvokeRepeating(nameof(DamagePlayer), 0, 0.5f);
            }
            
        }
    }

    private void DamagePlayer()
    {
        playerHealth.DamageEntity(ChangeHealth.Small_Health);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController pc = collision.GetComponent<PlayerController>();
            pc.MoveSpeed = pc.RegularMoveSpeed;
            pc.DodgeSpeed = pc.RegularDodgeSpeed;

            if (isHot)
            {
                CancelInvoke(nameof(DamagePlayer));
            }
        }
    }
    private void Disappear()
    {
        startToDisappear = true;
    }
}

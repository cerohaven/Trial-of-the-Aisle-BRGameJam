using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionThrow : MonoBehaviour
{

    //References
    [SerializeField] private SO_AdjustHealth adjustHealth;

    //Components
    private Rigidbody2D rb;

    //Variables
    [SerializeField] private float speed;
    private Vector2 bossPos;
    private Vector2 playerPos;
    private Vector2 midpoint;
    private GameObject boss;

    private float distanceFromMidPoint;
    private float angle = 0;
    private float startAngle;
    
    //Properties
    public Vector2 BossPos { get => bossPos; set => bossPos = value; }
    public Vector2 PlayerPos { get => playerPos; set => playerPos = value; }
    public GameObject Boss { get => boss; set => boss = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
    }
    private void Start()
    {
        midpoint = Vector2.Lerp(playerPos, bossPos, 0.5f);
        distanceFromMidPoint = Vector2.Distance(midpoint,bossPos);

        angle = Mathf.Atan2(midpoint .y - bossPos.y, midpoint.x - bossPos.x);
        angle += 180;
        startAngle = angle;
    }

    // Update is called once per frame
    void Update()
    {
        //dir = (playerPos - (Vector2)transform.position).normalized;
        float x = midpoint.x + Mathf.Cos(angle) * distanceFromMidPoint;
        float y = midpoint.y + Mathf.Sin(angle) * distanceFromMidPoint;

        rb.MovePosition(new Vector2(x,y));

        angle += speed * Time.deltaTime / distanceFromMidPoint;

        rb.MoveRotation(rb.rotation + angle * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collide with player
        if(collision.gameObject.CompareTag("Player"))
        {
            adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Medium_Health, HealthType.Damage);
            CinemachineShake.Instance.ShakeCamera();
        }

        if(collision.gameObject.CompareTag("Boss"))
        {
            if(angle > startAngle + 1)
            {
                LeanTween.scale(boss, Vector3.one * 1.1f, 0.1f).setEaseInOutQuad().setOnComplete(Testing);
               
            }
           
        }

        if(collision.gameObject.CompareTag("Pill"))
        {
            Projectile proj = collision.gameObject.GetComponent<Projectile>();
            if (proj.WhoThrew != WhoThrew.Player) return;
            adjustHealth.ChangeBossHealthEventSend(ChangeHealth.Medium_Health, HealthType.Damage, Vector2.up);
            CinemachineShake.Instance.ShakeCamera();
            Destroy(gameObject);

        }
    }

    private void Testing()
    {
        LeanTween.scale(boss, Vector3.one, 0.1f);
        if(gameObject != null) Destroy(gameObject);
    }

}

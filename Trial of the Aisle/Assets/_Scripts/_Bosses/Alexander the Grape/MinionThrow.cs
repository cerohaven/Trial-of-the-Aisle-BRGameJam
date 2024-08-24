using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionThrow : MonoBehaviour
{

    //Components
    private Rigidbody2D rb;

    //Variables
    [SerializeField] private float speed;
    [SerializeField] private GameObject jamSpilledGO;
    [SerializeField] private float spawnJamTrailRate;

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
    private EntityHealth entityHealth;

    private FMOD.Studio.EventInstance GrapeHitInstance;

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

        InvokeRepeating(nameof(SpawnJamSpilled), spawnJamTrailRate, spawnJamTrailRate);

    }



    
    void Update()
    {
        //dir = (playerPos - (Vector2)transform.position).normalized;
        float x = midpoint.x + Mathf.Cos(angle) * distanceFromMidPoint;
        float y = midpoint.y + Mathf.Sin(angle) * distanceFromMidPoint;

        rb.MovePosition(new Vector2(x,y));

        angle += speed * Time.deltaTime / distanceFromMidPoint;

        rb.MoveRotation(rb.rotation + angle * speed * Time.deltaTime);
    }

    private void SpawnJamSpilled()
    {
        Instantiate(jamSpilledGO, transform.position, new Quaternion(0, 0, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collide with player
        if(collision.gameObject.CompareTag("Player"))
        {
            entityHealth = collision.gameObject.GetComponent<EntityHealth>();
            entityHealth.DamageEntity(ChangeHealth.Medium_Health);
            CinemachineShake.Instance.ShakeCamera();
        }

        if(collision.gameObject.CompareTag("Boss"))
        {
            if(angle > startAngle + 1)
            {
                LeanTween.scale(boss, Vector3.one * 1.1f, 0.1f).setEaseInOutQuad().setOnComplete(Testing);
                GrapeHitInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_AtG/Grape_Hit");
                GrapeHitInstance.start();
            }
           
        }

        if(collision.gameObject.CompareTag("Pill"))
        {
            Projectile proj = collision.gameObject.GetComponent<Projectile>();
            if (proj.WhoThrew != WhoThrew.Player) return;

            entityHealth = collision.gameObject.GetComponent<EntityHealth>();
            entityHealth.DamageEntity(ChangeHealth.Medium_Health);

            CinemachineShake.Instance.ShakeCamera();
            Destroy(gameObject);

        }
    }



    private void Testing()
    {
        if (gameObject == null) return;
        
        LeanTween.scale(boss, Vector3.one, 0.1f);

        gameObject.SetActive(false);
        Invoke(nameof(DestroyMinion), 1);
       
    }



    private void DestroyMinion()
    {
        CancelInvoke(nameof(SpawnJamSpilled));
        Destroy(gameObject);
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionThrow : MonoBehaviour
{
    private enum MinionThrowType
    {
        Circular,
        Straight
    }

    private MinionThrowType throwType;
    delegate void ThrowTypeFunctionStart();
    delegate void ThrowTypeFunctionUpdate();
    ThrowTypeFunctionStart throwFunctionStart;
    ThrowTypeFunctionStart throwFunctionUpdate;

    //Components
    private Rigidbody2D rb;

    //Variables
    [SerializeField] private float speed;

    [Space]
    [Header("Projectile Jam")]
    [SerializeField] private GameObject jamSpilledGO;
    [SerializeField] private float spawnJamTrailRate;
    [SerializeField] private GameObject dangerLineRednererGO;


    [Space]
    [Header("Minion Time Alive")]
    [SerializeField] private float maxTimeAlive;
    [SerializeField] private float timeReductionPerWallHit;
    private float currentTimeAlive;

    private Vector2 bossPos;
    private Vector2 playerPos;
    private Vector2 midpoint;
    private Vector2 dirToPlayer;
    private GameObject boss;
    private LineRenderer lr;

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

        SetRandomThrowType();
        currentTimeAlive = maxTimeAlive;
    }

    private void SetRandomThrowType()
    {
        throwType = (MinionThrowType)Random.Range(0, 2);

        if (throwType == MinionThrowType.Circular)
        {
            throwFunctionStart = CircularThrowStart;
            throwFunctionUpdate = CircularThrowUpdate;  
        }
        else
        {
            throwFunctionStart = StraightThrowStart;
            throwFunctionUpdate = StraightThrowUpdate;
        }
    }


    private void Start()
    {
        throwFunctionStart();

        InvokeRepeating(nameof(SpawnJamSpilled), spawnJamTrailRate, spawnJamTrailRate);
    }

    private void CircularThrowStart()
    {
        midpoint = Vector2.Lerp(playerPos, bossPos, 0.5f);
        distanceFromMidPoint = Vector2.Distance(midpoint, bossPos);

        angle = Mathf.Atan2(midpoint.y - bossPos.y, midpoint.x - bossPos.x);
        angle += 180;
        startAngle = angle;

        
    }

    private void StraightThrowStart()
    {
        dirToPlayer = playerPos - bossPos;
        dirToPlayer.Normalize();
        rb.velocity = dirToPlayer * speed;
        GameObject go = Instantiate(dangerLineRednererGO, transform.position, Quaternion.identity);
        lr = go.GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, playerPos);
    }

    
    void Update()
    {
        throwFunctionUpdate();
    }

    private void CircularThrowUpdate()
    {
        //dir = (playerPos - (Vector2)transform.position).normalized;
        float x = midpoint.x + Mathf.Cos(angle) * distanceFromMidPoint;
        float y = midpoint.y + Mathf.Sin(angle) * distanceFromMidPoint;

        rb.MovePosition(new Vector2(x, y));

        angle += speed * Time.deltaTime / distanceFromMidPoint;

        rb.MoveRotation(rb.rotation + angle * speed * Time.deltaTime);

        
        if (angle > startAngle + 7)
        {
            DestroyMinion();
        }
    }

    private void StraightThrowUpdate()
    {
        LayerMask mask = LayerMask.GetMask("Wall");
        RaycastHit2D raycastHitUp = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.up * Mathf.Sign(rb.velocity.y), mask);
        RaycastHit2D raycastHitRight = Physics2D.Linecast(transform.position, (Vector2)transform.position + Vector2.right * Mathf.Sign(rb.velocity.x), mask);

        if (raycastHitUp)
        {
             rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            currentTimeAlive -= timeReductionPerWallHit;
        }
        if (raycastHitRight)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            currentTimeAlive -= timeReductionPerWallHit;
        }

        //Time Reduction
        currentTimeAlive -= Time.deltaTime;

        if(currentTimeAlive <= 0)
        {
            rb.drag = 4f;
            CancelInvoke(nameof(SpawnJamSpilled));
        }

        if(rb.velocity.magnitude <= 0.1f)
        {
            DestroyMinion();
        }
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
        else if(collision.gameObject.CompareTag("Boss"))
        {
            if(angle > startAngle + 1)
            {
                CancelInvoke(nameof(SpawnJamSpilled));
                LeanTween.scale(boss, Vector3.one * 1.1f, 0.1f).setEaseInOutQuad().setOnComplete(Testing);
                GrapeHitInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_AtG/Grape_Hit");
                GrapeHitInstance.start();
            }
           
        }
        else if(collision.gameObject.CompareTag("Pill"))
        {
            Projectile proj = collision.gameObject.GetComponent<Projectile>();
            if (proj.WhoThrew != WhoThrew.Player) return;

            entityHealth = boss.GetComponent<EntityHealth>();
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

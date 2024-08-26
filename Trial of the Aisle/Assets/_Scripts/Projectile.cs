using FMODUnity;
using NodeCanvas.Framework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum WhoThrew
{
    Boss,
    Null,
    Player
}

public class Projectile : MonoBehaviour
{
    ///The base class for any throwable projectile, either from the boss or from the player

    // -- REFERNCES -- //
    [SerializeField] protected ChangeHealth damageDealt;
    protected InteractableObject_Projectile interactableProjectile;

    // -- COMPONENTS -- //
    [SerializeField] protected GameObject hitParticles; //On collision, spawn particles
    private Collider2D projectileCollider;
    
    [Separator()]
    [Header("Colour of Outline")]
    [SerializeField] protected SpriteRenderer outlineRenderer;

    [SerializeField] protected Color bossOutlineColour;
    [SerializeField] protected Color playerOutlineColour;
    [SerializeField] protected Color nullOutlineColour;
   

    protected Rigidbody2D rb;

    // -- PROJECTILE VARIABLES -- //

    protected Vector2 travelDir;

    protected float travelSpeed;

    protected bool canBePickedUp;

    private bool shouldReturn;

    private bool shouldDestroyOnWall;

    protected Transform targetThrown; //Get the Thrown target. If it was thrown by the player
                                      //it shouldn't have any effect if it accidentally hits the player.

    protected WhoThrew whoThrew = WhoThrew.Boss;

    //Properties
    public Transform TargetThrown { get => targetThrown; set => targetThrown = value; }
    public WhoThrew WhoThrew { get => whoThrew; set => whoThrew = value; }
    public bool ShouldDestroyOnWall { get => shouldDestroyOnWall; set => shouldDestroyOnWall = value; }

    public virtual void InitializeProjectile(Vector2 _direction, float _speed, Transform _targetThrown, WhoThrew _whoThrew)
    {
        travelDir = _direction;
        transform.up = travelDir;
        travelSpeed = _speed;
        targetThrown = _targetThrown;
        whoThrew = _whoThrew;

        ChangeProjectileOutline();
        EnableTrigger(true);
        MoveProjectile();

    }

    private void MoveProjectile()
    {
        rb.velocity = travelDir * travelSpeed;
    }


    //On Awake, get the projectile's rigidbody
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        interactableProjectile = GetComponent<InteractableObject_Projectile>();
        projectileCollider = GetComponent<Collider2D>();
        RuntimeManager.PlayOneShot("event:/SFX/Bosses/General/ThrowProjectile");
        
    }

    
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if(rb.velocity.magnitude < 1.5f && whoThrew == WhoThrew.Boss)
        {
            SetProjectileNull();

        }

    }

    /// <summary>
    /// Removes the who threw and neutralizes the projectile so it can be picked up.
    /// </summary>
    public void SetProjectileNull()
    {
        canBePickedUp = true;
        targetThrown = null;
        whoThrew = WhoThrew.Null;
        ChangeProjectileOutline();
        interactableProjectile.SetInteractable(true);
        EnableTrigger(false);
    }


    public void ChangeProjectileOutline()
    {
        switch(whoThrew)
        {
            case WhoThrew.Player:
                outlineRenderer.color = playerOutlineColour;
                break;

            case WhoThrew.Boss:
                outlineRenderer.color = bossOutlineColour;
                break;

            case WhoThrew.Null:
                outlineRenderer.color = nullOutlineColour;
                break;
        }
    }


    public void EnableCollider(bool enable)
    {
        //enable collider
        projectileCollider.enabled = enable;
    }

    //Projectile should be a trigger Always except for when WhoThew == null
    protected void EnableTrigger(bool enable)
    {
        projectileCollider.isTrigger = enable;
    }


    #region Enabling Drag
    //Setting the drag of the projectile so it can slow down or not slow down
    public IEnumerator EnableDragCoroutine(float minTime, float maxTime, float dragAmount = 2.35f)
    {
        float time = Random.Range(minTime, maxTime);

        yield return new WaitForSeconds(time);

        if (shouldReturn) yield break;
       
        SetDrag(dragAmount);
    }
    public void RemoveDrag()
    {
        rb.drag = 0f;
    }
    private void SetDrag(float dragAmount)
    {
        rb.drag = dragAmount;
    }
    #endregion



    protected void OnDisable()
    {
        shouldReturn = true;
        StopCoroutine(nameof(EnableDragCoroutine));
    }


    
    protected void InstantiateHitParticles()
    {
        Instantiate(hitParticles, transform.position, Quaternion.identity);
    }
    protected void DestroyGameObject()
    {
        shouldReturn = true;
        StopCoroutine(nameof(EnableDragCoroutine));
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       

        EntityHealth entityHealth = collision.gameObject.GetComponent<EntityHealth>();
       
        //If we hit an entity with Health
        if (entityHealth != null)
        {
            bool hitBoss = collision.gameObject.CompareTag("Boss") && whoThrew == WhoThrew.Player;
            bool hitPlayer = collision.gameObject.CompareTag("Player") && whoThrew == WhoThrew.Boss;
            bool hitRandomEntity = whoThrew != WhoThrew.Null && !hitBoss && !hitPlayer;

            if (hitBoss)
            {
                //send to reduce scale of boss bar
                entityHealth.DamageEntity(damageDealt);
                CinemachineShake.Instance.ShakeCamera();
                DestroyGameObject();
                InstantiateHitParticles();

                return;
            }
            else if (hitPlayer)
            {
                entityHealth.DamageEntity(damageDealt);
                DestroyGameObject();
                InstantiateHitParticles();
                return;
            }
            else if (hitRandomEntity)
            {
                if (collision.gameObject.CompareTag("Boss") && whoThrew == WhoThrew.Boss) return;

                entityHealth.DamageEntity(damageDealt);
                DestroyGameObject();
                InstantiateHitParticles();
                return;
            }
        }
        else
        {
            Projectile otherProjectile = collision.gameObject.GetComponent<Projectile>();
            bool hitWall = collision.gameObject.CompareTag("Walls");
            bool hitFeta = collision.gameObject.CompareTag("Feta");

            if (hitWall)
            {
                InstantiateHitParticles();
                rb.velocity = Vector2.zero;

                if (whoThrew == WhoThrew.Player || shouldDestroyOnWall)
                {
                    DestroyGameObject();
                    return;
                }


            }
            else if (hitFeta)
            {
                DestroyGameObject();
                InstantiateHitParticles();
                return;
            }
            else
            {
                //Debug.Log(collision.gameObject.name);
            }

        }

    }

}

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
   

    /// </summary>
    protected Rigidbody2D rb;

    // -- PROJECTILE VARIABLES -- //

    protected Vector2 travelDir;

    protected float travelSpeed;

    protected bool canBePickedUp;

    private bool shouldReturn;

    protected Transform targetThrown; //Get the Thrown target. If it was thrown by the player
                                      //it shouldn't have any effect if it accidentally hits the player.

    protected WhoThrew whoThrew = WhoThrew.Boss;

    //Properties
    public Transform TargetThrown { get => targetThrown; set => targetThrown = value; }
    public WhoThrew WhoThrew { get => whoThrew; set => whoThrew = value; }
  
    public virtual void InitializeProjectile(Vector2 _direction, float _speed, Transform _targetThrown, WhoThrew _whoThrew)
    {
        travelDir = _direction;
        transform.up = travelDir;
        travelSpeed = _speed;
        targetThrown = _targetThrown;
        whoThrew = _whoThrew;

        ChangeProjectileOutline();

        MoveProjectile();


    }

    //On Awake, get the projectile's rigidbody
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        interactableProjectile = GetComponent<InteractableObject_Projectile>();
        projectileCollider = GetComponent<Collider2D>();
        RuntimeManager.PlayOneShot("event:/SFX/Bosses/General/ThrowProjectile");

    }

    //On Start, apply a velocity to the projectile in the direction and speed given.
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


    private void MoveProjectile()
    {
        rb.velocity = travelDir * travelSpeed;
    }



    //Called From other scripts to not collide with other projectiles
    public IEnumerator IgnoreProjectilesCoroutine(bool _ignore, float _delay)
    {
        yield return new WaitForSeconds(_delay);

        if (shouldReturn) yield break;

        IgnoreProjectileLayer(_ignore);
    }
    
    private void IgnoreProjectileLayer(bool ignoreCollision)
    {
        
        projectileCollider.isTrigger = ignoreCollision;

    }
  

    //Ignoring collisions with certain layers. Called From the boss' scripts when instantiating a projectile
    public void IgnoreBossCollision(bool _ignore, Collider2D collider)
    {
        Physics2D.IgnoreCollision(projectileCollider, collider, _ignore);

    }



    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //We don't want projectiles on the ground to spawn hit particles if the player or boss bumps into them
        if(whoThrew != WhoThrew.Null)
        {
            InstantiateHitParticles();
        }
        

        EntityHealth entityHealth = collision.gameObject.GetComponent<EntityHealth>();

                //If we hit an entity with Health
        if (entityHealth != null)
        {
            bool hitBoss = collision.gameObject.CompareTag("Boss") && whoThrew == WhoThrew.Player;
            bool hitPlayer = collision.gameObject.CompareTag("Player") && whoThrew == WhoThrew.Boss;
            bool hitRandomEntity = whoThrew != WhoThrew.Null && ! !hitBoss && !hitPlayer;

            if (hitBoss)
            {
                //send to reduce scale of boss bar
                entityHealth.DamageEntity(damageDealt);
                CinemachineShake.Instance.ShakeCamera();
                DestroyGameObject();
                return;
            }
            else if(hitPlayer)
            {
                entityHealth.DamageEntity(damageDealt);
                DestroyGameObject();
                return;
            }
            else if(hitRandomEntity)
            {
                entityHealth.DamageEntity(damageDealt);
                DestroyGameObject();
                return;
            }
        }
        else
        {
            bool hitWall = collision.gameObject.CompareTag("Walls");
            bool hitOtherProjectile = collision.gameObject.CompareTag("Pill") && collision.gameObject.GetComponent<Projectile>().whoThrew != whoThrew;
            bool hitFeta = collision.gameObject.CompareTag("Feta");

            if (hitWall)
            {
                rb.velocity = Vector2.zero;

                if (whoThrew == WhoThrew.Player)
                {
                    DestroyGameObject();
                    return;
                }

            }
            else if ( hitOtherProjectile || hitFeta)
            {
                DestroyGameObject();
                return;
            }
            else
            {
                Debug.Log(collision.gameObject.name);
            }

        }
        

    }

    protected void InstantiateHitParticles()
    {
        Instantiate(hitParticles, transform.position, Quaternion.identity);
    }
    protected void DestroyGameObject()
    {
        shouldReturn = true;
        StopCoroutine(nameof(EnableDragCoroutine));
        StopCoroutine(nameof(IgnoreProjectilesCoroutine));
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        bool hitWall = collision.gameObject.CompareTag("Walls");
        if (hitWall)
        {
            rb.velocity = Vector2.zero;

            if (whoThrew == WhoThrew.Player)
            {
                Destroy(gameObject);

            }

        }
    }

}

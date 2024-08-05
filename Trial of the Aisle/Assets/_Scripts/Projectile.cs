using FMODUnity;
using NodeCanvas.Framework;
using System.Collections;
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
    [SerializeField] protected Color neutralOutlineColour;


    /// </summary>
    protected Rigidbody2D rb;

    // -- PROJECTILE VARIABLES -- //

    protected Vector2 travelDir;

    protected float travelSpeed;

    protected bool canBePickedUp;

    

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

        //Set the colour of the outline
        if(whoThrew == WhoThrew.Player)
        {
              outlineRenderer.color = playerOutlineColour;
        }
        else if (whoThrew == WhoThrew.Boss)
        {
            outlineRenderer.color = bossOutlineColour;
        }
       
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
            canBePickedUp = true;
            targetThrown = null;
            whoThrew = WhoThrew.Null; 
            outlineRenderer.color = neutralOutlineColour;
            interactableProjectile.SetInteractable(true);

        }

    }

    //called from the 'playerCarryProjectile.cs' class when the player picks up an object
    public void ChangeOutlineToPlayer()
    {
        outlineRenderer.color = playerOutlineColour;

        //disable collider
        projectileCollider.enabled = false;
    }

    //called from the 'playerCarryProjectile.cs' class when the player throws an object
    public void EnableCollider(bool enable)
    {
        //enable collider
        projectileCollider.enabled = enable;
    }


    //Setting the drag of the projectile so it can slow down or not slow down
    public void EnableDrag(float minTime, float maxTime)
    {
        Invoke(nameof(SetDrag), Random.Range(minTime,maxTime));
    }
    public void RemoveDrag()
    {
        rb.drag = 0f;
    }
    private void SetDrag()
    {
         rb.drag = 2.35f;
    }


    private void MoveProjectile()
    {
        rb.velocity = travelDir * travelSpeed;
    }



    //Called From other scripts to not collide with other projectiles
    public IEnumerator IgnoreProjectilesCoroutine(bool _ignore, float _delay)
    {
        yield return new WaitForSeconds(_delay);

        IgnoreProjectileLayer(_ignore);
    }
    
    private void IgnoreProjectileLayer(bool ignoreCollision)
    {
        //Just set the collider to trigger
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
                Destroy(gameObject);
            }
            else if(hitPlayer)
            {
                entityHealth.DamageEntity(damageDealt);
                Destroy(gameObject);
            }
            else if(hitRandomEntity)
            {
                entityHealth.DamageEntity(damageDealt);
                Destroy(gameObject);
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
                    Destroy(gameObject);

                }

            }
            else if ( hitOtherProjectile || hitFeta)
            {
                Destroy(gameObject);

            }

        }
        

    }

    protected void InstantiateHitParticles()
    {
        Instantiate(hitParticles, transform.position, Quaternion.identity);
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

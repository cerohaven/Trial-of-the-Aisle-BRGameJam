using NodeCanvas.Framework;

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
    [SerializeField] protected SO_AdjustHealth adjustHealth;
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

    // BOSS VARIABLES //
    protected Blackboard bossBlackboard;


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

        //Set the blackboard of the boss only if the boss is the one that threw the pill, else if overwrites the 
        //bossBlackboard variable to null which isn't what we want
        if (whoThrew == WhoThrew.Boss && targetThrown.CompareTag("Boss"))
        {
            bossBlackboard = targetThrown.gameObject.GetComponent<Blackboard>();
        }



    }

    //On Awake, get the projectile's rigidbody
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        interactableProjectile = GetComponent<InteractableObject_Projectile>();
        projectileCollider = GetComponent<Collider2D>();
        AudioManager.Instance.Play("boss_attack");
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
        Invoke("SetDrag", Random.Range(minTime,maxTime));
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

    //Ignoring collision with other projectiles
    public void IgnoreProjectiles(bool _ignore, float _delay)
    {

        if (_ignore)
        {
            Invoke("IgnoreProjectileLayer", _delay);
        }
        else
        {
            Invoke("DontIgnoreProjectileLayer", _delay);
        }
    }
    private void IgnoreProjectileLayer()
    {
        //Just set the collider to trigger
        projectileCollider.isTrigger = true;


    }
    private void DontIgnoreProjectileLayer()
    {
        projectileCollider.isTrigger = false;
    }

    //Ignoring collisions with certain layers
    public void IgnoreBossCollision(bool _ignore)
    {
        if (bossBlackboard == null) return;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bossBlackboard.gameObject.GetComponent<Collider2D>(), _ignore);

    }



    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Play the Particle effect for hitting something if it's not
        if(whoThrew != WhoThrew.Null)
        {
            Instantiate(hitParticles, transform.position, Quaternion.identity);
        }
        


        // HITTING THE BOSS //
        if (collision.gameObject.CompareTag("Boss") && whoThrew == WhoThrew.Player)
        {
            //send to reduce scale of boss bar
            adjustHealth.changeBossHealthEvent.Invoke(damageDealt, HealthType.Damage, transform.up);
            CinemachineShake.Instance.ShakeCamera();
            Destroy(gameObject);
        }

        // HITTING THE PLAYER //
        if(collision.gameObject.CompareTag("Player") && whoThrew == WhoThrew.Boss)
        {
            adjustHealth.changePlayerHealthEvent.Invoke(damageDealt, HealthType.Damage);
            Destroy(gameObject);
        }

        // HITTING A WALL //
        if(collision.gameObject.CompareTag("Walls"))
        {
            rb.velocity = Vector2.zero;

            if(whoThrew == WhoThrew.Player)
            {
                Destroy(gameObject);
            }
        }

        // HITTING ANOTHER PROJECTILE //
        if(collision.gameObject.CompareTag("Pill") || collision.gameObject.CompareTag("Feta"))
        {
            Destroy(gameObject);
        }


    }



    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        // HITTING A WALL //
        if (collision.gameObject.CompareTag("Walls"))
        {

            Destroy(gameObject);

        }
    }
}

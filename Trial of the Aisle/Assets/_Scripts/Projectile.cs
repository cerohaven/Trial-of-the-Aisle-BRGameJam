using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
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
    }

    //On Awake, get the projectile's rigidbody
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        interactableProjectile = GetComponent<InteractableObject_Projectile>();
        projectileCollider = GetComponent<Collider2D>();
        AudioManager.instance.Play("boss_attack");
    }

    //On Start, apply a velocity to the projectile in the direction and speed given.
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if(rb.velocity.magnitude < 0.5f && whoThrew == WhoThrew.Boss)
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
    public void EnableCollider()
    {
        //enable collider
        projectileCollider.enabled = true;
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
        Physics2D.IgnoreLayerCollision(10, 10, true);
    }
    private void DontIgnoreProjectileLayer()
    {
        Physics2D.IgnoreLayerCollision(10, 10, false);
    }

    //Ignoring collisions with certain layers
    public void IgnoreBossCollision(bool _ignore)
    {
        if (bossBlackboard == null) return;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bossBlackboard.gameObject.GetComponent<Collider2D>(), _ignore);

    }



    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Perform OnCollision Logic
        if (collision.gameObject.CompareTag("Boss"))
        {
            //send to reduce scale of boss bar
            adjustHealth.changeBossHealthEvent.Invoke(damageDealt, HealthType.Damage);
        }
        if(collision.gameObject.CompareTag("Player"))
        {
            adjustHealth.changePlayerHealthEvent.Invoke(damageDealt, HealthType.Damage);
        }

        //Instantiate(hitParticles, transform.position + Vector3.right * TravelDirection * 0.2f, Quaternion.identity);
        Destroy(gameObject);
    }



    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    ///The base class for any throwable projectile, either from the boss or from the player

    // -- REFERNCES -- //
    [SerializeField] protected SO_AdjustHealth adjustHealth;

    // -- COMPONENTS -- //
    [SerializeField] private GameObject hitParticles; //On collision, spawn particles

    /// </summary>
    protected Rigidbody2D rb;

    // -- PROJECTILE VARIABLES -- //

    protected Vector2 travelDir;

    protected float travelSpeed;

    protected bool canBePickedUp;

    [SerializeField] private ChangeHealth damageDealt;

    protected Transform targetThrown; //Get the Thrown target. If it was thrown by the player
                                     //it shouldn't have any effect if it accidentally hits the player.



    //Properties
    public Transform TargetThrown { get => targetThrown; set => targetThrown = value; }

    public virtual void InitializeProjectile(Vector2 _direction, float _speed, Transform _target)
    {
        travelDir = _direction;
        travelSpeed = _speed;
        targetThrown = _target;
        MoveProjectile();
    }

    //On Awake, get the projectile's rigidbody
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        AudioManager.instance.Play("boss_attack");
    }

    //On Start, apply a velocity to the projectile in the direction and speed given.
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if(rb.velocity.magnitude < 0.1f)
        {
            canBePickedUp = true;
        }
    }
    private void MoveProjectile()
    {
        rb.velocity = travelDir * travelSpeed;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    ///The base class for any throwable projectile, either from the boss or from the player

    // -- COMPONENTS -- //
    private Rigidbody2D rb;

    // -- PROJECTILE VARIABLES -- //

    protected Vector2 travelDir;

    protected float travelSpeed;

    [SerializeField] private int damageDealt;

    protected Transform targetThrown; //Get the Thrown target. If it was thrown by the player
                                     //it shouldn't have any effect if it accidentally hits the player.



    //Properties
    public Transform TargetThrown { get => targetThrown; set => targetThrown = value; }
    public int DamageDealt { get => damageDealt; set => damageDealt = value; }

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
    }
    //On Start, apply a velocity to the projectile in the direction and speed given.
    protected virtual void Start()
    {
        
    }
    private void MoveProjectile()
    {
        rb.velocity = travelDir * travelSpeed;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //Perform OnCollision Logic
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}

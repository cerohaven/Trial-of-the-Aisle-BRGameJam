using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
public class Projectile_Cheese : Projectile
{
    //Components

    //For one of the boss' attacks that suck all the pills back up.
    private bool isThrownInWave = false;
    private float turnIntensity = 0; //For turning during the wave attack

    //This changes the behaviour of the pill based on the boss' attacks
    public bool IsThrownInWave { get => isThrownInWave; set => isThrownInWave = value; }

    //Sets the speed and direction of the pill as well as gets the blackboard of the pill boss
    public override void InitializeProjectile(Vector2 _dir, float _speed, Transform _target, WhoThrew _whoThrew)
    {
        base.InitializeProjectile(_dir, _speed, _target, _whoThrew);


    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }



    protected override void Update()
    {

    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
       
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        EntityHealth entityHealth = collision.gameObject.GetComponent<EntityHealth>();

        //On Collision with the player, deal damage
        if (entityHealth != null && collision.gameObject.CompareTag("Player"))
        {
            entityHealth.DamageEntity(damageDealt);
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

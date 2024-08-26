using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
public class Projectile_Jam : Projectile
{
    private EntityHealth entityHealth;

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
        if (rb.velocity.magnitude < 1.5f && whoThrew == WhoThrew.Boss)
        {
            BreakJam();
        }


    }


    private void BreakJam()
    {
        Instantiate(hitParticles, transform.position, Quaternion.identity);
        GameManager.Instance.CurrentProjectilesInScene--;
        Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // HITTING A WALL //
        if (collision.gameObject.CompareTag("Walls"))
        {
            BreakJam();

        }

        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        if (GameManager.Instance.GameEnded) return;

        //On Collision with the player, deal damage UNLESS it can be picked up 
        if (collision.gameObject.CompareTag("Player"))
        {
            if (entityHealth == null) entityHealth = collision.gameObject.GetComponent<EntityHealth>();
            entityHealth.DamageEntity(ChangeHealth.Small_Health);

        }

       

    }
}

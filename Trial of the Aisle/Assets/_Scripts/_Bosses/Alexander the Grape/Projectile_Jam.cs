using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
public class Projectile_Jam : Projectile
{

    //Sets the speed and direction of the pill as well as gets the blackboard of the pill boss
    public override void InitializeProjectile(Vector2 _dir, float _speed, Transform _target, WhoThrew _whoThrew)
    {
        base.InitializeProjectile(_dir, _speed, _target, _whoThrew);

        //Set the blackboard of the boss only if the boss is the one that threw the pill, else if overwrites the 
        //bossBlackboard variable to null which isn't what we want
        if (_whoThrew == WhoThrew.Boss)
        {
            bossBlackboard = _target.gameObject.GetComponent<Blackboard>();
        }

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


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        if (GameManager.gameEnded) return;

        //On Collision with the player, deal damage UNLESS it can be picked up 
        if (collision.gameObject.CompareTag("Player"))
        {
            adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Small_Health, HealthType.Damage);

        }

        BreakJam();

    }

    private void BreakJam()
    {
        Instantiate(hitParticles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // HITTING A WALL //
        if (collision.gameObject.CompareTag("Walls"))
        {
            BreakJam();

        }
    }
}

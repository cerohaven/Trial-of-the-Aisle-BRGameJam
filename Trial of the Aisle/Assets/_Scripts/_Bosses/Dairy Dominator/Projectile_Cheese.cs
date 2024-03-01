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

    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        if (GameManager.gameEnded) return;

        if (whoThrew == WhoThrew.Player)
        {
            if (collision.gameObject.CompareTag("Boss"))
            {
                adjustHealth.ChangeBossHealthEventSend(ChangeHealth.Large_Health, HealthType.Damage, transform.up);
                Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

            //On Collision with the player, deal damage
            if (!collision.gameObject.CompareTag("Player"))
            {

                Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

        }
        else if (whoThrew == WhoThrew.Boss)
        {
            //On Collision with the player, deal damage
            if (collision.gameObject.CompareTag("Player"))
            {

                adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Large_Health, HealthType.Damage);
                Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

            if (!collision.gameObject.CompareTag("Boss"))
            {
                Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

        }





    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //On Collision with the player, deal damage
        if (collision.gameObject.CompareTag("Player"))
        {
            adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Medium_Health, HealthType.Damage);
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

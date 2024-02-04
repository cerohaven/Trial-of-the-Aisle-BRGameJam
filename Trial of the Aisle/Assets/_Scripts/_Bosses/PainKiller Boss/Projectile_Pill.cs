using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;

public class Projectile_Pill : Projectile
{
    //Components
    

    //For one of the boss' attacks that suck all the pills back up.
    private bool isBeingSuckedIn = false;
    private bool isThrownInWave = false;
    
    //This changes the behaviour of the pill based on the boss' attacks
    public bool IsBeingSuckedIn { get => isBeingSuckedIn; set => isBeingSuckedIn = value; }
    public bool IsThrownInWave { get => isThrownInWave; set => isThrownInWave = value; }

    //Sets the speed and direction of the pill as well as gets the blackboard of the pill boss
    public override void InitializeProjectile(Vector2 _dir, float _speed, Transform _target, WhoThrew _whoThrew)
    {
        base.InitializeProjectile(_dir, _speed, _target, _whoThrew);

        //Set the blackboard of the boss only if the boss is the one that threw the pill, else if overwrites the 
        //bossBlackboard variable to null which isn't what we want
        if(_whoThrew == WhoThrew.Boss)
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
        if (rb.velocity.magnitude < 0.5f && whoThrew == WhoThrew.Boss)
        {
            canBePickedUp = true;
            targetThrown = null;
            whoThrew = WhoThrew.Null;
            outlineRenderer.color = neutralOutlineColour;
            interactableProjectile.SetInteractable(true);
            IgnoreProjectiles(true, 0);
        }

        //Keep increasing velocity towards the boss only if its being sucked in and
        //the pill isn't from the player
        if (isBeingSuckedIn && whoThrew != WhoThrew.Player)
        {
            base.InitializeProjectile(travelDir, travelSpeed, targetThrown, WhoThrew.Boss);
        }
    }

   
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        if (GameManager.gameEnded) return;


        //If a pill hits another pill, or if another projectile hits a pill
        //If the other projectile collides with this one
        bool collideWithProjectile;
        if(collision.gameObject.layer == 10 || collision.gameObject.layer == 9)
        {
            //If they are currently colliding with another projectile,
            //If this projectile is neutral, then don't destroy it
            if(targetThrown == null)
            {
                collideWithProjectile = false;
            }
            else
            {
                collideWithProjectile = true;
            }

            
        }
        else
        {
            collideWithProjectile = false;
        }

        if ((!collision.gameObject.CompareTag("Boss") && collideWithProjectile) ||
            //if hit wall during the wave, then destroy it
            (collision.gameObject.CompareTag("Walls") && isThrownInWave))
        {
            Destroy(gameObject);
        }
    
        //If the Pill is thrown by the player, make it get destroyed by anything it touches
        if(whoThrew == WhoThrew.Player)
        {

            if(collision.gameObject.CompareTag("Boss"))
            {
                adjustHealth.ChangeBossHealthEventSend(ChangeHealth.Large_Health, HealthType.Damage);
                CinemachineShake.Instance.ShakeCamera();
            }
            Destroy(gameObject);
        }


        //When the boss is sucking in all the pills for the attack, when the pill touches the boss don't deal
        //any damage and increase a pill count
        if (isBeingSuckedIn )
        {
            if (collision.gameObject.CompareTag("Boss"))
            {
                //Increase a pill count
                int currentPills = bossBlackboard.GetVariableValue<int>("pillsSucked");
                currentPills++;
                bossBlackboard.SetVariableValue("pillsSucked", currentPills);
                Destroy(gameObject);
            }
        }
        


        //On Collision with the player, deal damage UNLESS it can be picked up 
        if(collision.gameObject.CompareTag("Player"))
        {
            if (canBePickedUp && targetThrown != bossBlackboard.transform)
                return;

            adjustHealth.ChangePlayerHealthEventSend(ChangeHealth.Small_Health, HealthType.Damage);
            Destroy(gameObject);
        }

        

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}

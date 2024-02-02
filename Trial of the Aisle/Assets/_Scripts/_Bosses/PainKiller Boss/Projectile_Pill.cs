using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;

public class Projectile_Pill : Projectile
{
    //Components
    private Blackboard bossBlackboard;

    //For one of the boss' attacks that suck all the pills back up.
    private bool isBeingSuckedIn = false;
    private bool isThrownInWave = false;
    
    //This changes the behaviour of the pill based on the boss' attacks
    public bool IsBeingSuckedIn { get => isBeingSuckedIn; set => isBeingSuckedIn = value; }
    public bool IsThrownInWave { get => isThrownInWave; set => isThrownInWave = value; }

    //Sets the speed and direction of the pill as well as gets the blackboard of the pill boss
    public override void InitializeProjectile(Vector2 _dir, float _speed, Transform _target)
    {
        base.InitializeProjectile(_dir, _speed, _target);
        bossBlackboard = _target.gameObject.GetComponent<Blackboard>();

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
        base.Update();

        //Keep increasing velocity towards the boss
        if(isBeingSuckedIn)
        {
            base.InitializeProjectile(travelDir, travelSpeed, targetThrown);
        }
    }

    //Ignoring collisions with certain layers
    public void IgnoreBossCollision(bool _ignore)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bossBlackboard.gameObject.GetComponent<Collider2D>(), _ignore);

    }
    public void IgnorePillCollision(bool _ignore)
    {
        Physics2D.IgnoreLayerCollision(10, 10, _ignore);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isThrownInWave)
        {
            
            if (!collision.gameObject.CompareTag("Boss") && !collision.gameObject.CompareTag("Pill"))
            {
                Destroy(gameObject);
            }
        }
        if(isBeingSuckedIn )
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

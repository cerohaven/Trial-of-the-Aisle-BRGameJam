using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.Tasks.Actions;

public class Projectile_Pill : Projectile
{
    [SerializeField] private Blackboard bossBlackboard;


    //For one of the boss' attacks that suck all the pills back up.
    private bool isBeingSuckedIn = false;

    public bool IsBeingSuckedIn { get => isBeingSuckedIn; set => isBeingSuckedIn = value; }


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
    private void Update()
    {
        if(isBeingSuckedIn)
        {
            base.InitializeProjectile(travelDir, travelSpeed, targetThrown);
        }
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(isBeingSuckedIn )
        {

            if (collision.gameObject.tag == "Boss")
            {

                //Increase a pill count
                int currentPills = bossBlackboard.GetVariableValue<int>("pillsSucked");
                currentPills++;
                bossBlackboard.SetVariableValue("pillsSucked", currentPills);

                Destroy(gameObject);
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}

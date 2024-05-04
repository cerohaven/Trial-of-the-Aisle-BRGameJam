
using NodeCanvas.Framework;
using UnityEngine;


public class Projectile_PainKiller : Projectile
{
    //Components

    //For one of the boss' attacks that suck all the pills back up.
    private bool isBeingSuckedIn = false;
    private bool isThrownInWave = false;
    private float turnIntensity = 0; //For turning during the wave attack


    //This changes the behaviour of the pill based on the boss' attacks
    public bool IsBeingSuckedIn { get => isBeingSuckedIn; set => isBeingSuckedIn = value; }
    public bool IsThrownInWave { get => isThrownInWave; set => isThrownInWave = value; }
    public float TurnIntensity { get => turnIntensity; set => turnIntensity = value; }

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
        base.Update();

        if(isThrownInWave)
            PillInWave();

        //Keep increasing velocity towards the boss only if its being sucked in and
        //the pill isn't from the player
        if (isBeingSuckedIn && whoThrew != WhoThrew.Player && targetThrown != null)
        {
            base.InitializeProjectile(travelDir, travelSpeed, targetThrown, WhoThrew.Boss);
            interactableProjectile.SetInteractable(false);
        }
    }

    private void PillInWave()
    {

        //Rotate around the boss' position
        rb.velocity += (Vector2)transform.right * turnIntensity * Time.deltaTime;
        transform.up = rb.velocity;

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        //if (GameManager.gameEnded) return;

        base.OnCollisionEnter2D (collision);

        if(whoThrew == WhoThrew.Boss )
        {
            if(collision.gameObject.CompareTag("Boss"))
            {
                //if the pills are being sucked in and they collide with the boss, destroy them
                Destroy(gameObject);
            }

            if (collision.gameObject.CompareTag("Walls") && (isThrownInWave || isBeingSuckedIn))
            {
                Destroy(gameObject);
            }
        }
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if(collision.gameObject.CompareTag("Player"))
        {
            adjustHealth.changePlayerHealthEvent.Invoke(damageDealt, HealthType.Damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(this.gameObject);
        }
    }
}

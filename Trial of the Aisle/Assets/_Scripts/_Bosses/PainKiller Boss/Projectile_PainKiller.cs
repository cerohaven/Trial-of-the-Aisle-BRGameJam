
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

        //Keep increasing velocity towards the boss only if its being sucked in and
        //the pill isn't from the player
        if (isBeingSuckedIn && whoThrew != WhoThrew.Player && targetThrown != null)
        {
            base.InitializeProjectile(travelDir, travelSpeed, targetThrown, WhoThrew.Boss);
            interactableProjectile.SetInteractable(false);
        }
    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //If the boss is defeated at the end, then make sure we don't run code or else nullreference!
        //if (GameManager.gameEnded) return;

        base.OnCollisionEnter2D (collision);

    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        if(isBeingSuckedIn && collision.gameObject.CompareTag("Boss"))
        {
            DestroyGameObject();

            InstantiateHitParticles();
        }
        if (collision.gameObject.CompareTag("Walls") && (isThrownInWave || isBeingSuckedIn))
        {
            DestroyGameObject();

            InstantiateHitParticles();
        }


        base.OnTriggerEnter2D(collision);

        

    }
}

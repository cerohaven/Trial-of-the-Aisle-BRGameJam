
using NodeCanvas.Framework;
using System.Collections;
using UnityEngine;


public class Projectile_PainKiller : Projectile
{
    //Components

    //For one of the boss' attacks that suck all the pills back up.
    private bool isBeingSuckedIn = false;
    private bool isThrownInWave = false;

    private Vector2 suckDir;
    private float suckSpeed;
    private Transform suckTarget;

    //This changes the behaviour of the pill based on the boss' attacks
    public bool IsBeingSuckedIn { get => isBeingSuckedIn; set => isBeingSuckedIn = value; }
    public bool IsThrownInWave { get => isThrownInWave; set => isThrownInWave = value; }

    //Sets the speed and direction of the pill as well as gets the blackboard of the pill boss
    public override void InitializeProjectile(Vector2 _dir, float _speed, Transform _target, WhoThrew _whoThrew)
    {
        if (isBeingSuckedIn)
        {
            suckDir = _dir;
            suckSpeed = _speed;
            suckTarget = _target;

            RemoveDrag();
            interactableProjectile.SetInteractable(false);
            ShakeProjectile();
            return;
        }

        base.InitializeProjectile(_dir, _speed, _target, _whoThrew);

        
    }

    private void ShakeProjectile()
    {
        EnableTrigger(true);
        
        LeanTween.rotateZ(gameObject, transform.rotation.z + 0.01f, 0.1f).setLoopPingPong(Random.Range(3,10)).
            setOnComplete(MoveObject);

    }
    private void MoveObject()
    {

        base.InitializeProjectile(suckDir, suckSpeed, suckTarget, WhoThrew.Boss);
    }


    public void StopSuckingPill()
    {
        isBeingSuckedIn = false;

        StartCoroutine(EnableDragCoroutine(0, 0, 5));

        whoThrew = WhoThrew.Boss;
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

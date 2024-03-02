using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InteractableObject_Projectile : InteractableObject
{
    //References
    private Projectile projectile;
    private PlayerCarryProjectile playerCarry;

    //Variables
    [SerializeField] private float throwSpeed;

    public PlayerCarryProjectile PlayerCarry { get => playerCarry; set => playerCarry = value; }

    protected override void Awake()
    {
        base.Awake();
        projectile = GetComponent<Projectile>();
        SO_interactableObject.launchProjectileButtonEvent.AddListener(LaunchProjectile);
    }

    private void LaunchProjectile(GameObject player)
    {
        //Check to make sure that it's THIS pill that recieves the event since this event goes to all projectiles
        if (playerCarry == null) return;
        if (playerCarry.CarryObject != gameObject) return;

        //Now we launch the projectile in the direction the player is facing

        projectile.InitializeProjectile(transform.up, throwSpeed, playerCarry.transform.parent, WhoThrew.Player);
        projectile.EnableCollider(true);
        projectile.RemoveDrag();
        playerCarry.IsCarryingObject = false;

        //Now I need to re-enable collision with the boss layer
        projectile.IgnoreBossCollision(false);
        projectile.IgnoreProjectiles(false, 0);

        AudioManager.instance.Play("p_throw");
        CinemachineShake.Instance.ShakeCamera(1);

    }

    protected override void OnInteract(GameObject _interactedActor)
    {

        //Get the player's script on carrying an object
        playerCarry = _interactedActor.GetComponentInChildren<PlayerCarryProjectile>();

        //if the player is currently carrying a projectile, then don't pick this one up
        if (playerCarry.IsCarryingObject) return;

        playerCarry.InitializeObject(gameObject);
        projectile.WhoThrew = WhoThrew.Player;
        isInteractable = false;

        AudioManager.instance.Play("p_pickUp");
    }

    protected override bool IsInteractable() { return isInteractable; }

    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }


}

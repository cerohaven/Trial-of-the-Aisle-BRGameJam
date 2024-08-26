using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using FMODUnity;

public class InteractableObject_Projectile : InteractableObject
{
    //References
    private Projectile projectile;
    private PlayerCarryProjectile playerCarry;

    //Variables
    [SerializeField] private float throwSpeed;
    private Collider2D bossCollider;

    public PlayerCarryProjectile PlayerCarry { get => playerCarry; set => playerCarry = value; }

    protected override void Awake()
    {
        base.Awake();
        bossCollider = GameManager.Instance.BossTransform.GetComponent<Collider2D>();

        projectile = GetComponent<Projectile>();
        GameManager.Instance.EventSender.launchProjectileButtonEvent.AddListener(LaunchProjectile);
    }

    private void LaunchProjectile(GameObject player, Vector2 upDir)
    {
        //Check to make sure that it's THIS pill that recieves the event since this event goes to all projectiles
        if (playerCarry == null) return;
        if (playerCarry.CarryObject != gameObject) return;

        //Now we launch the projectile in the direction the player is facing
        transform.up = upDir;
        projectile.InitializeProjectile(transform.up, throwSpeed, playerCarry.transform.parent, WhoThrew.Player);
        projectile.EnableCollider(true);
        projectile.RemoveDrag();
        playerCarry.IsCarryingObject = false;


        RuntimeManager.PlayOneShot("event:/SFX/Bosses/General/ThrowProjectile");
        //AudioManager.instance.Play("p_throw");
        CinemachineShake.Instance.ShakeCamera(1);

    }

    protected override void OnInteract(GameObject _interactedActor)
    {

        //Get the player's script on carrying an object
        playerCarry = _interactedActor.GetComponentInChildren<PlayerCarryProjectile>();

        //if the player is currently carrying a projectile, then don't pick this one up
        if (playerCarry.IsCarryingObject) return;
        if (projectile.WhoThrew == WhoThrew.Boss) return;
        playerCarry.InitializeObject(gameObject);
        projectile.WhoThrew = WhoThrew.Player;
        isInteractable = false;

        RuntimeManager.PlayOneShot("event:/SFX/Bosses/General/PickUpItem");
        //AudioManager.instance.Play("p_pickUp");
    }

    protected override bool IsInteractable() { return isInteractable; }

    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }


}

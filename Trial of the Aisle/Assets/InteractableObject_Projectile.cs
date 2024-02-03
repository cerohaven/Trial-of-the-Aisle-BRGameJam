using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InteractableObject_Projectile : InteractableObject
{
    //References
    private Projectile projectile;

    protected override void Awake()
    {
        base.Awake();
        projectile = GetComponent<Projectile>();
    }

    protected override void OnInteract(GameObject _interactedActor)
    {
        
        Debug.Log("INTERACTED WITH");

        //Get the player's script on carrying an object
        PlayerCarryProjectile playerCarry = _interactedActor.GetComponentInChildren<PlayerCarryProjectile>();

        //if the player is currently carrying a projectile, then don't pick this one up
        if (playerCarry.IsCarryingObject) return;

        playerCarry.InitializeObject(gameObject);

        isInteractable = false;
    }

    protected override bool IsInteractable() { return isInteractable; }

    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject_Shelf : InteractableObject
{
    [Separator()]
    [SerializeField] private int amountOfItemsOnShelf = 1;

    [SerializeField] private GameObject[] shelfItem = new GameObject[1];

    protected override void OnInteract(GameObject _interactedActor)
    {
        GameObject shelfProjectile = Instantiate(shelfItem[Random.Range(0, shelfItem.Length)]);

        //Spawn in a shelf item in the carryObject script
        PlayerCarryProjectile playerCarry = _interactedActor.GetComponentInChildren<PlayerCarryProjectile>();

        //if the player is currently carrying a projectile, then don't pick this one up
        if (playerCarry.IsCarryingObject) return;


        playerCarry.InitializeObject(shelfProjectile);

        Projectile projectile = shelfProjectile.GetComponent<Projectile>();
        InteractableObject_Projectile ioProjectile = shelfProjectile.GetComponent<InteractableObject_Projectile>();
        ioProjectile.PlayerCarry = playerCarry;
        projectile.WhoThrew = WhoThrew.Player;

        amountOfItemsOnShelf--;

        if(amountOfItemsOnShelf <= 0)
        {
            //Now we can't interact with this shelf anymore
            HideUI();
            isInteractable = false;
            isInteractPointVisible = false;
            
        }
        
    }


    protected override bool IsInteractable() { return isInteractable; }
    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }



}

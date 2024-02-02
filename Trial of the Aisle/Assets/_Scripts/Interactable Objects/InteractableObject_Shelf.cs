using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject_Shelf : InteractableObject
{


    protected override void OnInteract()
    {
        inPlayerRange = true;
    }


    protected override bool IsInteractable() { return isInteractable; }
    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }



}

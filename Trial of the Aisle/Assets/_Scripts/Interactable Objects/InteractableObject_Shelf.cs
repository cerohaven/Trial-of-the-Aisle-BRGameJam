using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject_Shelf : InteractableObject
{


    protected override void OnInteract()
    {
        
    }


    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }



}

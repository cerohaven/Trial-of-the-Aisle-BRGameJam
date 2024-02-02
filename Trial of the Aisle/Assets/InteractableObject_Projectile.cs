using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InteractableObject_Projectile : InteractableObject
{

    protected override void OnInteract()
    {
        Debug.Log("INTERACTED WITH");
    }

    protected override bool IsInteractable() { return isInteractable; }

    protected override bool IsTargetPointVisible() { return isInteractPointVisible; }


}

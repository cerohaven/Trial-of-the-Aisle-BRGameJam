using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarryProjectile : MonoBehaviour
{

    private bool isCarryingObject = false;
    private GameObject carryObject;
    private Projectile objProjectile;

    private Transform carryObjectTransform;
    private Transform thisTransform;

    public bool IsCarryingObject { get => isCarryingObject; set => isCarryingObject = value; }
    public GameObject CarryObject { get => carryObject;}

    private void Awake()
    {
        thisTransform = transform;
    }

    void Update()
    {
        if (isCarryingObject == false) return;

        //Update the GameObject's position to this position if the player is carrying the object
        carryObjectTransform.position = thisTransform.position;
    }


    public void InitializeObject(GameObject _carryObj)
    {
        carryObject = _carryObj;
        isCarryingObject = true;
        objProjectile = _carryObj.GetComponent<Projectile>();
        carryObjectTransform = _carryObj.transform;

        //Set the target to the player
        objProjectile.TargetThrown = thisTransform.parent;
        objProjectile.ChangeOutlineToPlayer();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarryProjectile : MonoBehaviour
{

    private bool isCarryingObject = false;
    private GameObject carryObject;
    private Projectile objProjectile;

    private Transform carryObjectTransform;
    private Transform thisTransform;
    private Transform playerTransform;

    public bool IsCarryingObject { get => isCarryingObject; set => isCarryingObject = value; }
    public GameObject CarryObject { get => carryObject;}

    private void Awake()
    {
        thisTransform = transform;
        playerTransform = transform.parent;
    }

    void Update()
    {
        if (isCarryingObject == false) return;


        // Calculate direction towards the mouse cursor
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        //Move this gameObject around the player
        Vector2 dir = mouseWorldPosition - playerTransform.position;

        dir.Normalize();
        thisTransform.position = (Vector2)playerTransform.position + dir * 2;

        //Update the GameObject's position to this position if the player is carrying the object
        carryObjectTransform.position = thisTransform.position;
        carryObjectTransform.up = dir;
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

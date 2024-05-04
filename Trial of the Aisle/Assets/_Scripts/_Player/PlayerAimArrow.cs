using NodeCanvas.Tasks.Conditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimArrow : MonoBehaviour
{
    [SerializeField] private PlayerCarryProjectile pcp;
    private Transform playerTransform;
    private Transform thisTransform;
    private bool isActive;

    private void Awake()
    {
        thisTransform = transform;
        playerTransform = transform.parent;
        gameObject.SetActive(false);
    }
    void Update()
    {

        // Calculate direction towards the mouse cursor
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Move this gameObject around the player
        Vector2 dir = mouseWorldPosition - playerTransform.position;

        dir.Normalize();
        thisTransform.position = (Vector2)playerTransform.position + dir * 3;

        //Update the GameObject's position to this position if the player is carrying the object
        thisTransform.up = dir;
    }
}

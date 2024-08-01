using NodeCanvas.Tasks.Conditions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimArrow : MonoBehaviour
{

    private Transform _playerTransform;
    private Transform _thisTransform;


    private void Awake()
    {
        _thisTransform = transform;
        _playerTransform = transform.parent;
        gameObject.SetActive(false);
    }
    void Update()
    {

        // Calculate direction towards the mouse cursor
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Move this gameObject around the player
        Vector2 dir = mouseWorldPosition - _playerTransform.position;

        dir.Normalize();
        _thisTransform.position = (Vector2)_playerTransform.position + dir * 3;

        //Update the GameObject's position to this position if the player is carrying the object
        _thisTransform.up = dir;
    }
}

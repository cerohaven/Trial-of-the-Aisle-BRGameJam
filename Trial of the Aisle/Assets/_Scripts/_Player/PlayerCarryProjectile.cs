using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerCarryProjectile : MonoBehaviour
{
    [SerializeField] private GameObject _playerAimArrowGO;
    [Range(3,5)]
    [SerializeField] private float _aimArrowDistanceFromPlayer = 4;

    private bool _isCarryingObject = false;
    private GameObject _carryObject;
    private Projectile _objProjectile;

    private Transform _carryObjectTransform;
    private Transform _thisTransform;
    private Transform _playerTransform;

    private InputAction rightStick;
    private Vector2 prevDir = Vector2.up;

    //Properties
    public bool IsCarryingObject { get => _isCarryingObject; set => _isCarryingObject = value; }
    public GameObject CarryObject { get => _carryObject;}


    private void Awake()
    {
        _thisTransform = transform;
        _playerTransform = transform.parent;
        
        _playerAimArrowGO.SetActive(false);
    }

    public void InitializeObject(GameObject _carryObj)
    {
        _carryObject = _carryObj;
        _isCarryingObject = true;
        _objProjectile = _carryObj.GetComponent<Projectile>();
        _carryObjectTransform = _carryObj.transform;

        //Set the target to the player
        _objProjectile.TargetThrown = _thisTransform.parent;
        _objProjectile.ChangeProjectileOutline();
        _objProjectile.EnableCollider(false);

        SetActiveAimArrows(true);

        

        if (GameManager.Instance.ControlScheme == ControlScheme.Gamepad)
        {
           
            rightStick = GameManager.Instance.PlayerInputHandler.PlayerInput.actions["Look"];

            GameManager.Instance.GamepadCursor.EnableCursor(false);
        }
            
    }

    void Update()
    {
        if (_carryObject == null) return;

        if (_isCarryingObject == false) return;

        if (GameManager.Instance.PlayerInputHandler == null) return;

        Vector2 dir = new Vector2(0,0);

        if (GameManager.Instance.ControlScheme == ControlScheme.Gamepad)
        {
            //Move this gameObject around the player
            
            dir = rightStick.ReadValue<Vector2>();
            
            if(dir == Vector2.zero)
            {
                dir = prevDir;
            }
            else
            {
                prevDir = dir;
            }
        }
        else
        {
            //MOUSE 
            // Calculate direction towards the mouse cursor
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            dir = mouseWorldPosition - _playerTransform.position;

        }
        
        
        

        dir.Normalize();
        _thisTransform.position = (Vector2)_playerTransform.position + dir * 2;

        //Update the GameObject's position to this position if the player is carrying the object
        _carryObjectTransform.position = _thisTransform.position;
        _carryObjectTransform.up = dir;


        _playerAimArrowGO.transform.position = (Vector2)_playerTransform.position + dir * _aimArrowDistanceFromPlayer;
        _playerAimArrowGO.transform.up = dir;
    }



    public void SetActiveAimArrows(bool setActive)
    {
        if (_playerAimArrowGO.activeSelf == setActive) return;

        _playerAimArrowGO.SetActive(setActive);
    }
}

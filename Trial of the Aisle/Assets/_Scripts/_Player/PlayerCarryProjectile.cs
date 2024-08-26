using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
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

    private Vector2 tempBossDir;

    //Properties
    public bool IsCarryingObject { get => _isCarryingObject; set => _isCarryingObject = value; }
    public GameObject CarryObject { get => _carryObject;}

    public Vector2 TempBossDir { get => tempBossDir;}
    private void Awake()
    {
        _thisTransform = transform;
        _playerTransform = transform.parent;
        
       
    }
    private void Start()
    {
        rightStick = GameManager.Instance.PlayerInputHandler.PlayerInput.actions["Look"];
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
 
            GameManager.Instance.GamepadCursor.EnableCursor(false);
        }
            
    }

    void Update()
    {
        Vector2 alwaysDir = Vector2.zero;

        if( rightStick.ReadValue<Vector2>() != Vector2.zero)
        {
            alwaysDir= rightStick.ReadValue<Vector2>();
            alwaysDir.Normalize();
            prevDir = alwaysDir;
        }

        alwaysDir= rightStick.ReadValue<Vector2>();
        alwaysDir.Normalize();

        if(alwaysDir == Vector2.zero)
        {
            alwaysDir = prevDir;
        }
        

        _playerAimArrowGO.transform.position = (Vector2)_playerTransform.position + alwaysDir * _aimArrowDistanceFromPlayer;
        _playerAimArrowGO.transform.up = alwaysDir;

        if (_carryObject == null) return;

        if (_isCarryingObject == false) return;

        if (GameManager.Instance.PlayerInputHandler == null) return;

        Vector2 dir = new Vector2(0,0);
        Vector2 bossDir = GameManager.Instance.BossTransform.position - _playerTransform.position;

        
        if (GameManager.Instance.ControlScheme == ControlScheme.Gamepad)
        {
            dir = alwaysDir;
        }
        else
        {
            //MOUSE 
            // Calculate direction towards the mouse cursor
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            dir = mouseWorldPosition - _playerTransform.position;

        }
        
        float bossAngle = Vector2.Angle(bossDir,dir);
        Debug.Log(bossAngle);

        if(bossAngle < 20)
        {
            bossDir.Normalize();
            //snap to boss' transform
            //GameManager.Instance.GamepadCursor.CursorMouse.WarpCursorPosition(GameManager.Instance.BossTransform.position);
            //InputState.Change(GameManager.Instance.GamepadCursor.VirtualMouse.position, GameManager.Instance.BossTransform.position);
             //Update the GameObject's position to this position if the player is carrying the object
             Debug.Log("AAAA");
            tempBossDir = bossDir;

        }
        else
        {
            tempBossDir = dir;
        }
      
            
        dir.Normalize();
        
       

        _thisTransform.position = (Vector2)_playerTransform.position + dir * 2;

        //Update the GameObject's position to this position if the player is carrying the object
        _carryObjectTransform.position = _thisTransform.position;
        _carryObjectTransform.up = dir;


    }



    public void SetActiveAimArrows(bool setActive)
    {
        if (_playerAimArrowGO.activeSelf == setActive) return;

        _playerAimArrowGO.SetActive(setActive);
    }
}

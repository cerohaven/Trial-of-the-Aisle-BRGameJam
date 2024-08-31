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

    [Range(0,90)]
    [SerializeField] private float aimAssist = 20;


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
        if (GameManager.Instance.PlayerInputHandler == null) return;


        Vector2 aimingDirection = Vector2.zero;

        #region Aiming Direction

        if(GameManager.Instance.ControlScheme == ControlScheme.Gamepad)
        {
            //Reading the Value for the Right Stick Direction
            Vector2 rightStickValue = rightStick.ReadValue<Vector2>();

            aimingDirection = rightStickValue;

            //Now for a backup, we check to see if the player is moving the stick or not. If so, we can set the previous dir,
            //but if they are not moving the right stick, set it to the previous direction
            bool isMovingRightStick = rightStickValue != Vector2.zero;
            if (isMovingRightStick)
            {
                prevDir = aimingDirection;
            }
            else
            {
                aimingDirection = prevDir;
            }
        }
        else
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            aimingDirection = mouseWorldPosition - _playerTransform.position;
            
        }

        aimingDirection.Normalize();



        #endregion

        #region Moving Aiming Arrows Based on Direction
        _playerAimArrowGO.transform.position = (Vector2)_playerTransform.position + aimingDirection * _aimArrowDistanceFromPlayer;
        _playerAimArrowGO.transform.up = aimingDirection;
        #endregion

        if (_carryObject == null) return;

        if (_isCarryingObject == false) return;


        #region Aim Assist

        Vector2 bossDir = GameManager.Instance.BossTransform.position - _playerTransform.position;

        float bossAngle = Vector2.Angle(bossDir, aimingDirection);
        bool shouldAimAssist = bossAngle < aimAssist;
        if(shouldAimAssist)
        {
            bossDir.Normalize();
            tempBossDir = bossDir;
        }
        else
        {
            tempBossDir = aimingDirection;
        }
        #endregion

        #region Move Carry Projectile

        //Moving the Rotation and position of the Carry Projectile

        _thisTransform.position = (Vector2)_playerTransform.position + aimingDirection * 2;

        //Update the GameObject's position to this position if the player is carrying the object
        _carryObjectTransform.position = _thisTransform.position;
        _carryObjectTransform.up = aimingDirection;

        #endregion

    }



    public void SetActiveAimArrows(bool setActive)
    {
        if (_playerAimArrowGO.activeSelf == setActive) return;

        _playerAimArrowGO.SetActive(setActive);
    }
}

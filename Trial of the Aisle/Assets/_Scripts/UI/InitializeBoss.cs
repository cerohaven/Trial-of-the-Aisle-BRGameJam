
using NodeCanvas.Framework;
using UnityEngine;

public class InitializeBoss : MonoBehaviour
{
    [SerializeField] private SO_BossProfile _bossProfile;
    [SerializeField] private float _bossBarIncreaseDuration = 0.7f;

    private Blackboard _agentBlackboard;
    private EntityHealth _entityHealth;
    public SO_BossProfile ThisBossProfile { get => _bossProfile;}

    private void Awake()
    {
        GameManager.Instance.BossProfile = _bossProfile;

        _entityHealth = GetComponent<EntityHealth>();

        _agentBlackboard = GetComponent<Blackboard>();
    }

    private void Start()
    {
        //set the speed
        _agentBlackboard.SetVariableValue("bossSpeed", _bossProfile.B_BaseMoveSpeed);

        //Set the Boss Profile
        _agentBlackboard.SetVariableValue("bossProfile", _bossProfile);

        //Check if player is null
        Transform playerTransform = _agentBlackboard.GetVariableValue<Transform>("playerTransform");
        if(playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerController>().transform;
            _agentBlackboard.SetVariableValue("playerTransform", playerTransform);
        }
        
    }

    /// <summary>
    /// Called from the BossIntroAnimation when the animation finishes, we can then increase the fill amount and set the variable to true
    /// </summary>
    public void StartBossBattle()
    {
        _agentBlackboard.SetVariableValue("canStartBossFight", true);

        _entityHealth.IncreaseHealthBar(1, _bossBarIncreaseDuration);
    }
}


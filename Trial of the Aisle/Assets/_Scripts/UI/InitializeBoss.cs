
using FMODUnity;
using NodeCanvas.Framework;
using System.Collections;
using UnityEngine;

public class InitializeBoss : MonoBehaviour
{
    [SerializeField] private SO_BossProfile _bossProfile;

    [Title("Intro Sequence", TextAlignment.Left, TextColour.White, 20)]
    [Separator]
    [SerializeField] private float _introTimeOnScreen;
    [SerializeField] private Animator[] _HUDanimators;

    [Space]
    [Title("After Intro", TextAlignment.Left, TextColour.White, 20)]
    [Separator]
    [SerializeField] private float _bossBarIncreaseDuration = 0.7f;

    private Blackboard _agentBlackboard;
    private EntityHealth _entityHealth;

    //Properties
    public SO_BossProfile ThisBossProfile { get => _bossProfile;}


    private void Awake()
    {
        GameManager.Instance.BossProfile = _bossProfile;
        GameManager.Instance.BossTransform = transform;
        GameManager.Instance.BossIsDefeated = false;
        _entityHealth = GetComponent<EntityHealth>();

        _agentBlackboard = GetComponent<Blackboard>();
        Debug.Log(_agentBlackboard);
    }

    private void Start()
    {
        SetBossVariableValues();

        BeginIntroAnimation();

    }



    private void SetBossVariableValues()
    {
        //set the speed
        _agentBlackboard.SetVariableValue("bossSpeed", _bossProfile.B_BaseMoveSpeed);

        //Set the Boss Profile
        _agentBlackboard.SetVariableValue("bossProfile", _bossProfile);

        //Check if player is null
        Transform playerTransform = _agentBlackboard.GetVariableValue<Transform>("playerTransform");
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerController>().transform;
            _agentBlackboard.SetVariableValue("playerTransform", playerTransform);
        }
    }



    #region Intro Animation Sequence
    private void BeginIntroAnimation()
    {
        //If we disabled the Intro Panel Game Object in the inspector, than just play the game
        if (_HUDanimators[0].gameObject.activeSelf == false) 
        { 
            StartBossBattle(); 
            return; 
        }

        Invoke(nameof(EndOfAnimation), _introTimeOnScreen);
        GameManager.Instance.FreezePlayerMovement();
    }

    private void EndOfAnimation()
    {
        RemoveHUD();
    }

    private void RemoveHUD()
    {
        Invoke(nameof(StartBossBattle), 1.5f);
        for (int i = 0; i < _HUDanimators.Length; i++)
        {
            _HUDanimators[i].SetTrigger("Reverse");
        }
    }

   
    #endregion

    /// <summary>
    /// Called from the BossIntroAnimation when the animation finishes, we can then increase the fill amount and set the variable to true
    /// </summary>
    public void StartBossBattle()
    {
        RuntimeManager.PlayOneShot("event:/UI/GUI/HealthBarRaise");

        GameManager.Instance.UnFreezePlayerMovement();

        _agentBlackboard.SetVariableValue("canStartBossFight", true);

        _entityHealth.HealthBar.fillAmount = 0;
        _entityHealth.IncreaseHealthBar(1, _bossBarIncreaseDuration);
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<EntityHealth>().DestroyEntity();
        }
    }
}


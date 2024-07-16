
using NodeCanvas.Framework;
using UnityEngine;

public class InitializeBoss : MonoBehaviour
{
    [SerializeField] private SO_BossProfile bossProfile;
    private Blackboard agentBlackboard;

    public SO_BossProfile ThisBossProfile { get => bossProfile;}

    private void Awake()
    {
        GameManager.Instance.BossProfile = bossProfile;
        agentBlackboard = GetComponent<Blackboard>();
    }

    private void Start()
    {
        //set the speed
        agentBlackboard.SetVariableValue("bossSpeed", bossProfile.B_BaseMoveSpeed);

        //Set the Boss Profile
        agentBlackboard.SetVariableValue("bossProfile", bossProfile);

        //Check if player is null
        Transform playerTransform = agentBlackboard.GetVariableValue<Transform>("playerTransform");
        if(playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerController>().transform;
            agentBlackboard.SetVariableValue("playerTransform", playerTransform);
        }
        
    }
}


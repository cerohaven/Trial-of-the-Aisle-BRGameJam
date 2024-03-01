using NodeCanvas.Framework;

using UnityEngine;

public class UpdateGrapeSpriteHealth : MonoBehaviour
{


    //References
    [SerializeField] SO_AdjustHealth adjustHealth;

    //Components
    [SerializeField] private Animator animator;
    private Blackboard agentBlackboard;

    private void Awake()
    {
        agentBlackboard = GetComponent<Blackboard>();

        adjustHealth.updateBossSpriteEventSend.AddListener(UpdateSprite);
    }


    private void UpdateSprite()
    {
        
        //Set it to the current phase - 1
        int currentPhase = agentBlackboard.GetVariableValue<int>("bossPhase");
        animator.SetInteger("increment", currentPhase);


    }
}

using NodeCanvas.Framework;

using UnityEngine;

public class UpdateGrapeSpriteHealth : MonoBehaviour
{

    //Components
    [SerializeField] private Animator animator;
    private Blackboard agentBlackboard;

    private void Awake()
    {
        agentBlackboard = GetComponent<Blackboard>();

        GameManager.Instance.EventSender.updateBossSpriteEventSend.AddListener(UpdateSprite);
    }


    private void UpdateSprite()
    {
        
        //Set it to the current phase - 1
        int currentPhase = agentBlackboard.GetVariableValue<int>("bossPhase");
        animator.SetInteger("increment", currentPhase);


    }
}

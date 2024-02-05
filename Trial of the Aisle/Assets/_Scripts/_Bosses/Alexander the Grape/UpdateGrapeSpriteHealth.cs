using NodeCanvas.Framework;

using UnityEngine;

public class UpdateGrapeSpriteHealth : MonoBehaviour
{

    //References
    [SerializeField] SO_AdjustHealth adjustHealth;

    //Components
    [SerializeField] private Animator animator;
    private Blackboard agentBlackboard;
    

    //Variables
    private float[] healthIncrement = new float[4];
    private float currentHealth;
    private float maxHealth;

    private void Awake()
    {
        agentBlackboard = GetComponent<Blackboard>();
        adjustHealth.updateBossSpriteEventSend.AddListener(UpdateSprite);
    }

    private void Start()
    {
        maxHealth = agentBlackboard.GetVariableValue<float>("bossMaxHealth");
        //Setting the health incrememnt values. At these milestones the boss' attacks change behaviour
        healthIncrement[0] = 100 / maxHealth;  //100% health
        healthIncrement[1] = 75 / maxHealth;   //75% health
        healthIncrement[2] = 50 / maxHealth;   //50% health
        healthIncrement[3] = 25 / maxHealth;	//25% health
    }

    private void UpdateSprite()
    {
        //Calculate the health increment
        // CALCULATE SPEED OF THE PILL //
        currentHealth = agentBlackboard.GetVariableValue<float>("bossHealth");

        float increment = currentHealth / maxHealth;

        //For loop to see if it is > the current increment or not
        for (int i = healthIncrement.Length - 1; i >= 0; i--)
        {
            //eg. if increment is 0.65, then we want to check if <25, then <50, then <75, then <100
            //Yes to <75 (i = 1), so we set the pill speed to be projectileSpeed[i]
            if (increment <= healthIncrement[i])
            {
                animator.SetInteger("increment", i);
                break;
            }
        }
    }
}

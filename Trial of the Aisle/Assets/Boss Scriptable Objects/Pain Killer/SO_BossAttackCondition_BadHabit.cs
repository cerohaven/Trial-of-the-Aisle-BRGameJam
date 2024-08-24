using NodeCanvas.Framework;

using UnityEngine;

[CreateAssetMenu(fileName = "Bad Habit Condition", menuName = "Boss Scriptable Objects/Boss Attack Condition/PainKiller/Bad Habit Condition")]
public class SO_BossAttackCondition_BadHabit : SO_BossAttackConditionBase
{
    [Range(0,100)]
    public float _minimumHealthPercentToUseAttack = 70f;
    private EntityHealth _entityHealth;

    public override bool OnCheckAttackCondition(Blackboard bossBlackboard)
    {
        _entityHealth = bossBlackboard.gameObject.GetComponent<EntityHealth>();

        //need to get a reference to the boss details for the current Health
        float currentHealth = _entityHealth.CurrentHealth;
        float maxHealth = _entityHealth.MaxHealth;
       
        //if the health  < certain amount, then we can perform this attack
        float healthPercent = (currentHealth / maxHealth) * 100;

        if (healthPercent < _minimumHealthPercentToUseAttack)
        {
            return true;
        }
        
        return false;

    }
}

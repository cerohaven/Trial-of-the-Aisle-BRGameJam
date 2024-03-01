using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bad Habit Condition", menuName = "Boss Scriptable Objects/Boss Attack Condition/PainKiller/Bad Habit Condition")]
public class SO_BossAttackCondition_BadHabit : BossAttackCondition
{
    [Range(0,100)]
    public float minimumHealthPercentToUseAttack = 70f;

    public override bool OnCheckAttackCondition(Blackboard bossBlackboard)
    {
        //need to get a reference to the boss details for the current Health
        float currentHealth = bossBlackboard.GetVariableValue<float>("bossHealth");
        float maxHealth = bossBlackboard.GetVariableValue<SO_BossProfile>("bossProfile").B_MaxHealth;
       
        //if the health  < certain amount, then we can perform this attack
        float healthPercent = (currentHealth / maxHealth) * 100;

        if (healthPercent < minimumHealthPercentToUseAttack)
        {
            return true;
        }
        
        return false;

    }
}

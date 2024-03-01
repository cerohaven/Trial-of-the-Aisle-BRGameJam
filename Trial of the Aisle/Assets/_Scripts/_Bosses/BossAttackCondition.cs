using NodeCanvas.Framework;
using UnityEngine;

/// <summary>
/// If the boss' attack needs to meet a special condition to be used, write the logic here
/// This is just the base script, do not edit anything here. 
/// To make a new condition, create a new Scriptable Object script and inherit from this script
/// </summary>

public abstract class BossAttackCondition : ScriptableObject
{


    public abstract bool OnCheckAttackCondition(Blackboard bossBlackboard);     /// <summary>
                                                                                /// if we return true, then the attack can successfully be used.
                                                                                /// if we return false, then we can't use the attack
                                                                                /// </summary>

}

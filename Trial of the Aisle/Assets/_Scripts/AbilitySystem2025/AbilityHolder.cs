using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    /// <summary>
    /// Base Class for cooldown controls
    /// </summary>
    
    [Header("Variables")]
    public Ability_SO ability;
    float cooldownTime;
    float activeTime;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }

    AbilityState state = AbilityState.ready;

    public KeyCode key;


    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key)) {
                    ability.Activate(gameObject);
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                }
            break;

            case AbilityState.active:
                if (activeTime <= 0) {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    ability.BeginCooldown(gameObject);
                    state = AbilityState.cooldown;
                    activeTime -= Time.deltaTime;
                }
            break;
            case AbilityState.cooldown:
                if (cooldownTime <= 0) {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.ready;
                }
            break;


        }
    }
}

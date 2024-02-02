using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Adjust Health Event Sender", menuName = "Scriptable Objects/AdjustHealth")]
public class SO_AdjustHealth : ScriptableObject
{
    /// <summary>
    /// Using the Scriptable Object as the middle man
    /// Other scripts call this event which sends it to the UI Manager
    /// Reduces Dependencies on other scripts
    /// </summary>
 
    [System.NonSerialized]
    public ChangeBossHealth changeBossHealthEvent = new ChangeBossHealth();

    [System.NonSerialized]
    public ChangePlayerHealth changePlayerHealthEvent = new ChangePlayerHealth();

    public void ChangeBossHealthEventSend(ChangeHealth changeHealth, HealthType healthType)
    {
        changeBossHealthEvent.Invoke(changeHealth, healthType);
    }

    public void ChangePlayerHealthEventSend(ChangeHealth changeHealth, HealthType healthType)
    {
        changePlayerHealthEvent.Invoke(changeHealth, healthType);
    }
}

public class ChangeBossHealth : UnityEvent<ChangeHealth, HealthType> { }
public class ChangePlayerHealth : UnityEvent<ChangeHealth, HealthType> { }






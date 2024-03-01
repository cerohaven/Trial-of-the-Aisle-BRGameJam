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
    /// 
    [System.NonSerialized]
    public UnityEvent<float> OnPlayerHealthChanged; // Event for player health change

    private void OnEnable()
    {
        if (OnPlayerHealthChanged == null)
            OnPlayerHealthChanged = new UnityEvent<float>();
    }

    public void AdjustPlayerHealth(float amount)
    {
        OnPlayerHealthChanged.Invoke(amount); // Invoke the event with the health adjustment amount
    }

    [System.NonSerialized]
    public ChangeBossHealth changeBossHealthEvent = new ChangeBossHealth();

    [System.NonSerialized]
    public ChangePlayerHealth changePlayerHealthEvent = new ChangePlayerHealth();

    [System.NonSerialized]
    public UnityEvent updateBossSpriteEventSend = new UnityEvent();

    public void ChangeBossHealthEventSend(ChangeHealth changeHealth, HealthType healthType, Vector2 projectileUpDir)
    {
        changeBossHealthEvent.Invoke(changeHealth, healthType, projectileUpDir);
        updateBossSpriteEventSend.Invoke();
    }

    public void ChangePlayerHealthEventSend(ChangeHealth changeHealth, HealthType healthType)
    {
        changePlayerHealthEvent.Invoke(changeHealth, healthType);
    }

}

public class ChangeBossHealth : UnityEvent<ChangeHealth, HealthType, Vector2> { }
public class ChangePlayerHealth : UnityEvent<ChangeHealth, HealthType> { }






using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Adjust Speed Event Sender", menuName = "Scriptable Objects/AdjustSpeed")]
public class SO_AdjustSpeed : ScriptableObject
{
    /// <summary>
    /// Using the Scriptable Object as the middle man
    /// Other scripts call this event which sends it to the UI Manager
    /// Reduces Dependencies on other scripts
    /// </summary>
    /// 
    [System.NonSerialized]
    public UnityEvent<float> OnPlayerSpeedChanged; // Event for player speed change

    private void OnEnable()
    {
        if (OnPlayerSpeedChanged == null)
            OnPlayerSpeedChanged = new UnityEvent<float>();
    }

    public void AdjustPlayerSpeed(float amount)
    {
        OnPlayerSpeedChanged.Invoke(amount); // Invoke the event with the speed adjustment amount
    }

    [System.NonSerialized]
    public ChangeBossSpeed changeBossSpeedEvent = new ChangeBossSpeed();

    [System.NonSerialized]
    public ChangePlayerSpeed changePlayerSpeedEvent = new ChangePlayerSpeed();

    [System.NonSerialized]
    public UnityEvent updateBossSpriteEventSend = new UnityEvent();

    public void ChangeBossSpeedEventSend(ChangeSpeed changeSpeed, SpeedType speedType, Vector2 projectileUpDir)
    {
        changeBossSpeedEvent.Invoke(changeSpeed, speedType, projectileUpDir);
        updateBossSpriteEventSend.Invoke();
    }

    public void ChangePlayerSpeedEventSend(ChangeSpeed changeSpeed, SpeedType speedType)
    {
        changePlayerSpeedEvent.Invoke(changeSpeed, speedType);
    }

}

public class ChangeBossSpeed : UnityEvent<ChangeSpeed, SpeedType, Vector2> { }
public class ChangePlayerSpeed : UnityEvent<ChangeSpeed, SpeedType> { }






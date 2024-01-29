using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BossDefeatEventSender", menuName = "ScriptableObjects/BossDefeatEvent")]
public class SO_BossDefeatedEventSender : ScriptableObject
{
    //Goes to GameManager to set the function to flicker the screen and activate functions
    [System.NonSerialized]
    public UnityEvent bossIsDefeatedEvent = new UnityEvent();

    public void BossIsDefeatedSend()
    {
        bossIsDefeatedEvent.Invoke();
    }

    //Goes to the flicker Game Object
    [System.NonSerialized]
    public UnityEvent flickerScreenEvent = new UnityEvent();

    public void FlickerScreenSend()
    {
        flickerScreenEvent.Invoke();
    }

    //Goes to the boss in this level (Whichever will recieve the event in this scene)
    [System.NonSerialized]
    public UnityEvent bossDefeatAnimationEvent = new UnityEvent();

    public void BossDefeatedAnimationSend()
    {
        bossDefeatAnimationEvent.Invoke();
    }

    //Whenever the boss is hit, update their health 
    [System.NonSerialized]
    public UpdateHealthEvent updateHealthEvent = new UpdateHealthEvent();

    public void UpdateBossHealthEventSend(float value)
    {
        updateHealthEvent.Invoke(value);
    }

}

public class UpdateHealthEvent : UnityEvent<float> { }
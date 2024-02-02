using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BossDefeatEventSender", menuName = "Scriptable Objects/BossDefeatEvent")]
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


}

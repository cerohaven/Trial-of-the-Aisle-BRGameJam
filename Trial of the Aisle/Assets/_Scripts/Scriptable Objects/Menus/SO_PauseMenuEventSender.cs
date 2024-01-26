using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Pause Menu Event Sender", menuName = "Event Senders/Pause Menu Sender")]
public class SO_PauseMenuEventSender : ScriptableObject
{
    //Pause Game Event
    [System.NonSerialized]
    public UnityEvent pauseGameEvent = new UnityEvent();

    public void PauseGameEventSend()
    {
        pauseGameEvent.Invoke();
    }

    //Resume Game Event
    [System.NonSerialized]
    public UnityEvent resumeGameEvent = new UnityEvent();

    public void ResumeGameEventSend()
    {
        resumeGameEvent.Invoke();
    }
}

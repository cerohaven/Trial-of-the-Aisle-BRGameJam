using FMODUnity;
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
    FMOD.Studio.EventInstance SFX_PauseEvent;
    FMOD.Studio.EventInstance SFX_UnPauseEvent;

    public void PauseGameEventSend()
    {
        SFX_PauseEvent = RuntimeManager.CreateInstance("event:/UI/Buttons/Pause");
        SFX_PauseEvent.start();
        pauseGameEvent.Invoke();
    }

    //Resume Game Event
    [System.NonSerialized]
    public UnityEvent resumeGameEvent = new UnityEvent();

    public void ResumeGameEventSend()
    {
        SFX_UnPauseEvent = RuntimeManager.CreateInstance("event:/UI/Buttons/Unpause");
        SFX_UnPauseEvent.start();
        resumeGameEvent.Invoke();
    }
}

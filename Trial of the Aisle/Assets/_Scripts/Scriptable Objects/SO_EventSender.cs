using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// //In charge of storing all the envents for scripts to invoke and listen to 
/// Using the Scriptable Object as the middle man
/// Other scripts call this event which sends it to the UI Manager
/// Reduces Dependencies on other scripts
/// </summary>
/// 

[CreateAssetMenu(fileName = "Event Sender", menuName = "Event Sender")]
public class SO_EventSender : ScriptableObject
{
    #region Boss Defeated Events

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

    #endregion




    #region Adjust Health Events

    [System.NonSerialized]
    public PlayerHealthChanged OnPlayerHealthChanged = new PlayerHealthChanged();

    public void AdjustPlayerHealth(float amount)
    {
        OnPlayerHealthChanged.Invoke(amount);
    }


    [System.NonSerialized]
    public ChangeBossHealth changeBossHealthEvent = new ChangeBossHealth();

    [System.NonSerialized]
    public UnityEvent updateBossSpriteEventSend = new UnityEvent();
    public void ChangeBossHealthEventSend(ChangeHealth changeHealth, HealthType healthType, Vector2 projectileUpDir)
    {
        changeBossHealthEvent.Invoke(changeHealth, healthType, projectileUpDir);
        updateBossSpriteEventSend.Invoke();
    }


    [System.NonSerialized]
    public ChangePlayerHealth changePlayerHealthEvent = new ChangePlayerHealth();

    public void ChangePlayerHealthEventSend(ChangeHealth changeHealth, HealthType healthType)
    {
        changePlayerHealthEvent.Invoke(changeHealth, healthType);
    }

    #endregion




    #region Interactable Object Events

    //When the player clicks the "Interact" button
    [System.NonSerialized]
    public ClickedInteractionButtonEvent clickedInteractButtonEvent = new ClickedInteractionButtonEvent();

    public void ClickedInteractButtonEventSend(GameObject _interactedActor)
    {
        clickedInteractButtonEvent.Invoke(_interactedActor);
    }


    //When the player clicks the Interact button when they are carrying an object
    [System.NonSerialized]
    public LaunchProjectileButtonEvent launchProjectileButtonEvent = new LaunchProjectileButtonEvent();

    public void LaunchProjectileButtonEventSend(GameObject _interactedActor)
    {
        launchProjectileButtonEvent.Invoke(_interactedActor);
    }

    //Is called from the "PlayerInputHandler.cs" class
    //Sends an event to all the interactable objects to update their sprite and text based on control
    [System.NonSerialized]
    public ChangedControlSchemeEvent changedControlSchemeEvent = new ChangedControlSchemeEvent();
    public void ChangedControlSchemeEventSend(string _controlScheme)
    {
        changedControlSchemeEvent.Invoke(_controlScheme);
    }

    [System.NonSerialized]
    public UnityEvent clickedCancelButtonEvent = new UnityEvent();
    public void ClickedCancelButtonEventSend()
    {
        clickedCancelButtonEvent.Invoke();
    }

    #endregion




    #region Pause Menu Events

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

    #endregion


    //Dodge
    [System.NonSerialized]
    public UnityEvent dodgeEvent = new UnityEvent();
    public void DodgeEventSender()
    {
        dodgeEvent.Invoke();
    }
    
}


public class ChangeBossHealth : UnityEvent<ChangeHealth, HealthType, Vector2> { }
public class ChangePlayerHealth : UnityEvent<ChangeHealth, HealthType> { }
public class PlayerHealthChanged : UnityEvent<float> { }

public class LaunchProjectileButtonEvent : UnityEvent<GameObject> { }
public class ClickedInteractionButtonEvent : UnityEvent<GameObject> { }
public class ChangedControlSchemeEvent : UnityEvent<string> { }

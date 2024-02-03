using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Clicked Interaction Event", menuName = "Event Senders/Clicked Interaction")]
public class SO_InteractableObject : ScriptableObject
{
    [System.NonSerialized]
    public ClickedInteractionButtonEvent clickedInteractButtonEvent = new ClickedInteractionButtonEvent();

    public void ClickedInteractButtonEventSend(GameObject _interactedActor)
    {
        clickedInteractButtonEvent.Invoke(_interactedActor);
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
}
[System.Serializable]
public class ClickedInteractionButtonEvent : UnityEvent<GameObject> { }

[System.Serializable]
public class ChangedControlSchemeEvent : UnityEvent<string> { }

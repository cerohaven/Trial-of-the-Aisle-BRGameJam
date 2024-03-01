using UnityEngine;

/// <summary>
/// WHen the boss reaches a certain health increment percent (like half health), then this script can be callsed
/// </summary>
public abstract class BossPhaseEvent : ScriptableObject
{
    protected bool eventPlayed;
    public bool EventPlayed { get => eventPlayed;}
    public abstract void OnHealthChange();

}

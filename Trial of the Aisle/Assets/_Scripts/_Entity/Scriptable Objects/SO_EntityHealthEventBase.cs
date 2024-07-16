using UnityEngine;

public abstract class SO_EntityHealthEventBase : ScriptableObject
{
    protected bool alreadyPerformedFunction;
    public bool AlreadyPerformedFunction { get => alreadyPerformedFunction; }
    public abstract void Initialize(GameObject gameObject);
    public abstract void StartEventMethod();
}
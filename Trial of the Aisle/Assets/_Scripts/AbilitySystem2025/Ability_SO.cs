using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_SO : ScriptableObject
{
    [Header("Name and States")]
    public new string name;
    public float cooldownTime;
    public float activeTime;

    public virtual void Activate(GameObject parent)
    {
        Debug.Log("Activating " + name);
    }
    public virtual void BeginCooldown(GameObject parent)
    {
        Debug.Log("Deactivating " + name);
    }
}

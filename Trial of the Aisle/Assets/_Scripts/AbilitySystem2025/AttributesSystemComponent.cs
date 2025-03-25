using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributesSystemComponent : MonoBehaviour
{
    /// <summary>
    /// There are two types of attributes in the system, Resources and stats
    /// </summary>
    
    protected string _name;
    protected string _attributeName;
    public enum modifierOpertor
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}

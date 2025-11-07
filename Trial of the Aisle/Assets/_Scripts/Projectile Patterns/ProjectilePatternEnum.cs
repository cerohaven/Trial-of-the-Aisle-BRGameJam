using UnityEngine;
public enum ProjectilePatterns
{
    Some,
    Spread,
    Randomize_Angle,
    Rapid,
    Burst,
    Randomize_Spawn_Offset
}


[System.Serializable]
public struct PatternTypeMod
{
    [SerializeField] public string modName;
    [SerializeField] public float modValue;

    public PatternTypeMod(string name, float val) 
    {
       
        modName = name; 
        modValue = val;


    }
}

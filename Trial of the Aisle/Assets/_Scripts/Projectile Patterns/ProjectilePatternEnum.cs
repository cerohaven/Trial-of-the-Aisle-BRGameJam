using UnityEngine;
public enum ProjectilePatterns
{
    Some,
    Spread,
    Randomize_Angle,
    Rapid,
    Burst
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

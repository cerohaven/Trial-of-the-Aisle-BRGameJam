
public enum ProjectilePatterns
{
    Some,
    Spread,
    Repeat
}


[System.Serializable]
public struct PatternTypeMod
{
    public string modName;
    public float modValue;

    public PatternTypeMod(string name, float val) 
    {
       
        modName = name; 
        modValue = val;


    }
}

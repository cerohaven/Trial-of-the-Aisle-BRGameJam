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

//For each projectile pattern mod we can decalre a type for it so it displays as that type in the inspector to ease the load on designers.
//In the code however everything is read as a float.
//You can create any new type you want by adding to this enum and then adding logic for visuals in the SO_ProjectilePattern_Editor.cs
public enum ModVariableType
{
    Float,
    Float_Slider_0_1,  // creates a float slider from ranges 0-1
    Float_Slider_0_360,// creates a float slider from ranges 0-360
    Int,
    Int_Buttons, //Plus and Minus buttons surround the input field
    Bool
}


[System.Serializable]
public struct PatternTypeMod
{
    [SerializeField] public string modName;
    [SerializeField] public float modValue;
    [SerializeField] public ModVariableType modVariableType;

    public PatternTypeMod(string name, float val, ModVariableType type) 
    {
       
        modName = name; 
        modValue = val;
        modVariableType = type;

    }
}

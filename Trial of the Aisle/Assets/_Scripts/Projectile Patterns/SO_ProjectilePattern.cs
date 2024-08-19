using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[System.Serializable]
public class ProjectilePattern
{
    [SerializeField] public ProjectilePatterns thisPatternType;
    [SerializeField] public PatternTypeMod[] thisPatternTypeModifiers;
}



[CreateAssetMenu(fileName = "Projectile Pattern", menuName = "Scriptable Objects/Projectiles/New Projectile Pattern")]
public class SO_ProjectilePattern : ScriptableObject
{

    [Space]
    [SerializeField] private int _patternNumberToUse;

    [Space]
    [SerializeField] private List<ProjectilePattern> _projectilePatterns = new List<ProjectilePattern>();

    [SerializeField] private List<bool> _foldouts = new List<bool>();
    [SerializeField] private bool _baseFoldout;

    //These Structs contain all the logic we need to store for each parameter/modifier in each pattern type.

    [SerializeField]
    public PatternTypeMod[] basePAT =
    {
        new PatternTypeMod("Angle", 0),                     //0
        new PatternTypeMod("Speed", 1),                     //1

    };

    [SerializeField]
    public PatternTypeMod[] somePAT = 
    { 
        new PatternTypeMod("Source", 0),                    //0
        new PatternTypeMod("Percent Chance", 1)             //1
    };

    [SerializeField]
    public PatternTypeMod[] spreadPAT =
    {
        new PatternTypeMod("Source", 0),                    //0
        new PatternTypeMod("Number of Projectiles", 1),     //1
        new PatternTypeMod("Spread Angle", 1)               //2
    };
    [SerializeField]
    public PatternTypeMod[] repeatPAT =
   {
        new PatternTypeMod("Source", 0),                    //0
        new PatternTypeMod("Number of Projectiles", 1),     //1
        new PatternTypeMod("delay", 1)                      //2
    };
    public List<ProjectilePattern> ProjectilePatternList { get => _projectilePatterns; set => _projectilePatterns = value; }
    public List<bool> Foldouts { get => _foldouts; set => _foldouts = value; }



    public List<PatternTypeMod[]> GetProjectilePatterns()
    {
        //Create a temporary list of Base Pattern structs and add a baseProjectile Already
        List<PatternTypeMod[]> tempProjectileList = new List<PatternTypeMod[]>
        {
            CreateNewProjectile(basePAT[0].modValue,basePAT[1].modValue)
        };

        for (int i = 0; i < _projectilePatterns.Count; i++)
        {
            ProjectilePatterns currentPattern = _projectilePatterns[i].thisPatternType;

            switch (currentPattern)
            {
                case ProjectilePatterns.Some:
                    //We want to get the tempProjectileList, loop through it, and if random number fails, remove it from the list
                    //How do we get the struct? We need to make a new struct in the inspector. NO WAIT, what if we have a base class and
                    //
                    for (int j = tempProjectileList.Count - 1; j >= 0; j++)
                    {
                        //if (!RandomValue(mod.percentChance)) tempProjectileList.RemoveAt(j);
                    }

                    break;

                case ProjectilePatterns.Spread:

                    break;

                case ProjectilePatterns.Repeat:
                    break;


                default:
                    break;
            }


            return tempProjectileList;
        }

        return tempProjectileList;

    }


    public void SetPatternInfo(int arrayIndex)
    {
        //Debug.Log(arrayIndex);
        //Debug.Log(_projectilePatterns.Count);
        ProjectilePatterns pats = _projectilePatterns[arrayIndex].thisPatternType;
        switch (pats)
        {
            case ProjectilePatterns.Some:
                _projectilePatterns[arrayIndex].thisPatternTypeModifiers = somePAT;
                break;
            case ProjectilePatterns.Spread:
                _projectilePatterns[arrayIndex].thisPatternTypeModifiers = spreadPAT;
                break;
            case ProjectilePatterns.Repeat:
                _projectilePatterns[arrayIndex].thisPatternTypeModifiers = repeatPAT;
                break;

        }
    }

    private PatternTypeMod[] CreateNewProjectile(float angle, float speed)
    {
        PatternTypeMod[] bp = basePAT;
        bp[1].modValue = angle;
        bp[2].modValue = speed;
        return bp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="percentChance"> Value From 0-1 </param>
    /// <returns></returns>
    private bool RandomValue(float percentChance)
    {
        float rand = Random.Range(0.0f, 1.0f);

        if (percentChance >= rand)
            return true;

        return false;
    }


}


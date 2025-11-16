using System.Collections.Generic;

using UnityEngine;


[System.Serializable]
public class ProjectilePattern
{
    [SerializeField] public ProjectilePatterns thisPatternType;
    [SerializeField] public PatternTypeMod[] thisPatternTypeModifiers;
    [SerializeField] public bool thisPatternIsFoldout = true;
    [SerializeField] public bool thisPatternIsActive = true;
}



[CreateAssetMenu(fileName = "Projectile Pattern", menuName = "Scriptable Objects/Projectiles/New Projectile Pattern")]
public class SO_ProjectilePattern : ScriptableObject
{

    [Space]
    [SerializeField] private List<ProjectilePattern> _projectilePatterns = new List<ProjectilePattern>();

    [SerializeField] private bool _baseFoldout = true;


    //These Structs contain all the logic we need to store for each parameter/modifier in each pattern type.

    /// <summary>
    /// IF YOU WANT TO CREATE A NEW PROJECTILE MODIFIER:
    /// 1. Update the enum in the "ProjectilePatternEnum" for the name of the modifier
    /// 2. Add a new Struct below on the data values that this modifier needs.
    /// 3. In the Editor script in the "UpdateModifierInfo",  add a new case for the new modifer
    /// 4. In the "GetProjectilePatterns" method apply the actual logic for what the modifer does to the previous bullets.
    /// 
    /// IF YOU WANT TO CREATE A NEW PROJECTILE MODIFIER VARIABLE TYPE: (ie. int, float, bln)
    /// 1. Navigate to the "ProjectilePatternEnum.cs" and look at the "ModVariableType" enum.
    /// 2. If the variable type you want isn't there, add it to the list. Give it a name that reflects what it'll be for organization.
    /// 3. Go to the "SO_ProjectilePattern_Editor.cs" and navigate to the "DrawModifier()" method, add a new case for that enum type.
    /// 4. You can now customize the Property to look however you want it by utilizing EditorGUI.<insertPropertyFieldOfYourChoice>
    /// </summary>
    /// 


    [SerializeField]
    public PatternTypeMod[] basePAT =
    {
        new PatternTypeMod("Angle", 0, ModVariableType.Float_Slider_0_360),        //0
        new PatternTypeMod("Speed", 1, ModVariableType.Float),                     //1
        new PatternTypeMod("Extra Angle", 0, ModVariableType.Float),               //2
        new PatternTypeMod("Delay", 0, ModVariableType.Float),                     //3
        new PatternTypeMod("Spawn Offset", 0, ModVariableType.Float),              //4
        new PatternTypeMod("Target Player", 1, ModVariableType.Bool)               //5
    };

    [SerializeField]
    public PatternTypeMod[] somePAT = 
    { 
        new PatternTypeMod("Percent Chance", 1, ModVariableType.Float_Slider_0_1)  //0
    };

    [SerializeField]
    public PatternTypeMod[] spreadPAT =
    {
        new PatternTypeMod("Number of Projectiles", 1, ModVariableType.Int_Buttons), //0
        new PatternTypeMod("Spread Angle", 1, ModVariableType.Float),                //1
        new PatternTypeMod("Spread Speed Increase", 0f, ModVariableType.Float),      //2
        new PatternTypeMod("Delay Between Projectiles", 0, ModVariableType.Float),   //3
        new PatternTypeMod("Mirror", 0, ModVariableType.Bool),                       //4
        new PatternTypeMod("Shift", 0, ModVariableType.Bool),                        //5
        new PatternTypeMod("Center Remove Number", 0, ModVariableType.Int)           //6
    };

    [SerializeField]
    public PatternTypeMod[] randomizeAnglePAT =
    {
        new PatternTypeMod("Angle Randomize Range", 1, ModVariableType.Float_Slider_0_360),   //0
    };

    [SerializeField]
    public PatternTypeMod[] rapidPAT =
    {
        new PatternTypeMod("Number of Projectiles", 1, ModVariableType.Int_Buttons), //0
        new PatternTypeMod("Fire Delay", 0.2f, ModVariableType.Float),               //1
    };

    [SerializeField]
    public PatternTypeMod[] burstPAT =
    {
        new PatternTypeMod("Number of Projectiles", 1, ModVariableType.Int_Buttons), //0
        new PatternTypeMod("Angle Range", 20f, ModVariableType.Float_Slider_0_360),  //1
        new PatternTypeMod("Speed Range", 0.5f, ModVariableType.Float),              //2
        new PatternTypeMod("Fire Delay Range", 0.5f, ModVariableType.Float),         //3
    };

    [SerializeField]
    public PatternTypeMod[] randomizeSpawnOffsetPAT =
    {
        new PatternTypeMod("Spawn Offset Range Range", 1, ModVariableType.Float),     //0
    };


    //Properties
    public List<ProjectilePattern> ProjectilePatternList { get => _projectilePatterns; set => _projectilePatterns = value; }

    public List<PatternTypeMod[]> GetProjectilePatterns()
    {
        bool targetPlayer = basePAT[5].modValue == 1 ? true : false;
        float calculateBaseAngle = targetPlayer == true ? -99 : basePAT[0].modValue;
        //Create a temporary list of Base Pattern structs and add a baseProjectile Already
        List<PatternTypeMod[]> tempProjectileList = new List<PatternTypeMod[]>
        {
            
            CreateNewProjectile(calculateBaseAngle, basePAT[1].modValue, basePAT[2].modValue, basePAT[3].modValue, basePAT[4].modValue)
        };

        List<PatternTypeMod[]> temptempProjectileList = new List<PatternTypeMod[]>();

        for (int i = 0; i < _projectilePatterns.Count; i++) 
        {
            //here we would check if the pattern is active or not and continue accordingly
            if (!_projectilePatterns[i].thisPatternIsActive) continue;

            temptempProjectileList.Clear();

            ProjectilePatterns currentPattern = _projectilePatterns[i].thisPatternType;

            switch (currentPattern)
            {
                case ProjectilePatterns.Some:
                    
                    for (int j = tempProjectileList.Count - 1; j >= 0; j--)
                    {
                        if (!RandomValueSuccess(_projectilePatterns[i].thisPatternTypeModifiers[0].modValue))
                        {
                            tempProjectileList.RemoveAt(j);
                        }
                    }

                    break;

                case ProjectilePatterns.Spread:
                    int amountOfProjectiles = (int)_projectilePatterns[i].thisPatternTypeModifiers[0].modValue;
                    float extraAngle = _projectilePatterns[i].thisPatternTypeModifiers[1].modValue;
                    float speed = _projectilePatterns[i].thisPatternTypeModifiers[2].modValue;
                    float delay = _projectilePatterns[i].thisPatternTypeModifiers[3].modValue;
                    bool mirror = _projectilePatterns[i].thisPatternTypeModifiers[4].modValue > 0;
                    bool shift = _projectilePatterns[i].thisPatternTypeModifiers[5].modValue > 0;
                    int spreadRemoveNumber = (int)_projectilePatterns[i].thisPatternTypeModifiers[6].modValue;
                    bool isEven = _projectilePatterns[i].thisPatternTypeModifiers[0].modValue % 2 == 0;
                    float sum = extraAngle * (amountOfProjectiles-1);

                    

                    //Loop through all the previous projectiles
                    for (int j = 0; j < tempProjectileList.Count; j++)
                    {
                        
                        //Loop the amount of projectiles we want to spawn for the spread
                        for (int k = 0 + spreadRemoveNumber; k < amountOfProjectiles; k++)
                        {
                            
                            float shiftAmount = 0;


                            if(shift)
                            {
                                if(!isEven)
                                {
                                    shiftAmount = extraAngle * Mathf.Floor(amountOfProjectiles / 2);
                                }
                                else
                                {

                                    shiftAmount = (sum / amountOfProjectiles) * (amountOfProjectiles / 2.00f);
                                }
                            }

                            temptempProjectileList.Add(CreateNewProjectile(basePAT[0].modValue, //Angle
                                                                           tempProjectileList[j][1].modValue + speed * k,                    //Speed
                                                                           tempProjectileList[j][2].modValue + extraAngle * k - shiftAmount, //Extra Angle
                                                                           tempProjectileList[j][3].modValue + delay * k,                    //Delay
                                                                           tempProjectileList[j][4].modValue));                              //Spawn Offset
                            
                            //If we're mirroring, we create a new bullet on the other side
                            if(mirror)
                            {
                                temptempProjectileList.Add(CreateNewProjectile(basePAT[0].modValue, //Angle
                                                                           tempProjectileList[j][1].modValue + speed * k,           //Speed
                                                                           tempProjectileList[j][2].modValue - extraAngle * k,      //Extra Angle
                                                                           tempProjectileList[j][3].modValue + delay * k,           //Delay
                                                                           tempProjectileList[j][4].modValue));                     //Spawn Offset
                            }

                        }

                        if (mirror || shift || j == 0)
                        {
                            tempProjectileList.RemoveAt(j);
                        }

                    }
                    break;

                case ProjectilePatterns.Randomize_Angle:

                    //Loop through all the projectiles before it and get their angle and add/remove a range
                    for (int j = 0; j < tempProjectileList.Count; j++)
                    {
                        //Get Random Range
                        float range = _projectilePatterns[i].thisPatternTypeModifiers[0].modValue;
                        float randomAngle = Random.Range(-range, range);

                        tempProjectileList[j][2].modValue += randomAngle;

                    }

                    break;

                case ProjectilePatterns.Rapid:
                    
                    for (int j = 0; j < tempProjectileList.Count; j++)
                    {
                        for (int k = 1; k < (int)_projectilePatterns[i].thisPatternTypeModifiers[0].modValue; k++)
                        {
                            Debug.Log(k);
                            temptempProjectileList.Add(CreateNewProjectile(basePAT[0].modValue,
                                                                           tempProjectileList[j][1].modValue,
                                                                           tempProjectileList[j][2].modValue,
                                                                           tempProjectileList[j][3].modValue +
                                                                           _projectilePatterns[i].thisPatternTypeModifiers[1].modValue * k,
                                                                           tempProjectileList[j][4].modValue)); //spawnOffset

                        }
                    }
                    break;

                case ProjectilePatterns.Burst:

                    //Loop through all the projectiles before it and get their angle and add/remove a range
                    for (int j = 0; j < tempProjectileList.Count; j++)
                    {
                        for (int k = 1; k < (int)_projectilePatterns[i].thisPatternTypeModifiers[0].modValue; k++)
                        {
                            //Get Random Range
                            float angleRange = _projectilePatterns[i].thisPatternTypeModifiers[1].modValue;
                            float delayRange = _projectilePatterns[i].thisPatternTypeModifiers[3].modValue;
                            float speedRange = _projectilePatterns[i].thisPatternTypeModifiers[2].modValue;

                            float randomAngleRange = Random.Range(-angleRange, angleRange);
                            float randomDelayRange = Random.Range(0, delayRange);
                            float randomSpeedRange = Random.Range(0, speedRange);

                            temptempProjectileList.Add(CreateNewProjectile(basePAT[0].modValue,
                                                                           tempProjectileList[j][1].modValue + randomSpeedRange, //speed
                                                                           tempProjectileList[j][2].modValue + randomAngleRange, //extra angle
                                                                           tempProjectileList[j][3].modValue + randomDelayRange, //delay
                                                                           tempProjectileList[j][4].modValue)); //spawnOffset
                        }
                    }

                    break;

                case ProjectilePatterns.Randomize_Spawn_Offset:

                    //Loop through all the projectiles before it and get their spawn offset and add/remove a range
                    for (int j = 0; j < tempProjectileList.Count; j++)
                    {
                        //Get Random Range
                        float range = _projectilePatterns[i].thisPatternTypeModifiers[0].modValue;
                        float randomAngle = Random.Range(tempProjectileList[j][4].modValue, range);

                        tempProjectileList[j][4].modValue = randomAngle;

                    }

                    break;

                default:
                    Debug.LogWarning("MODIFER NOT IMPLEMENTED");
                    break;
            }

            for(int j = 0; j < temptempProjectileList.Count; j++)
            {
                tempProjectileList.Add(temptempProjectileList[j]);
            }

        }

        return tempProjectileList;

    }



    private PatternTypeMod[] CreateNewProjectile(float angle, float speed, float extraAngle, float shootDelay, float spawnOffset)
    {
        PatternTypeMod[] bp = new PatternTypeMod[basePAT.Length];
        bp[0].modValue = angle;
        bp[1].modValue = speed;
        bp[2].modValue = extraAngle;
        bp[3].modValue = shootDelay;
        bp[4].modValue = spawnOffset;
        return bp;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="percentChance"> Value From 0-1 </param>
    /// <returns></returns>
    private bool RandomValueSuccess(float percentChance)
    {
        float rand = Random.Range(0.0f, 1.0f);

        if (percentChance >= rand)
            return true;

        return false;
    }


}



using System.Collections.Generic;

using UnityEngine;

namespace ProjectilePatterns
{
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
        /// IF YOU WANT TO CREATE A NEW PROJECTILE PATTERN:
        /// 1. Update the enum in the "ProjectilePatternEnum" for the name of the pattern you want to make
        /// 2. Add a new PatternTypeMod[] struct array below and a corresponding static function to initialize it. (Copy + Paste what I have for the BasePAT)
        /// 3. Change the name of the method and variable to the new pattern name for organization
        /// 4. In the "PAT_HelperFunction.cs" navigate to the "GetProjectilePatternModType(ProjectilePatterns pattern)" function and add a new case with your pattern.
        /// 3. In the Editor script in the "UpdateModifierInfo", add a new case for the new modifer
        /// 4. In the "GetProjectilePatterns" function in this script apply the actual logic for what the modifer does to the previous bullets.
        /// 
        /// 
        /// IF YOU WANT TO CREATE A NEW PROJECTILE MODIFIER VARIABLE TYPE: (ie. int, float, bln)
        /// 1. Navigate to the "ProjectilePatternEnum.cs" and look at the "ModVariableType" enum.
        /// 2. If the variable type you want isn't there, add it to the list. Give it a name that reflects what it'll be for organization.
        /// 3. Go to the "SO_ProjectilePattern_Editor.cs" and navigate to the "DrawModifier()" method, add a new case for that enum type.
        /// 4. You can now customize the Property to look however you want it by utilizing EditorGUI.<insertPropertyFieldOfYourChoice>
        /// 
        /// IF YOU WANT TO ADD A NEW PROPERTY TO THE PatternTypeMod struct in the "ProjectilePatternEnum.cs" - I currently have -
        /// modName, modValue, modVariableType, modTooltip
        /// 1. Like the other variables in the struct, make a new [SerializeField] public [variableType] [modName]
        /// 2. Add it in the constructor of that struct (like the others)
        /// 3. Now you can go in the Initialize_PAT functions and go to each pattern modifier and add it there!
        /// </summary>
        /// 



        [SerializeField]
        public PatternTypeMod[] basePAT = Initialize_BasePAT();
        public static PatternTypeMod[] Initialize_BasePAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Initial Angle", 0, ModVariableType.Float),              //0
                new PatternTypeMod("Initial Speed", 1, ModVariableType.Float),              //1
                new PatternTypeMod("Extra Angle", 0, ModVariableType.Float),                //2
                new PatternTypeMod("Spawn Delay", 0, ModVariableType.Float),                //3
                new PatternTypeMod("Spawn Offset", 0, ModVariableType.Float,                //4
                                   "How far away from the source should this projectile spawn"),
                new PatternTypeMod("Target Player", 1, ModVariableType.Bool,                //5
                                   "Should this projectile's initial angle target the player?"),
                new PatternTypeMod("Lifetime Duration", 0, ModVariableType.Float,           //6
                                   "How many seconds before this projectile should be destroyed"),
            };

        }


        [SerializeField]
        public PatternTypeMod[] somePAT = Initialize_SomePAT();
        public static PatternTypeMod[] Initialize_SomePAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Percent Chance", 1, ModVariableType.Float_Slider_0_1,       //0
                                   "The percent chance of each individual projectile from spawning in. \n" +
                                   "1 = All previous projectiles spawn in, \n" +
                                   "0 = No previous projectiles spawn inAAAAAA"),
            };

        }

        [SerializeField]
        public PatternTypeMod[] spreadPAT = Initialize_SpreadPAT();
        public static PatternTypeMod[] Initialize_SpreadPAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Number of Projectiles", 1, ModVariableType.Int_Buttons),            //0
                new PatternTypeMod("Spread Angle", 1, ModVariableType.Float,                            //1
                                   "The distance each projectile should be from one another"),
                new PatternTypeMod("Spread Speed Increase", 0f, ModVariableType.Float,                  //2
                                   "The initial speed of each sequential projectile will increase by this amount"),     
                new PatternTypeMod("Delay Between Projectiles", 0, ModVariableType.Float_Slider_0_1,    //3
                                   "How many seconds to wait before spawning the next sequential projectile"),         
                new PatternTypeMod("Mirror", 0, ModVariableType.Bool,                                   //4
                                   "Mirrors the Spread projectiles based on the initial angle"),                                  
                new PatternTypeMod("Shift", 0, ModVariableType.Bool,                                    //5
                                   "If we should shift the spread projectiles so the middle projectile will be where the inital angle is"),
                new PatternTypeMod("Center Remove Number", 0, ModVariableType.Int_Buttons,              //6
                                   "The spread projectiles to remove starting at the centre of the spread. Can be used if you want to create a gap/opening in the middle")
            };

        }


        [SerializeField]
        public PatternTypeMod[] randomizeAnglePAT = Initialize_RandomizeAnglePAT();
        public static PatternTypeMod[] Initialize_RandomizeAnglePAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Angle Randomize Range", 1, ModVariableType.Float,              //0
                                   "Randomize the angle of every individual projectile before this pattern by a range from -value to value."),
            };

        }


        [SerializeField]
        public PatternTypeMod[] rapidPAT = Initialize_RapidPAT();
        public static PatternTypeMod[] Initialize_RapidPAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Number of Projectiles", 1, ModVariableType.Int_Buttons,  //0
                                   "The amount of times should we loop the previous patterns"),
                new PatternTypeMod("Fire Delay", 0.2f, ModVariableType.Float,                //1
                                   "How many seconds to wait before spawning the next sequential projectile"),
            };

        }


        [SerializeField]
        public PatternTypeMod[] burstPAT = Initialize_BurstPAT();
        public static PatternTypeMod[] Initialize_BurstPAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Number of Projectiles", 1, ModVariableType.Int_Buttons), //0
                new PatternTypeMod("Angle Range", 20f, ModVariableType.Float,                //1
                                   "The random distance from -value to +value the projectiles can be from the initial angle in the clump"),
                new PatternTypeMod("Speed Range", 0.5f, ModVariableType.Float,               //2
                                   "The random speed from -value to +value each projectile in the clump can have"),
                new PatternTypeMod("Fire Delay Range", 0.5f, ModVariableType.Float,          //3
                                   "The random delay from -value to +value to spawn each projectile in the clump "),
            };

        }


        [SerializeField]
        public PatternTypeMod[] randomizeSpawnOffsetPAT = Initialize_RandomizeSpawnOffsetPAT();
        public static PatternTypeMod[] Initialize_RandomizeSpawnOffsetPAT()
        {
            return new PatternTypeMod[] {
                new PatternTypeMod("Spawn Offset Range Range", 1, ModVariableType.Float,    //0
                "The distance from the source Transform to spawn the projectile"),
            };

        }



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
                        float sum = extraAngle * (amountOfProjectiles - 1);



                        //Loop through all the previous projectiles
                        for (int j = 0; j < tempProjectileList.Count; j++)
                        {

                            //Loop the amount of projectiles we want to spawn for the spread
                            for (int k = 0 + spreadRemoveNumber; k < amountOfProjectiles; k++)
                            {

                                float shiftAmount = 0;


                                if (shift)
                                {
                                    if (!isEven)
                                    {
                                        shiftAmount = extraAngle * Mathf.Floor(amountOfProjectiles / 2);
                                    }
                                    else
                                    {

                                        shiftAmount = (sum / amountOfProjectiles) * (amountOfProjectiles / 2.00f);
                                    }
                                }

                                temptempProjectileList.Add(CreateNewProjectile(calculateBaseAngle, //Angle
                                                                               tempProjectileList[j][1].modValue + speed * k,                    //Speed
                                                                               tempProjectileList[j][2].modValue + extraAngle * k - shiftAmount, //Extra Angle
                                                                               tempProjectileList[j][3].modValue + delay * k,                    //Delay
                                                                               tempProjectileList[j][4].modValue));                              //Spawn Offset

                                //If we're mirroring, we create a new bullet on the other side
                                if (mirror)
                                {
                                    temptempProjectileList.Add(CreateNewProjectile(calculateBaseAngle, //Angle
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
                                temptempProjectileList.Add(CreateNewProjectile(calculateBaseAngle,
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
                            //Loop through the number of projectiles in the Burst PAT
                            for (int k = 1; k < (int)_projectilePatterns[i].thisPatternTypeModifiers[0].modValue; k++)
                            {
                                //Get Random Range
                                float angleRange = _projectilePatterns[i].thisPatternTypeModifiers[1].modValue;
                                float delayRange = _projectilePatterns[i].thisPatternTypeModifiers[3].modValue;
                                float speedRange = _projectilePatterns[i].thisPatternTypeModifiers[2].modValue;

                                float randomAngleRange = Random.Range(-angleRange, angleRange);
                                float randomDelayRange = Random.Range(0, delayRange);
                                float randomSpeedRange = Random.Range(0, speedRange);

                                temptempProjectileList.Add(CreateNewProjectile(calculateBaseAngle,
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

                for (int j = 0; j < temptempProjectileList.Count; j++)
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

        //Currently this refresh happens automatically as it's checked in the OnEnabled function in the SO_ProjectilePattern_Editor.cs
        //I can always make this into a button or change this logic so it doesn't always occur.
        [ContextMenu("Refresh Contents")]
        public void RefreshContents()
        {
            //loop through all patterns currently in the stack
            for(int i = 0; i < _projectilePatterns.Count; i++)
            {
                //Store the current pattern in temporary array
                PatternTypeMod[] tempMod = _projectilePatterns[i].thisPatternTypeModifiers;

                //Set the base pattern to the new one
                PatternTypeMod[] newMod = PAT_HelperFunctions.GetProjectilePatternModType(_projectilePatterns[i].thisPatternType);
                if (newMod == null) return;

                _projectilePatterns[i].thisPatternTypeModifiers = newMod;

                //Now get the newly created pattern and add back in our content by the index
                //NOTE: This might mix up values since it's only by index. If we have 3 modifiers and delete the second one,
                //the third one will now go to the 2 index and now share it's modValue.
                for(int j = 0; j < _projectilePatterns[i].thisPatternTypeModifiers.Length; j++)
                {
                    if (tempMod.Length <= j) continue;
                    _projectilePatterns[i].thisPatternTypeModifiers[j].modValue = tempMod[j].modValue;
                }
            }
        }

        public bool IsPatternStructsUpdated()
        {
            //check the PAT and see if it's equal to the Initialized Version of it.
            for (int i = 0; i < _projectilePatterns.Count; i++)
            {
                //Get the patternType and compare if it's different than the new initialized version
                ProjectilePatterns patternType = _projectilePatterns[i].thisPatternType;
                if (_projectilePatterns[i].thisPatternTypeModifiers != PAT_HelperFunctions.GetProjectilePatternModType(patternType))
                {
                    return false;
                }
            }
            return true;
        }


    }

}
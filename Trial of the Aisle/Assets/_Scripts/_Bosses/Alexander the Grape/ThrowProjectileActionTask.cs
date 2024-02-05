using NodeCanvas.Framework;
using UnityEngine;
using System.Collections;

namespace NodeCanvas.Tasks.Actions{

	public class ThrowProjectileActionTask : ActionTask{

        public float[] projectileSpeedAtHealthIncrements = new float[4];

        public float[] timeBetweenAttacksPerIncrement = new float[4];

        private float bossMaxHealth;
        private float bossHealth;
        private Blackboard agentBlackboard;
        private GameObject projectileToSpawn;

        private Transform playerTransform;
        private float pillSpeed;

        private float[] bossHealthIncrements = new float[4];


        private IEnumerator endActionRoutine;

        //Split Pill Variables
        public float SplitAngleInDegrees = 10;


        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit()
        {

            //Getting blackboar Variables
            agentBlackboard = agent.GetComponent<Blackboard>();

            projectileToSpawn = agentBlackboard.GetVariableValue<GameObject>("throwProjectile");
           
            playerTransform = agentBlackboard.GetVariableValue<Transform>("playerTransform");
            bossMaxHealth = agentBlackboard.GetVariableValue<float>("bossMaxHealth");

            //Setting the health incrememnt values. At these milestones the boss' attacks change behaviour
            bossHealthIncrements[0] = 100 / bossMaxHealth;  //100% health
            bossHealthIncrements[1] = 75 / bossMaxHealth;   //75% health
            bossHealthIncrements[2] = 50 / bossMaxHealth;   //50% health
            bossHealthIncrements[3] = 25 / bossMaxHealth;	//25% health


            return null;
        }



        //This is called once each time the task is enabled.
        protected override void OnExecute()
        {


            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;


            // CALCULATE SPEED OF THE PILL //
            bossHealth = agentBlackboard.GetVariableValue<float>("bossHealth");

            float healthIncrement = bossHealth / bossMaxHealth;

            //For loop to see if it is > the current increment or not
            for (int i = bossHealthIncrements.Length - 1; i >= 0; i--)
            {
                //eg. if increment is 0.65, then we want to check if <25, then <50, then <75, then <100
                //Yes to <75 (i = 1), so we set the pill speed to be projectileSpeed[i]
                if (healthIncrement <= bossHealthIncrements[i])
                {
                    pillSpeed = projectileSpeedAtHealthIncrements[i];

                    //0.7 * (100/100)
                    //0.7 * (75/100)
                    endActionRoutine = EndActionTask(timeBetweenAttacksPerIncrement[i]);
                    StartCoroutine(endActionRoutine);

                    break;
                }
            }




            // SPAWNING THE PILL GAME OBJECT //
            GameObject pill = GameObject.Instantiate(projectileToSpawn);

            //Setting the trajectory of the pill game object
            //Get rigidbody component and set direction and speed
            Vector3 dir = playerTransform.position - agent.transform.position;

            dir.Normalize();

            Projectile_Pill projectilePill = pill.GetComponent<Projectile_Pill>();
            ApplyInitializations(projectilePill, dir, pill);

            //Spawn in 3 pills at a time when the boss gets low
            if (healthIncrement < bossHealthIncrements[3])
            {
                SpawnMultiplePills(dir, projectileToSpawn);
            }


        }

        private void ApplyInitializations(Projectile_Pill projectilePill, Vector3 dir, GameObject pill)
        {


            pill.transform.position = agent.transform.position + (dir * 3.5f);
            projectilePill.InitializeProjectile(dir, pillSpeed, agent.transform, WhoThrew.Boss);
            projectilePill.IgnoreBossCollision(true);

            projectilePill.IgnoreProjectiles(true, 0);
            projectilePill.IgnoreProjectiles(false, 0.5f);
            projectilePill.EnableDrag(0, 2);

        }

        public void SpawnMultiplePills(Vector3 directionTravelling, GameObject pillPrefab)
        {

            for (int i = 0; i < 2; i++)
            {
                GameObject tempPill;
                Projectile_Pill tempProjectile;

                //spawn object and get rigidbody
                tempPill = MakePillCopy(pillPrefab);
                tempProjectile = tempPill.GetComponent<Projectile_Pill>();

                //If i == 0, make the split angle positive, if not, make it negative
                SplitAngleInDegrees = i == 0 ? SplitAngleInDegrees : -SplitAngleInDegrees;

                //Make a new Rotated vector from the main's vector
                Vector2 newVec = new Vector2(directionTravelling.x * Mathf.Cos(SplitAngleInDegrees * Mathf.Deg2Rad) - directionTravelling.y * Mathf.Sin(SplitAngleInDegrees * Mathf.Deg2Rad),
                                             directionTravelling.x * Mathf.Sin(SplitAngleInDegrees * Mathf.Deg2Rad) + directionTravelling.y * Mathf.Cos(SplitAngleInDegrees * Mathf.Deg2Rad));
                newVec.Normalize();

                //Add the force
                ApplyInitializations(tempProjectile, newVec, tempPill);

            }
        }

        private GameObject MakePillCopy(GameObject original)
        {
            Projectile_Pill newPill = GameObject.Instantiate(original).GetComponent<Projectile_Pill>();
            return newPill.gameObject;
        }


        IEnumerator EndActionTask(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            EndAction(true);
        }


        #region Other Functions

        protected override void OnUpdate()
        {

        }

        protected override void OnStop()
        {

        }

        protected override void OnPause()
        {

        }
        #endregion

    }
}
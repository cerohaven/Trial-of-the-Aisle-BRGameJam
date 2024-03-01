using NodeCanvas.Framework;
using System.Collections;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    public class ThrowJamActionTask : ActionTask{

        public float[] projectileSpeedAtHealthIncrements = new float[4];

        public float[] timeBetweenAttacksPerIncrement = new float[4];

        private float bossHealth;
        private Blackboard agentBlackboard;
        private SO_BossProfile bossProfile;
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
            bossProfile = agentBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
            projectileToSpawn = agentBlackboard.GetVariableValue<GameObject>("jamProjectile");

            playerTransform = agentBlackboard.GetVariableValue<Transform>("playerTransform");
            

            return null;
        }



        //This is called once each time the task is enabled.
        protected override void OnExecute()
        {


            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;


            // CALCULATE SPEED OF THE PILL //
            bossHealth = agentBlackboard.GetVariableValue<float>("bossHealth");

            int currentPhase = agentBlackboard.GetVariableValue<int>("bossPhase");
            pillSpeed = HelperFunctions.ProjectileSpeedAtPhase(bossProfile, currentPhase);

            endActionRoutine = EndActionTask(HelperFunctions.TimeBetweenAttacksAtPhase(bossProfile, currentPhase));
            StartCoroutine(endActionRoutine);
            


            // SPAWNING THE PILL GAME OBJECT //
            GameObject jam = GameObject.Instantiate(projectileToSpawn);

            //Setting the trajectory of the pill game object
            //Get rigidbody component and set direction and speed
            Vector3 dir = playerTransform.position - agent.transform.position;

            dir.Normalize();

            Projectile_Jam projectileJam = jam.GetComponent<Projectile_Jam>();
            ApplyInitializations(projectileJam, dir, jam);



            //Spawn in 3 pills at a time when the boss gets low
            if (currentPhase == bossProfile.B_BossPhases.Length)
            {
                SpawnMultiplePills(dir, projectileToSpawn);
            }


        }

        private void ApplyInitializations(Projectile_Jam projectileJam, Vector3 dir, GameObject jam)
        {
            jam.transform.position = agent.transform.position + (dir * 3.5f);
            projectileJam.InitializeProjectile(dir, pillSpeed, agent.transform, WhoThrew.Boss);
            projectileJam.IgnoreBossCollision(true);

            projectileJam.IgnoreProjectiles(true, 0);
            projectileJam.EnableDrag(0, 2);

        }

        public void SpawnMultiplePills(Vector3 directionTravelling, GameObject jamPrefab)
        {

            for (int i = 0; i < 2; i++)
            {
                GameObject tempJam;
                Projectile_Jam tempProjectile;

                //spawn object and get rigidbody
                tempJam = MakeJamCopy(jamPrefab);
                tempProjectile = tempJam.GetComponent<Projectile_Jam>();

                //If i == 0, make the split angle positive, if not, make it negative
                SplitAngleInDegrees = i == 0 ? SplitAngleInDegrees : -SplitAngleInDegrees;

                //Make a new Rotated vector from the main's vector
                Vector2 newVec = new Vector2(directionTravelling.x * Mathf.Cos(SplitAngleInDegrees * Mathf.Deg2Rad) - directionTravelling.y * Mathf.Sin(SplitAngleInDegrees * Mathf.Deg2Rad),
                                             directionTravelling.x * Mathf.Sin(SplitAngleInDegrees * Mathf.Deg2Rad) + directionTravelling.y * Mathf.Cos(SplitAngleInDegrees * Mathf.Deg2Rad));
                newVec.Normalize();

                //Add the force
                ApplyInitializations(tempProjectile, newVec, tempJam);

            }
        }

        private GameObject MakeJamCopy(GameObject original)
        {
            Projectile_Jam newJam = GameObject.Instantiate(original).GetComponent<Projectile_Jam>();
            return newJam.gameObject;
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
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace NodeCanvas.Tasks.Actions{

	public class ThrowProjectileActionTask : ActionTask{

		private Blackboard agentBlackboard;
        private SO_BossProfile bossProfile;

		private GameObject projectileToSpawn;

		private Transform playerTransform;
		private float pillSpeed;


		private IEnumerator endActionRoutine;

        //Split Pill Variables
        public float SplitAngleInDegrees = 10;


        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit(){

			//Getting blackboar Variables
            agentBlackboard = agent.GetComponent<Blackboard>();
            bossProfile = agentBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
     
            playerTransform = agentBlackboard.GetVariableValue<Transform>("playerTransform");



            return null;
		}



		//This is called once each time the task is enabled.
		protected override void OnExecute(){

            //Get the pill speed and time between attacks based on the current phase we're in
            int currentPhase = agentBlackboard.GetVariableValue<int>("bossPhase");
            
            pillSpeed = HelperFunctions.ProjectileSpeedAtPhase(bossProfile, currentPhase);
            endActionRoutine = EndActionTask(HelperFunctions.TimeBetweenAttacksAtPhase(bossProfile, currentPhase));
            StartCoroutine(endActionRoutine);


            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;



            // SPAWNING THE PROJECTILE GAME OBJECT //
            projectileToSpawn = HelperFunctions.RNGProjectile(bossProfile);
            GameObject spawnedProjectile = GameObject.Instantiate(projectileToSpawn);

            //Setting the trajectory of the projectile game object
            //Get rigidbody component and set direction and speed
            Vector3 dir = playerTransform.position - agent.transform.position;
            
            dir.Normalize();
            
			Projectile projectile = spawnedProjectile.GetComponent<Projectile>();
            ApplyInitializations(projectile, dir, spawnedProjectile);

            //Final Phase
            if(currentPhase  == bossProfile.B_BossPhases.Length)
            {
                SpawnMultiplePills(dir, projectileToSpawn);
            }
            

        }

        private void ApplyInitializations(Projectile projectile, Vector3 dir, GameObject spawnedProjectile)
        {
            
            
            spawnedProjectile.transform.position = agent.transform.position + (dir * 3.5f);
            projectile.InitializeProjectile(dir, pillSpeed, agent.transform, WhoThrew.Boss);
            projectile.IgnoreBossCollision(true);

            projectile.IgnoreProjectiles(true, 0);
            projectile.IgnoreProjectiles(false, 0.5f);
            projectile.EnableDrag(0, 2);

        }

        public void SpawnMultiplePills(Vector3 directionTravelling, GameObject pillPrefab)
        {

            for (int i = 0; i < 2; i++)
            {
                GameObject tempProj;
                Projectile tempProjectile;

                //spawn object and get rigidbody
                tempProj = MakePillCopy(pillPrefab);
                tempProjectile = tempProj.GetComponent<Projectile>();

                //If i == 0, make the split angle positive, if not, make it negative
                SplitAngleInDegrees = i == 0 ? SplitAngleInDegrees : -SplitAngleInDegrees;

                //Make a new Rotated vector from the main's vector
                Vector2 newVec = new Vector2(directionTravelling.x * Mathf.Cos(SplitAngleInDegrees * Mathf.Deg2Rad) - directionTravelling.y * Mathf.Sin(SplitAngleInDegrees * Mathf.Deg2Rad),
                                             directionTravelling.x * Mathf.Sin(SplitAngleInDegrees * Mathf.Deg2Rad) + directionTravelling.y * Mathf.Cos(SplitAngleInDegrees * Mathf.Deg2Rad));
                newVec.Normalize();

                //Add the force
                ApplyInitializations(tempProjectile, newVec, tempProj);

            }
        }

        private GameObject MakePillCopy(GameObject original)
        {
            Projectile newPill = GameObject.Instantiate(original).GetComponent<Projectile>();
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
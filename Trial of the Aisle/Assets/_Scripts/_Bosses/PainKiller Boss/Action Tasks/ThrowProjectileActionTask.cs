using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace NodeCanvas.Tasks.Actions{

	public class ThrowProjectileActionTask : ActionTask{

		private Blackboard agentBlackboard;
        private SO_BossProfile bossProfile;

        private EntityHealth _entityHealth;
        private Collider2D bossCollider;



		private IEnumerator endActionRoutine;

        private ProjectileSpawner_Boss projectileSpawner;


        
        protected override string OnInit(){

			//Getting blackboar Variables
            agentBlackboard = agent.GetComponent<Blackboard>();
            bossProfile = agentBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
            bossCollider = agent.GetComponent<Collider2D>();
            _entityHealth = agent.GetComponent<EntityHealth>();
            projectileSpawner = agent.GetComponent<ProjectileSpawner_Boss>();

            return null;
		}



		
		protected override void OnExecute(){

            //Get the pill speed and time between attacks based on the current phase we're in
            int currentPhase = _entityHealth.CurrentPhase;
            

            endActionRoutine = EndActionTask(HelperFunctions.TimeBetweenAttacksAtPhase(bossProfile, currentPhase));
            StartCoroutine(endActionRoutine);

            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;


            // SPAWNING THE PROJECTILE GAME OBJECT //

            projectileSpawner.ProjPattern.Clear();
            projectileSpawner.ProjPattern.Add(HelperFunctions.GetProjectilePatternAtPhase(bossProfile, currentPhase));
            
            projectileSpawner.SpawnProjectiles();

        }


        //REMMINDER: FIND A WAY TO APPLY INITIALIZATIONS FROM ANOTHER SCRIPT
        private void ApplyInitializations(Projectile projectile, GameObject spawnedProjectile)
        {
            
            StartCoroutine(projectile.EnableDragCoroutine(0, 2));

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
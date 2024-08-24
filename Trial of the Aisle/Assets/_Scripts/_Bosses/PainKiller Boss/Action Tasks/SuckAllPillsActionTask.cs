using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

    public class SuckAllPillsActionTask : ActionTask{

        public float maxPillSpeed;
		public float minPillSpeed;

		private Blackboard agentBlackboard;
		private Projectile_PainKiller[] pillProjectiles;
		private Collider2D bossCollider;
        
		
		private FMOD.Studio.EventInstance ParacetamaniaInhaleInstance;

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit(){
			agentBlackboard = agent.GetComponent<Blackboard>();
			bossCollider = agent.GetComponent<Collider2D>();
            return null;
		
        }

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
            pillProjectiles = GameObject.FindObjectsOfType<Projectile_PainKiller>();
			blackboard.SetVariableValue("groundedPills", pillProjectiles.Length-5);
           
			for (int i = 0; i < pillProjectiles.Length; i++)
            {
				if (pillProjectiles[i].WhoThrew == WhoThrew.Player) continue;

				pillProjectiles[i].IsBeingSuckedIn = true;
                pillProjectiles[i].IgnoreBossCollision(false, bossCollider);
                Vector2 direction = agent.transform.position - pillProjectiles[i].transform.position;
                direction.Normalize();
                float speed = Random.Range(minPillSpeed,maxPillSpeed);
				
                pillProjectiles[i].InitializeProjectile(direction, speed, agent.transform, WhoThrew.Boss);

                ParacetamaniaInhaleInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_PK/B_Parasitomania_Inhale");
                ParacetamaniaInhaleInstance.start();
            }
        }

		//Called once per frame while the action is active.
		protected override void OnUpdate(){
			
			
		}

		//Called when the task is disabled.
		protected override void OnStop(){
			
		}

		//Called when the task is paused.
		protected override void OnPause(){
			
		}
	}
}
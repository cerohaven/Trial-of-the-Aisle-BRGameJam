using NodeCanvas.Framework;
using ParadoxNotion.Design;

using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class ThrowPillActionTask : ActionTask{

		
		public float chanceToSpawnPainKillerPill;
		private Blackboard agentBlackboard;
		private GameObject painkillerPillGO;
		private GameObject energyPillGO;
		private GameObject pillToSpawn;

		private Transform playerTransform;
		private int pillsThrown;
		private float pillSpeed;

		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit(){
			agentBlackboard = agent.GetComponent<Blackboard>();

			painkillerPillGO = agentBlackboard.GetVariableValue<GameObject>("painkillerPill");
            energyPillGO = agentBlackboard.GetVariableValue<GameObject>("energyPill");
            playerTransform = agentBlackboard.GetVariableValue<Transform>("playerTransform");
			pillsThrown = agentBlackboard.GetVariableValue<int>("pillsThrown");
            pillSpeed = agentBlackboard.GetVariableValue<float>("pillSpeed");
            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){

			agentBlackboard.SetVariableValue("pillsThrown", pillsThrown++);

            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			//float Random Chance. If number is >chance, then energy, else painkiller
			float rand = Random.Range(0.00f, 1.00f);

			if(rand <= chanceToSpawnPainKillerPill)
			{
				pillToSpawn = painkillerPillGO;
			}
			else
			{
				pillToSpawn = energyPillGO;
			}

			//Spawning in the pill game object
			GameObject pill = GameObject.Instantiate(pillToSpawn);
			pill.transform.position = agent.transform.position;

            //Setting the trajectory of the pill game object
            
			//Get rigidbody component and set direction and speed
            Rigidbody2D rb = pill.GetComponent<Rigidbody2D>();
			Vector2 dir = playerTransform.position - agent.transform.position;
			dir.Normalize();

			rb.velocity = dir * pillSpeed;


            EndAction(true);
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
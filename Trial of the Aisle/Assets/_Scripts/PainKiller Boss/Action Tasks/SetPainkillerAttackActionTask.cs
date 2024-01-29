using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class SetPainkillerAttackActionTask : ActionTask{

		private Blackboard agentBlackboard;

		protected override string OnInit(){
            agentBlackboard = agent.GetComponent<Blackboard>();

            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
			float randomNumber = Random.Range(0.0f, 1.0f);
			if(randomNumber < 0.5f)
			{
				agentBlackboard.SetVariableValue("randomAttack", "Paracetamania");

            }
			else
			{
                agentBlackboard.SetVariableValue("randomAttack", "BadHabit");
            }

			//Set the pills Thrown to 0
			Debug.Log("Setting");
			agentBlackboard.SetVariableValue("pillsThrown", 0);
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
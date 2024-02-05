using NodeCanvas.Framework;
using ParadoxNotion.Design;

using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class ThrowMinionGrapeActionTask : ActionTask{

		private Blackboard agentBlackboard;
		private Vector3 playerPos;
		private Vector3 bossPos;
		private GameObject grapeMinion;

		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit(){
			agentBlackboard = agent.GetComponent<Blackboard>();
			grapeMinion = agentBlackboard.GetVariableValue<GameObject>("grapeMinion");

            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){

			playerPos = agentBlackboard.GetVariableValue<Transform>("playerTransform").position;
            bossPos = agent.transform.position;

			//Spawn the grape Game Object there
			GameObject grape = GameObject.Instantiate(grapeMinion, bossPos, Quaternion.identity);
			MinionThrow minionThrow = grape.GetComponent<MinionThrow>();
			minionThrow.BossPos = bossPos;
			minionThrow.PlayerPos = playerPos;
			minionThrow.Boss = agent.gameObject;
            LeanTween.scale(agent.gameObject, Vector3.one * 0.9f, 0.1f).setEaseInOutQuad().setOnComplete(Testing);
            //Get the script and set the radius to half
            EndAction(true);
		}
        private void Testing()
        {
            LeanTween.scale(agent.gameObject, Vector3.one, 0.1f);

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
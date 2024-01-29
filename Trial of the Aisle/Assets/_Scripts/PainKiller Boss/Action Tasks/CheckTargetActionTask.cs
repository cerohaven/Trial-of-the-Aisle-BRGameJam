using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;
namespace NodeCanvas.Tasks.Actions{

	public class CheckTargetActionTask : ActionTask{

		private Blackboard agentBlackboard;

		protected override string OnInit(){
            agentBlackboard = agent.GetComponent<Blackboard>();

            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){

			
			List<Transform> targetList = agentBlackboard.GetVariableValue <List<Transform>> ("waypoints");
			Transform currentTarget = agentBlackboard.GetVariableValue<Transform>("targetWaypoint");
			Transform randomTransform = null;
            bool unique = false;

			while(!unique)
			{
                randomTransform = targetList[Random.Range(0, targetList.Count)];

				if(randomTransform != currentTarget || currentTarget == null)
				{
					unique = true;
				}
            }
			
            agentBlackboard.SetVariableValue("targetWaypoint", randomTransform);

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
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions{

	public class ReachedTargetConditionTask : ConditionTask{

		public float threshold;
		
		private Vector2 targetPos;

		private Blackboard agentBlackboard;

		protected override string OnInit(){
			agentBlackboard = agent.GetComponent<Blackboard>();
			return null;
		}

		protected override void OnEnable(){
			targetPos = agentBlackboard.GetVariableValue<Transform>("targetWaypoint").position;
		}

		protected override void OnDisable(){
			
		}

		protected override bool OnCheck(){

			//Get the distance between the boss and the target
			//if the distance is less than a certain threshold, continue
			Vector2 distance = targetPos - (Vector2)agent.transform.position;
			
			if (distance.magnitude < threshold)
				return true;

			else
				return false;

		}
	}
}
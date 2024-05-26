using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions{

	public class SetBaseSpeedActionTask : ActionTask{

		Blackboard agentBlackboard;

		protected override string OnInit(){
			agentBlackboard = agent.GetComponent<Blackboard>();

			//set the speed
			SO_BossProfile bossProfile = agentBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
			agentBlackboard.SetVariableValue("bossSpeed", bossProfile.B_BaseMoveSpeed);
			return null;
		}

		
		protected override void OnExecute(){
			EndAction(true);
		}

		
		protected override void OnUpdate(){
			
		}

		
		protected override void OnStop(){
			
		}

		protected override void OnPause(){
			
		}
	}
}
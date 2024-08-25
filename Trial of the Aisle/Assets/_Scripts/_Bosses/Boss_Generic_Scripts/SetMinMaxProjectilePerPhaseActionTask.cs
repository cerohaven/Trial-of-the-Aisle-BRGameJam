using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions {

	public class SetMinMaxProjectilePerPhaseActionTask : ActionTask {

		public MinMaxPhase[] phaseProjectileThrowCount;
        private EntityHealth _entityHealth;
		private Blackboard agentBlackboard;

        protected override string OnInit() {
            _entityHealth = agent.GetComponent<EntityHealth>();
            agentBlackboard = agent.GetComponent<Blackboard>();
            return null;
		}


		protected override void OnExecute() {
            int currentPhase = _entityHealth.CurrentPhase;
			int maxArray = Mathf.Min(currentPhase, phaseProjectileThrowCount.Length - 1);
			blackboard.SetVariableValue("minProjectiles", phaseProjectileThrowCount[maxArray].minProjs);
            blackboard.SetVariableValue("maxProjectiles", phaseProjectileThrowCount[maxArray].maxProjs);

            EndAction(true);
		}


	}

	[System.Serializable]
	public class MinMaxPhase
	{
		public int minProjs;
		public int maxProjs;
	}
}
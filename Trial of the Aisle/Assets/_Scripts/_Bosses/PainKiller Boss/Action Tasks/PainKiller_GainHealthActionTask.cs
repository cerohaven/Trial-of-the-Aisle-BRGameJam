using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class PainKiller_GainHealthActionTask : ActionTask{
		public ChangeHealth healAmount;
		private EntityHealth _entityHealth;
		private FMOD.Studio.EventInstance BadHabitInstance;
		
		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit(){
            _entityHealth = agent.GetComponent<EntityHealth>();
			
            return null;
        	

		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
			_entityHealth.HealUnit(healAmount);
            LeanTween.scale(agent.gameObject, Vector3.one * 1.1f, 0.1f).setOnComplete(Testing);
			agent.GetComponent<SwapMaterialDemo>().Swap(1);

            BadHabitInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_PK/B_Bad Habit");
            BadHabitInstance.start();
        }
		private void Testing()
		{
			LeanTween.scale(agent.gameObject, Vector3.one, 0.1f).setOnComplete(End);
           
        }

		private void End()
		{
            agent.GetComponent<SwapMaterialDemo>().Swap(0);
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
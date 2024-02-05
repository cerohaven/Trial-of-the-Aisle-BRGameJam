using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class PainKiller_GainHealthActionTask : ActionTask{
		public SO_AdjustHealth adjustHealth;
		public ChangeHealth healAmount;

		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit(){
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
			adjustHealth.ChangeBossHealthEventSend(healAmount, HealthType.Healing, new UnityEngine.Vector2(0,0));
            LeanTween.scale(agent.gameObject, Vector3.one * 1.1f, 0.1f).setOnComplete(Testing);
			agent.GetComponent<SwapMaterialDemo>().Swap(1);
            
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
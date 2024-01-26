using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	public class SetColourActionTask : ActionTask{
		public SpriteRenderer[] agentSpriteRenderer;
		public Color[] clrToChangeTo;


		protected override string OnInit(){
			
			return null;
		}

		protected override void OnExecute(){
			for(int i = 0; i < agentSpriteRenderer.Length; i++)
            {
				agentSpriteRenderer[i].color = clrToChangeTo[i];
			}
			
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
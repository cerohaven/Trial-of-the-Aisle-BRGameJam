using NodeCanvas.Framework;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class Boss_SetAnim_ActionTask : ActionTask{

		public int animState;
		private Animator animator;
		public bool triggerAttack;

		protected override string OnInit(){
			animator = agent.GetComponentInChildren<Animator>();
			
            return null;
		}

		protected override void OnExecute(){
			animator.SetInteger("animState", animState);
			animator.SetBool("triggerAttack", triggerAttack);

            EndAction(true);
		}

	
	}
}
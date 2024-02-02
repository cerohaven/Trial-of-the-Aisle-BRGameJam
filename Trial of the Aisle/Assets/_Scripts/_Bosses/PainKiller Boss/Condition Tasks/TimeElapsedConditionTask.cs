using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	public class TimeElapsedConditionTask : ConditionTask
	{
		public float minWaitTime;
		public float maxWaitTime;
		private float timeElapsed;
		private float waitTime;

		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit(){
			return null;
		}

		//Called whenever the condition gets enabled.
		protected override void OnEnable(){
			timeElapsed = 0.0f;

			if(minWaitTime == maxWaitTime)
			{
				waitTime = maxWaitTime;
            }
			else
			{
				waitTime = Random.Range(minWaitTime, maxWaitTime);
			}

		}

		//Called whenever the condition gets disabled.
		protected override void OnDisable(){
			
		}

		//Called once per frame while the condition is active.
		//Return whether the condition is success or failure.
		protected override bool OnCheck(){
      		timeElapsed += Time.deltaTime;
			return timeElapsed > waitTime;
		}
	}
}
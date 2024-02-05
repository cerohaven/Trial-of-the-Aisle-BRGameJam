using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class SetGrapeAttackActionTask : ActionTask{

		public float chanceToGetAttack1 = 0.6f;

		private Blackboard agentBlackboard;
		private float bossHealth;
		private float maxBossHealth;

		private const string attack1Name = "Fruits of Fury";
        private const string attack2Name = "Jammed";
        protected override string OnInit(){
            agentBlackboard = agent.GetComponent<Blackboard>();

            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){
			bossHealth = agentBlackboard.GetVariableValue<float>("bossHealth");
            maxBossHealth = agentBlackboard.GetVariableValue<float>("bossMaxHealth");


            int timesUsedAttack = blackboard.GetVariableValue<int>("timesUsedAttack");
			string previousAttack = agentBlackboard.GetVariableValue<string>("randomAttack");

			float randomNumber = Random.Range(0.0f, 1.0f);

            //Set the new attack but make sure the attack can't happen more than twice in a row

            // ATTACK 1 - PAARACETAMANIA //
            if (randomNumber < chanceToGetAttack1)
			{
				//Check to see if they performed 2 paracetamanias in a row. If so, then do other attack
				if(previousAttack == attack1Name)
				{
                    timesUsedAttack++;
					if(timesUsedAttack > 2)
					{
						timesUsedAttack = 0;
                        agentBlackboard.SetVariableValue("randomAttack", attack2Name);
                    }
                }
				else
				{
                    agentBlackboard.SetVariableValue("randomAttack", attack1Name);
                }
				
            }

			// ATTACK 2 - BAD HABIT //
			else
			{

                //Check to see if they already performed 2 bad habits in a row. If so, then do other attack
                if (previousAttack == attack2Name)
                {
                    timesUsedAttack++;
                    if (timesUsedAttack > 2)
                    {
                        timesUsedAttack = 0;
                        agentBlackboard.SetVariableValue("randomAttack", attack1Name);
                    }
                }
                else
                {
                    agentBlackboard.SetVariableValue("randomAttack", attack2Name);
                }
            }



            blackboard.SetVariableValue("timesUsedAttack", timesUsedAttack);

            //Set the pills Thrown to 0
            agentBlackboard.SetVariableValue("objectsThrown", 0);
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
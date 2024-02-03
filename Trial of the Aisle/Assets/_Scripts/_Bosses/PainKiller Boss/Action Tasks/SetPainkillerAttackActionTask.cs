using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class SetPainkillerAttackActionTask : ActionTask{

		public float chanceToGetAttack1 = 0.6f;

		private Blackboard agentBlackboard;
		private float bossHealth;
		private float maxBossHealth;

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
				if(previousAttack == "Paracetamania")
				{
                    timesUsedAttack++;
					if(timesUsedAttack > 2)
					{
						timesUsedAttack = 0;
                        agentBlackboard.SetVariableValue("randomAttack", "BadHabit");
                    }
                }
				else
				{
                    agentBlackboard.SetVariableValue("randomAttack", "Paracetamania");
                }
				
            }

			// ATTACK 2 - BAD HABIT //
			else
			{

				//Only perform Bad Habit if the boss isn't at full health
				if(bossHealth / maxBossHealth > 75/ maxBossHealth)
				{
                    agentBlackboard.SetVariableValue("randomAttack", "Paracetamania");
                }

                //Check to see if they already performed 2 bad habits in a row. If so, then do other attack
                else if (previousAttack == "BadHabit")
                {
                    timesUsedAttack++;
                    if (timesUsedAttack > 2)
                    {
                        timesUsedAttack = 0;
                        agentBlackboard.SetVariableValue("randomAttack", "Paracetamania");
                    }
                }
                else
                {
                    agentBlackboard.SetVariableValue("randomAttack", "BadHabit");
                }
            }



            blackboard.SetVariableValue("timesUsedAttack", timesUsedAttack);

            //Set the pills Thrown to 0
            agentBlackboard.SetVariableValue("pillsThrown", 0);
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
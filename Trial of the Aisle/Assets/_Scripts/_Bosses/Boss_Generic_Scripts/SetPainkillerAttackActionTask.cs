using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class SetRandomAttackActionTask : ActionTask{

		private Blackboard agentBlackboard;
        private SO_BossProfile bossProfile;

		protected override string OnInit(){
            agentBlackboard = agent.GetComponent<Blackboard>();
            bossProfile = agentBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");
            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){

            //int timesUsedAttack = blackboard.GetVariableValue<int>("timesUsedAttack");


            bool choseAttack = false;
            int randAbility = 0;

            while (!choseAttack)
            {
                //get a random ability
                randAbility = HelperFunctions.RNGAttack(bossProfile);

                bool atkCondition = false; //Checks to see if we can use the attack based on the special condition (if any)
                //bool canUseAttackTime = false; //chcks to see if can repeat the attack again


                /// CHECKING TO SEE IF THE ATTACK HAS A SPECIAL CONDITION ///

                //check to see if we can use the attack if it has a special condition
                if (bossProfile.B_BossAttacks[randAbility].attackCondition != null)
                {
                    atkCondition = 
                    bossProfile.B_BossAttacks[randAbility].attackCondition.OnCheckAttackCondition(agentBlackboard) ? 
                    true : false;
                }
                else
                {
                    atkCondition = true;
                }

                

                //Now check to see if we can perform the attack if we didn't use it more than twice in a row

                //if we're good, then set chose Attack to true and get out of while loop
                if(atkCondition)
                {
                    choseAttack = true;
                }
            }

            //Set the new attack
            agentBlackboard.SetVariableValue("randomAttack", bossProfile.B_BossAttacks[randAbility].attackName);
           

            //blackboard.SetVariableValue("timesUsedAttack", timesUsedAttack);

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
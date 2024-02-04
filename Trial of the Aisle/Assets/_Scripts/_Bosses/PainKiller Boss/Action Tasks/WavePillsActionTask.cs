using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class WavePillsActionTask : ActionTask{

		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		public float timeBetweenWaves;
        public float chanceToSpawnPainKillerPill;
        public float pillSpeed;

        public int waveAmount = 7;
        private int currentWaveAmount = 0;

        private float timeElapsed;
        private float bossHealth;
        private float bossMaxHealth;

        private Blackboard agentBlackboard;
        private GameObject painkillerPillGO;
        private GameObject energyPillGO;
        private GameObject pillToSpawn;

        
        
		private float[] angles1 = new float[8];
        private float[] angles2 = new float[8];

        private float[] pillAngles = new float[8];
        protected override string OnInit(){
            angles1[0] = 0;
            angles1[1] = 90;
            angles1[2] = 180;
            angles1[3] = 270;
            angles1[4] = 45;
            angles1[5] = 135;
            angles1[6] = 225;
            angles1[7] = 315;

            angles2[0] = 22.5f;
            angles2[1] = 67.5f;
            angles2[2] = 112.5f;
            angles2[3] = 157.5f;
            angles2[4] = 202.5f;
            angles2[5] = 247.5f;
            angles2[6] = 292.5f;
            angles2[7] = 337.5f;

            agentBlackboard = agent.GetComponent<Blackboard>();

            painkillerPillGO = agentBlackboard.GetVariableValue<GameObject>("painkillerPill");
            energyPillGO = agentBlackboard.GetVariableValue<GameObject>("energyPill");
           
            bossMaxHealth = agentBlackboard.GetVariableValue<float>("bossMaxHealth");

            pillAngles = angles1;
            return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute(){

            bossHealth = agentBlackboard.GetVariableValue<float>("bossHealth");

            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            currentWaveAmount = 0;


        }

		//Called once per frame while the action is active.
		protected override void OnUpdate(){
            timeElapsed += Time.deltaTime;

            if (timeElapsed > timeBetweenWaves)
			{

                currentWaveAmount++;
                timeElapsed = 0;

				//Spawn Pills
                for(int i = 0; i < 8; i++)
                {
                    SpawnWave(pillAngles[i]);
                }

                //Switch the wave's pill spawn angle
                if (pillAngles[0] == 0)
                {
                    pillAngles = angles2;
                }
                else
                {
                    pillAngles = angles1;
                }


                if (currentWaveAmount >= waveAmount)
                {
                    EndAction(true);
                }
            }

           
            
		}

		private void SpawnWave(float _angle)
		{
            //Make sure that these pills have no drag and continue until they hit something and then they disapear.
            //MAKE non-carryable pills Coated/outlined in a different colour than the regular pills.

            
            //float Random Chance. If number is >chance, then energy, else painkiller
            float rand = Random.Range(0.00f, 1.00f);

            if (rand <= chanceToSpawnPainKillerPill)
            {
                pillToSpawn = painkillerPillGO;
            }
            else
            {
                pillToSpawn = energyPillGO;
            }

            //Spawning in the pill game object
            GameObject pill = GameObject.Instantiate(pillToSpawn);
            Projectile_Pill projectilePill = pill.GetComponent<Projectile_Pill>();

            //Setting the trajectory of the pill game object

            //Get rigidbody component and set direction and speed
            Vector3 dir = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad), Mathf.Sin(_angle * Mathf.Deg2Rad));

            //Set Position
            pill.transform.position = agent.transform.position;



            projectilePill.InitializeProjectile(dir, pillSpeed, agent.transform, WhoThrew.Boss);
            projectilePill.IgnoreBossCollision(true);
            projectilePill.IgnoreProjectiles(true, 0);
            projectilePill.IsThrownInWave = true;

            //Calculate turn intensity
            if(bossHealth < 75)
            {
                float turnIntensity = (bossMaxHealth / bossHealth) / 2;

                projectilePill.TurnIntensity = turnIntensity;
            }
           

        }

		//Called when the task is disabled.
		protected override void OnStop(){
			
		}

		//Called when the task is paused.
		protected override void OnPause(){
			
		}
	}
}
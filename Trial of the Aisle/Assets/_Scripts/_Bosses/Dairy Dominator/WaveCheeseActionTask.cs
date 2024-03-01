using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class WaveCheeseActionTask : ActionTask{

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        public int projectilesToSpawn;
        public float moonRadius;

        public float timeBetweenWaves;
        public float cheeseSpeed;

        public int waveAmount = 7;

        public float turnIntensityMultiplier = 4;

        private Blackboard agentBlackboard;

        private GameObject cheeseGO;
        public GameObject centerGO;
        private GameObject center;

        private IEnumerator waveCoroutine;

        protected override string OnInit()
        {
        
            agentBlackboard = agent.GetComponent<Blackboard>();

            cheeseGO = agentBlackboard.GetVariableValue<GameObject>("waveProjectile");


            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {

            int currentPhase = agentBlackboard.GetVariableValue<int>("bossPhase");

            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            center = GameObject.Instantiate(centerGO, agent.transform);

            //Change speed of center rotating
         
            center.GetComponent<RotateObject>().RotateSpeed = currentPhase * turnIntensityMultiplier;


            waveCoroutine = Wave();
            StartCoroutine(waveCoroutine);

        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {
           
        }
        IEnumerator Wave()
        {

            for (int j = 0; j < projectilesToSpawn; j++)
            {
                Vector2 randomPointInCircle = (Vector2)agent.transform.position + Random.insideUnitCircle * moonRadius;

                GameObject.Instantiate(cheeseGO, randomPointInCircle, Quaternion.identity, center.transform);

                yield return new WaitForSeconds(timeBetweenWaves);
            }
            

            EndAction(true);
        }
      

        //Called when the task is disabled.
        protected override void OnStop()
        {

        }

        //Called when the task is paused.
        protected override void OnPause()
        {

        }
    }
}
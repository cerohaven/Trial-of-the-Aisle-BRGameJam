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

        private float timeElapsed;
        private float bossHealth;
        private float bossMaxHealth;

        private Blackboard agentBlackboard;
        private GameObject cheeseGO;
        public GameObject centerGO;
        private GameObject center;

        private IEnumerator waveCoroutine;

        protected override string OnInit()
        {
        
            agentBlackboard = agent.GetComponent<Blackboard>();

            cheeseGO = agentBlackboard.GetVariableValue<GameObject>("waveProjectile");

            bossMaxHealth = agentBlackboard.GetVariableValue<float>("bossMaxHealth");

            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {

            bossHealth = agentBlackboard.GetVariableValue<float>("bossHealth");

            //Set the boss' velocity to none so they don't continue moving
            agent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            center = GameObject.Instantiate(centerGO, agent.transform);

            //Change speed of center rotating
            float turnIntensity = (bossMaxHealth / bossHealth) * 20;
            center.GetComponent<RotateObject>().RotateSpeed = turnIntensity;


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

                GameObject go = GameObject.Instantiate(cheeseGO, randomPointInCircle, Quaternion.identity, center.transform);
                Projectile_Cheese cheesee = go.GetComponent<Projectile_Cheese>();
                cheesee.IgnoreWallLayer();
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            

            EndAction(true);
        }
        private void SpawnWave(float _angle)
        {

            //Spawning in the pill game object
            GameObject cheese = GameObject.Instantiate(cheeseGO);
            Projectile_Cheese projectileCheese = cheese.GetComponent<Projectile_Cheese>();
            projectileCheese.IsThrownInWave = true;
            //Setting the trajectory of the pill game object

            //Get rigidbody component and set direction and speed
            Vector3 dir = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad), Mathf.Sin(_angle * Mathf.Deg2Rad));

            //Set Position
            cheese.transform.position = agent.transform.position;

            projectileCheese.InitializeProjectile(dir, cheeseSpeed + (bossMaxHealth / bossHealth) / 3, agent.transform, WhoThrew.Boss);
            projectileCheese.IgnoreBossCollision(true);
            projectileCheese.IgnoreProjectiles(true, 0);

            

            //Calculate turn intensity
      
            float turnIntensity = (bossMaxHealth / bossHealth) / 2;

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
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class ThrowHomingFetaActionTask : ActionTask{

        private Blackboard agentBlackboard;
        private Transform playerTransform;
        private Vector3 bossPos;
        private GameObject fetaCheestGO;

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit()
        {
            agentBlackboard = agent.GetComponent<Blackboard>();
            fetaCheestGO = agentBlackboard.GetVariableValue<GameObject>("fetaCheese");

            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {

            playerTransform = agentBlackboard.GetVariableValue<Transform>("playerTransform");
            bossPos = agent.transform.position;

            //Spawn the grape Game Object there
            //Spawn a homing Feta starting at a random direction (transform.up = unitCircle) 
            GameObject feta = GameObject.Instantiate(fetaCheestGO, bossPos, Quaternion.identity);
            HomingFeta homingFeta = feta.GetComponent<HomingFeta>();

            homingFeta.PlayerTransform = playerTransform;
            homingFeta.transform.up = Random.insideUnitCircle;
            LeanTween.scale(agent.gameObject, Vector3.one * 0.9f, 0.1f).setEaseInOutQuad().setOnComplete(Testing);
           
            EndAction(true);
        }
        private void Testing()
        {
            LeanTween.scale(agent.gameObject, Vector3.one, 0.1f);

        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {


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
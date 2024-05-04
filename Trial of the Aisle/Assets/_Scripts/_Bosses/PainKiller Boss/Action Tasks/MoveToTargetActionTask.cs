using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	public class MoveToTargetActionTask : ActionTask{

		private Blackboard agentBlackboard;
		private Rigidbody2D _rb;
		private float _speed;
		private Vector2 _targetPoint;
		private Vector2 _currentPosition;
		private Vector3 _dir;



		protected override string OnInit(){

			agentBlackboard = agent.GetComponent<Blackboard>();

			_rb = agent.GetComponent<Rigidbody2D>();
			_speed = agentBlackboard.GetVariableValue<SO_BossProfile>("bossProfile").B_BaseMoveSpeed;
            
			

			return null;
		}


		protected override void OnExecute(){



            //Get the target waypoint
            _targetPoint = agentBlackboard.GetVariableValue<Transform>("targetWaypoint").position;

			

           
        }

		//Called once per frame while the action is active.
		protected override void OnUpdate(){
            _currentPosition = agent.transform.position;
            _dir = _targetPoint - _currentPosition;
            _dir.Normalize();

            //update their position to go to the deisred location
            _rb.MovePosition(agent.transform.position + _dir * _speed * Time.fixedDeltaTime);
		}

		//Called when the task is disabled.
		protected override void OnStop(){
			
		}

		//Called when the task is paused.
		protected override void OnPause(){
			
		}
	}
}
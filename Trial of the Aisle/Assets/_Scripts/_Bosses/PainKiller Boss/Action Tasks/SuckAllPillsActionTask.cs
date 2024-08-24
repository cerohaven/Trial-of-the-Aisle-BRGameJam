using Cinemachine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

    public class SuckAllPillsActionTask : ActionTask{

        public float maxPillSpeed;
		public float minPillSpeed;

		private Projectile_PainKiller[] pillProjectiles;
        private CinemachineTargetGroup targetGroup;


        private FMOD.Studio.EventInstance ParacetamaniaInhaleInstance;

        
        protected override string OnInit(){
            return null;
		
        }

		
		protected override void OnExecute(){

            //Zoom out the camera
            targetGroup = GameObject.FindObjectOfType<CinemachineTargetGroup>();
            targetGroup.m_Targets[1].radius = 13;
            
            pillProjectiles = GameObject.FindObjectsOfType<Projectile_PainKiller>();
			blackboard.SetVariableValue("groundedPills", pillProjectiles.Length-5);
           
			for (int i = 0; i < pillProjectiles.Length; i++)
            {
				if (pillProjectiles[i].WhoThrew == WhoThrew.Player) continue;

				pillProjectiles[i].IsBeingSuckedIn = true;
                Vector2 direction = agent.transform.position - pillProjectiles[i].transform.position;
                direction.Normalize();
                float speed = Random.Range(minPillSpeed,maxPillSpeed);
				
                pillProjectiles[i].InitializeProjectile(direction, speed, agent.transform, WhoThrew.Boss);

                ParacetamaniaInhaleInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_PK/B_Parasitomania_Inhale");
                ParacetamaniaInhaleInstance.start();

            }

            
            
        }

        protected override void OnStop()
        {
            base.OnStop();
            targetGroup.m_Targets[1].radius = 7;
            Debug.Log("Stopped Method");
        }

        protected override void OnPause()
        {
            base.OnPause();
            targetGroup.m_Targets[1].radius = 7;
            Debug.Log("Pasued Method");
        }
    }
}
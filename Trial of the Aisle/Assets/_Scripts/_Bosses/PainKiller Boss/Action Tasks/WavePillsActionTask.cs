using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	public class WavePillsActionTask : ActionTask{


        public SO_ProjectilePattern[] patternBasedOnPhase;
        private ProjectileSpawner_Boss _projectileSpawner;

		public float waveDuration;



        private float timeElapsed;
        private int bossPhase;


        private EntityHealth _entityHealth;
        
        protected override string OnInit(){

            _projectileSpawner = agent.GetComponent<ProjectileSpawner_Boss>();
            _entityHealth = agent.GetComponent<EntityHealth>();


            StopSuckingPills();

            return null;
		}

        private void StopSuckingPills()
        {
            Projectile_PainKiller[] pillProjectiles = GameObject.FindObjectsOfType<Projectile_PainKiller>();
           
            for (int i = 0; i < pillProjectiles.Length; i++)
            {
                if (pillProjectiles[i].WhoThrew != WhoThrew.Boss) continue;
                if (pillProjectiles[i].IsBeingSuckedIn == false) continue;

                pillProjectiles[i].IsBeingSuckedIn = false;

                StartCoroutine(pillProjectiles[i].EnableDragCoroutine(0, 0, 5));


            }
        }

		
		protected override void OnExecute(){
            bossPhase = _entityHealth.CurrentPhase;

            //Depending on the Boss Phase play a specific Wave Pill attack
            _projectileSpawner.ProjPattern.Clear();
            _projectileSpawner.ProjPattern.Add(patternBasedOnPhase[bossPhase]);

            _projectileSpawner.SpawnProjectiles(true);

        }

		
		protected override void OnUpdate(){

            timeElapsed += Time.deltaTime;

            if (timeElapsed > waveDuration)
			{
                 EndAction(true);

            }

           
            
		}


	}
}
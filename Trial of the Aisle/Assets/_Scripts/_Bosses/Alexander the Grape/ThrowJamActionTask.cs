using NodeCanvas.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    public class ThrowJamActionTask : ActionTask{
        
        public List<SO_ProjectilePattern[]> jammedProjectilePatternStack;
        private ProjectileSpawner_Boss _projectileSpawner;
        private Blackboard agentBlackboard;
        private GameObject projectileToSpawn;
        private EntityHealth _entityHealth;

        private FMOD.Studio.EventInstance JammedInstance;
        protected override string OnInit()
        {
            _projectileSpawner = agent.GetComponent<ProjectileSpawner_Boss>();
            agentBlackboard = agent.GetComponent<Blackboard>();
            projectileToSpawn = agentBlackboard.GetVariableValue<GameObject>("jamProjectile");
            _entityHealth = agent.GetComponent<EntityHealth>();

            return null;
        }
        protected override void OnExecute()
        {
            int currentPhase = _entityHealth.CurrentPhase;

            //Depending on the Boss Phase play a specific Wave Pill attack
            _projectileSpawner.ProjPattern.Clear();
            
            for (int i = 0; i < jammedProjectilePatternStack[currentPhase].Length; i++)
            {
                _projectileSpawner.ProjPattern.Add(jammedProjectilePatternStack[currentPhase][i]);
            }


            JammedInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Bosses/Boss_AtG/B_Jammed");
            JammedInstance.start();

            _projectileSpawner.SpawnProjectiles(false, true, projectileToSpawn);
            EndAction();
        }

    }
}
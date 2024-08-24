using NodeCanvas.Framework;
using System.Collections;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    public class ThrowJamActionTask : ActionTask{
        
        public SO_ProjectilePattern[] jammedProjectilePatternStack;
        private ProjectileSpawner_Boss _projectileSpawner;
        private Blackboard agentBlackboard;
        private GameObject projectileToSpawn;

        protected override string OnInit()
        {
            _projectileSpawner = agent.GetComponent<ProjectileSpawner_Boss>();
            agentBlackboard = agent.GetComponent<Blackboard>();
            projectileToSpawn = agentBlackboard.GetVariableValue<GameObject>("jamProjectile");

            return null;
        }
        protected override void OnExecute()
        {

            //Depending on the Boss Phase play a specific Wave Pill attack
            _projectileSpawner.ProjPattern.Clear();
            
            for (int i = 0; i < jammedProjectilePatternStack.Length; i++)
            {
                _projectileSpawner.ProjPattern.Add(jammedProjectilePatternStack[i]);
            }
            

            _projectileSpawner.SpawnProjectiles(true, true, projectileToSpawn);
            EndAction();
        }

    }
}
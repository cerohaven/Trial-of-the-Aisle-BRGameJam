using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectilePatterns
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] private SO_ProjectilePattern[] projPattern;
        [SerializeField] private GameObject projectileGO;
        [SerializeField] private Transform playerTransform;


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < projPattern.Length; i++)
                {
                    List<PatternTypeMod[]> projs = projPattern[i].GetProjectilePatterns();

                    foreach (PatternTypeMod[] mod in projs)
                    {

                        StartCoroutine(SpawnBulletCoroutine(mod[4].modValue, mod));

                    }
                }

            }

        }

        private IEnumerator SpawnBulletCoroutine(float delayTime, PatternTypeMod[] mod)
        {
            yield return new WaitForSeconds(delayTime);
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            bool isTargettingPlayer = PAT_HelperFunctions.IsTargettingPlayer(mod[0]);
            float angle = isTargettingPlayer ? -99 : mod[1].modValue;

            Vector2 direction = PAT_HelperFunctions.GetDirectionFromAngle(angle, mod[3].modValue, playerTransform);
            Vector2 directionPlusSpawnOffset = direction * mod[5].modValue;

            GameObject go = Instantiate(projectileGO, mouseWorldPosition + directionPlusSpawnOffset, Quaternion.identity);
            
            go.GetComponent<TestProjectile>().Initialize(direction, mod[2].modValue, mod[6].modValue);

            yield break;
        }

       
    }
}
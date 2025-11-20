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

                        StartCoroutine(SpawnBulletCoroutine(mod[3].modValue, mod));

                    }
                }

            }

        }

        private IEnumerator SpawnBulletCoroutine(float delayTime, PatternTypeMod[] mod)
        {
            yield return new WaitForSeconds(delayTime);
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 direction = PAT_HelperFunctions.GetDirectionFromAngle(mod[0].modValue, mod[2].modValue, playerTransform);
            Vector2 directionPlusSpawnOffset = direction * mod[4].modValue;

            GameObject go = Instantiate(projectileGO, mouseWorldPosition + directionPlusSpawnOffset, Quaternion.identity);

            go.GetComponent<TestProjectile>().Initialize(direction, mod[1].modValue);

            yield break;
        }

       
    }
}
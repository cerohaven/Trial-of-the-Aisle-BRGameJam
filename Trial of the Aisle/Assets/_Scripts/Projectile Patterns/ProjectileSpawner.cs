using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private SO_ProjectilePattern projPattern;
    [SerializeField] private GameObject projectileGO;
    [SerializeField] private Transform playerTransform;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            List<PatternTypeMod[]> projs = projPattern.GetProjectilePatterns();

            foreach (PatternTypeMod[] mod in projs)
            {
                

                StartCoroutine(SpawnBulletCoroutine(mod[3].modValue,  mod));

            }

        }
    }

    private IEnumerator SpawnBulletCoroutine(float delayTime,  PatternTypeMod[] mod)
    {
        yield return new WaitForSeconds(delayTime);
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        GameObject go = Instantiate(projectileGO, mouseWorldPosition, Quaternion.identity);

        go.GetComponent<TestProjectile>().Initialize(GetDirectionFromAngle(mod[0].modValue, mod[2].modValue), mod[1].modValue);

        yield break;
    }

    private Vector2 GetDirectionFromAngle(float angle, float extraAngle)
    {
        Vector2 dir = new Vector2();

        if (angle == -99)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //Move this gameObject around the player
            dir = (Vector2)playerTransform.position - mouseWorldPosition;
            
            dir.Normalize();

            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Debug.Log(extraAngle);
            dir = new Vector2(Mathf.Cos((a + extraAngle) * Mathf.Deg2Rad),
                              Mathf.Sin((a + extraAngle) * Mathf.Deg2Rad));
            

        }
        else
        {
            dir = new Vector2(Mathf.Cos((angle + extraAngle) * Mathf.Deg2Rad),
                                   Mathf.Sin((angle + extraAngle)  * Mathf.Deg2Rad));
        }


        

        return dir;
    }
}

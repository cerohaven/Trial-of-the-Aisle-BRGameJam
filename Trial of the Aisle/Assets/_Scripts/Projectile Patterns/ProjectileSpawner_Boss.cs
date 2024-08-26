using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner_Boss : MonoBehaviour
{
    [SerializeField] private List<SO_ProjectilePattern> projPattern;
    private Transform playerTransform;

    //Properties
    public List<SO_ProjectilePattern> ProjPattern { get => projPattern; set => projPattern = value; }

    private void Start()
    {
        playerTransform = GameManager.Instance.PlayerTransform;
    }

    public void SpawnProjectiles(bool inWave = false, bool shouldApplyDrag = false, GameObject spawnSpecificGO = null)
    {
        for (int i = 0; i < projPattern.Count; i++)
        {
            List<PatternTypeMod[]> projs = projPattern[i].GetProjectilePatterns();

            foreach (PatternTypeMod[] mod in projs)
            {
                if(spawnSpecificGO == null)
                {
                    StartCoroutine(SpawnBulletCoroutine(inWave, shouldApplyDrag, mod[3].modValue, mod));
                }
                else
                {
                    StartCoroutine(SpawnBulletCoroutine(inWave, shouldApplyDrag, mod[3].modValue, mod, spawnSpecificGO));
                }

                GameManager.Instance.CurrentProjectilesInScene++;
            }
            
        }
    }



    private IEnumerator SpawnBulletCoroutine(bool inWave, bool shouldApplyDrag, float delayTime, PatternTypeMod[] mod)
    {
        yield return new WaitForSeconds(delayTime);

        Vector2 direction = GetDirectionFromAngle(mod[0].modValue, mod[2].modValue);

        GameObject go = Instantiate(HelperFunctions.RNGProjectile(GameManager.Instance.BossProfile),  //Get a random projectile from the list in to Boss Profile
                                    (Vector2)transform.position + direction * mod[4].modValue,        //Position of boss + an offset distance away from the boss
                                    Quaternion.identity);

        //Initializing the values
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.InitializeProjectile(direction, mod[1].modValue, transform, WhoThrew.Boss);

        BulletConditions(projectile, inWave, shouldApplyDrag);

        yield break;
    }

 


    private IEnumerator SpawnBulletCoroutine(bool inWave, bool shouldApplyDrag, float delayTime, PatternTypeMod[] mod, GameObject prefabToSpawn)
    {
        yield return new WaitForSeconds(delayTime);

        Vector2 direction = GetDirectionFromAngle(mod[0].modValue, mod[2].modValue);

        GameObject go = Instantiate(prefabToSpawn,  
                                    (Vector2)transform.position + direction * mod[4].modValue,        //Position of boss + an offset distance away from the boss
                                    Quaternion.identity);

        //Initializing the values
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.InitializeProjectile(direction, mod[1].modValue, transform, WhoThrew.Boss);

        BulletConditions(projectile, inWave, shouldApplyDrag);

        yield break;
    }



    private void BulletConditions(Projectile projectile, bool inWave, bool shouldApplyDrag)
    {
        bool destroyOnWall = GameManager.Instance.CurrentProjectilesInScene > 10 ? true : false;

        if(destroyOnWall || inWave)
        {
            DestroyOnWall(projectile);
        }
        if((!destroyOnWall && !inWave) || shouldApplyDrag)
        {
            SetDrag(projectile);
        }
     
        
    }
    private void DestroyOnWall(Projectile projectile)
    {
        projectile.ShouldDestroyOnWall = true;
    }

    private void SetDrag(Projectile projectile)
    {
        StartCoroutine(projectile.EnableDragCoroutine(0.5f, 2.5f));
    }


    private Vector2 GetDirectionFromAngle(float angle, float extraAngle)
    {
        Vector2 dir = new Vector2();

        if (angle == -99)
        {
            //Move this gameObject around the player
            dir = (Vector2)playerTransform.position - (Vector2)transform.position;

            dir.Normalize();

            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            dir = new Vector2(Mathf.Cos((a + extraAngle) * Mathf.Deg2Rad),
                              Mathf.Sin((a + extraAngle) * Mathf.Deg2Rad));


        }
        else
        {
            dir = new Vector2(Mathf.Cos((angle + extraAngle) * Mathf.Deg2Rad),
                                   Mathf.Sin((angle + extraAngle) * Mathf.Deg2Rad));
        }




        return dir;
    }
}

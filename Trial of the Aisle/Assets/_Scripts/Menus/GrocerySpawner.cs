using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrocerySpawner : MonoBehaviour
{
    [SerializeField] private float timeForSpawn;
    [SerializeField] private GameObject[] objectsToSpawn;

    private IEnumerator spawnGroceryCoroutine;

    void Start()
    {
        spawnGroceryCoroutine = SpawnGroceryCorouitne();

        if (objectsToSpawn.Length == 0) return;

        StartCoroutine(spawnGroceryCoroutine);
    }

    private IEnumerator SpawnGroceryCorouitne()
    {
        while(true)
        {
            Instantiate(objectsToSpawn[Random.Range(0,objectsToSpawn.Length)], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(timeForSpawn);
        }
        
    }
   
}

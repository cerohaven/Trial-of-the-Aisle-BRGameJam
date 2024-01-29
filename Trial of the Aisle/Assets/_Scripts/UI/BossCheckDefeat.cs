using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    ///This script Checks to see if it recieves an event from the 'BossHealthBar' class
    ///that the health is 0.

    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;

    public ObjectsToSpawnIn[] objectsToSpawnIn;

    private void Awake()
    {
        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);
    }

    private void DestroyBoss()
    {
        //spawn objects if we want to
        for (int i = 0; i < objectsToSpawnIn.Length; i++)
        {
            Vector3 spawnLocation = GetSpawnLocation(objectsToSpawnIn[i].spawnPosition);
            Instantiate(objectsToSpawnIn[i].gameObjectToSpawn, spawnLocation, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private Vector3 GetSpawnLocation(SpawnType spawnPos)
    {
        //based on what the designer wants, we can spawn certain objects in specific spaces
        if (spawnPos == SpawnType.WORLD_SPAWN)
            return Vector3.zero;
        else if (spawnPos == SpawnType.BOSS_POSITION)
            return transform.position;
        else
            return Vector3.zero;
    }
}

[System.Serializable]
public class ObjectsToSpawnIn
{
    public GameObject gameObjectToSpawn;
    public SpawnType spawnPosition;
}

public enum SpawnType
{
    WORLD_SPAWN,
    BOSS_POSITION
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;
    public ObjectsToSpawnIn[] objectsToSpawnIn;
    public KeyCode debugSpawnKey = KeyCode.Space; // Define the debug key (Space key in this case)

    private void Awake()
    {
        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);
    }

    private void Update()
    {
        // Check if the debug key is pressed
        if (Input.GetKeyDown(debugSpawnKey))
        {
            DestroyBoss();
        }
    }

    private void DestroyBoss()
    {
        SpawnObjects();
        Destroy(gameObject);
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < objectsToSpawnIn.Length; i++)
        {
            Vector3 spawnLocation = GetSpawnLocation(objectsToSpawnIn[i].spawnPosition);
            Instantiate(objectsToSpawnIn[i].gameObjectToSpawn, spawnLocation, Quaternion.identity);
        }
    }

    private Vector3 GetSpawnLocation(SpawnType spawnPos)
    {
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;
    [SerializeField] private List<MonoBehaviour> bossAbilityComponents; // Change to MonoBehaviour list
    private LevelLoader levelLoader;

    public ObjectsToSpawnIn[] objectsToSpawnIn;
    public KeyCode debugSpawnKey = KeyCode.Space;

    private void Awake()
    {
        levelLoader = GameObject.FindObjectOfType<LevelLoader>();

        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);
    }

    private void DestroyBoss()
    {

        try
        {
            UnlockBossAbilities(); // Make sure this method is safely handling nulls and other potential issues
            SpawnObjects(); // Ensure this method doesn't rely on the boss GameObject after it's destroyed
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while executing DestroyBoss: {e.Message}");
        }

        Debug.Log("Destroying boss GameObject.");
        GameManager.gameEnded = true;


        levelLoader.SetTrigger();
        Destroy(gameObject); // Destroy the boss GameObject last to ensure all cleanup is done before this


    }
    

    private void UnlockBossAbilities()
    {
        //unimplemented
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
        switch (spawnPos)
        {
            case SpawnType.WORLD_SPAWN:
                return Vector3.zero;
            case SpawnType.BOSS_POSITION:
                return transform.position;
            default:
                return Vector3.zero;
        }
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

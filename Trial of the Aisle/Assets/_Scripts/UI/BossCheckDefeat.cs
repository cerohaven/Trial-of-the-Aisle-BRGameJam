using NodeCanvas.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    private Blackboard bossBlackboard;
    private SO_BossProfile bossProfile;


    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;

    private LevelLoader levelLoader;

    public ObjectsToSpawnIn[] objectsToSpawnIn;
    public KeyCode debugDefeatBossKey = KeyCode.K; // Assign a key for debug defeat

    private void Awake()
    {
        bossBlackboard = GetComponent<Blackboard>();
        bossProfile = bossBlackboard.GetVariableValue<SO_BossProfile>("bossProfile");

        levelLoader = FindObjectOfType<LevelLoader>();
        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);

    }

    private void Update()
    {
        if (Input.GetKeyDown(debugDefeatBossKey))
        {
            Debug.Log("Debug: Boss defeated");
            DestroyBoss();
        }
    }

    private void DestroyBoss()
    {
        try
        {
            SpawnObjects();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while executing DestroyBoss: {e.Message}");
        }

  
        Debug.Log("Destroying boss GameObject.");
        GameManager.gameEnded = true;
        Destroy(gameObject);
    }

    private void SpawnObjects()
    {
        foreach (var item in objectsToSpawnIn)
        {
            Vector3 spawnLocation = GetSpawnLocation(item.spawnPosition);
            GameObject spawnedObj = Instantiate(item.gameObjectToSpawn, spawnLocation, Quaternion.identity);

            //This is specific to the post battle Canvas
            NewAbilitySelectionUI newUI = spawnedObj.GetComponent < NewAbilitySelectionUI>();

            if (newUI == null) continue;

            newUI.ShowAbilities(bossProfile.Ability1, bossProfile.Ability2, bossProfile.PostBattleCanvasUI);
        }
    }

    private Vector3 GetSpawnLocation(SpawnType spawnPos)
    {
        return spawnPos switch
        {
            SpawnType.WORLD_SPAWN => Vector3.zero,
            SpawnType.BOSS_POSITION => transform.position,
            _ => Vector3.zero,
        };
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

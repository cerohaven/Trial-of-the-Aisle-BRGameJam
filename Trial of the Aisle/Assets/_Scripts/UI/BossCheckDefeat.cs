using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;
    [SerializeField] private List<MonoBehaviour> bossAbilityComponents; // Change to MonoBehaviour list
    [SerializeField] private AbilityManager abilityManager; // Reference to the AbilityManager

    public ObjectsToSpawnIn[] objectsToSpawnIn;
    public KeyCode debugSpawnKey = KeyCode.Space;

    private void Awake()
    {
        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);
        if (abilityManager == null)
        {
            abilityManager = FindObjectOfType<AbilityManager>();
            if (abilityManager == null)
            {
                Debug.LogError("AbilityManager not found in the scene.");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(debugSpawnKey))
        {
            DestroyBoss();
        }
    }

    private void DestroyBoss()
    {
        if (abilityManager == null)
        {
            Debug.LogError("AbilityManager reference is missing in BossCheckDefeat.");
            return; // Abort if there's no reference to the AbilityManager
        }

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
        Destroy(gameObject); // Destroy the boss GameObject last to ensure all cleanup is done before this
    }


    private void UnlockBossAbilities()
    {
        if (abilityManager == null)
        {
            Debug.LogError("AbilityManager is null when trying to unlock boss abilities.");
            return;
        }

        foreach (var abilityComponent in bossAbilityComponents)
        {
            if (abilityComponent == null)
            {
                Debug.LogError("One of the boss ability components is null.");
                continue; // Skip the null component
            }
            abilityManager.UnlockAbility(abilityComponent);
        }
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

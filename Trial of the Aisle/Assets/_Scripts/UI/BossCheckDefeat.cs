using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;
    [SerializeField] private List<MonoBehaviour> bossAbilityComponents; // List of boss ability components
    [SerializeField] private GameObject postBattleCanvas; // Reference to the post-battle canvas
    [SerializeField] private NewAbilitySelectionUI abilitySelectionUI; // Reference to the ability selection UI
    [SerializeField] private Ability[] newAbilitiesAfterDefeat; // Abilities offered after defeat, updated to use Ability objects

    private LevelLoader levelLoader;

    public ObjectsToSpawnIn[] objectsToSpawnIn;
    public KeyCode debugDefeatBossKey = KeyCode.K; // Assign a key for debug defeat

    private void Awake()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);
        postBattleCanvas.SetActive(false);
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

        ShowPostBattleUI();
        TriggerAbilitySelection();

        Debug.Log("Destroying boss GameObject.");
        GameManager.gameEnded = true;
        Destroy(gameObject);
    }

    private void ShowPostBattleUI()
    {
        if (postBattleCanvas != null)
        {
            postBattleCanvas.SetActive(true); // Enable the PostBattleCanvas
        }
    }

    private void TriggerAbilitySelection()
    {
        // Ensure there are at least 2 new abilities to offer
        if (newAbilitiesAfterDefeat.Length >= 2)
        {
            abilitySelectionUI.ShowAbilities(newAbilitiesAfterDefeat[0], newAbilitiesAfterDefeat[1]);
        }
        else
        {
            Debug.LogWarning("Not enough new abilities specified for post-boss defeat selection.");
        }
    }

    private void SpawnObjects()
    {
        foreach (var item in objectsToSpawnIn)
        {
            Vector3 spawnLocation = GetSpawnLocation(item.spawnPosition);
            Instantiate(item.gameObjectToSpawn, spawnLocation, Quaternion.identity);
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

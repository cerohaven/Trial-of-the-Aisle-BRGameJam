using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckDefeat : MonoBehaviour
{
    [SerializeField] private SO_BossDefeatedEventSender bossDefeatedEventSender;
    [SerializeField] private List<MonoBehaviour> bossAbilityComponents; // List of boss ability components
    [SerializeField] private GameObject postBattleCanvas; // Reference to the post-battle canvas

    private LevelLoader levelLoader;

    public ObjectsToSpawnIn[] objectsToSpawnIn;
    public KeyCode debugSpawnKey = KeyCode.Space;
    public KeyCode debugDefeatBossKey = KeyCode.K; // Assign a key for debug defeat

    private void Awake()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        bossDefeatedEventSender.bossIsDefeatedEvent.AddListener(DestroyBoss);
        postBattleCanvas.SetActive(false);
    }

    private void Update()
    {
        // Check if the debug key for defeating the boss is pressed
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
            SpawnObjects(); // Ensure this method doesn't rely on the boss GameObject after it's destroyed
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while executing DestroyBoss: {e.Message}");
        }

        ShowPostBattleUI(); // Show the post-battle UI before destroying the boss

        Debug.Log("Destroying boss GameObject.");
        GameManager.gameEnded = true;

        //levelLoader.SetTrigger();
        Destroy(gameObject); // Destroy the boss GameObject last to ensure all cleanup is done before this
    }

    private void ShowPostBattleUI()
    {
        if (postBattleCanvas != null)
        {
            postBattleCanvas.SetActive(true); // Enable the PostBattleCanvas
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

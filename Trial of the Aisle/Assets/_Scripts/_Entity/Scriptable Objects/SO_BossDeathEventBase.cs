using UnityEngine;


[CreateAssetMenu(fileName = "Boss Death Event", menuName = "Boss Scriptable Objects/Events/Boss Death Event Default")]
public class SO_BossDeathEventBase : SO_EntityHealthEventBase
{
    [SerializeField] private ObjectsToSpawnIn[] spawnOnDefeat;

    private GameObject attachedGameObject;

    public override void Initialize(GameObject gameObject)
    {
        attachedGameObject = gameObject;
        SpawnObjects();
        DestroyBoss();
    }

    private void SpawnObjects()
    {
        foreach (var item in spawnOnDefeat)
        {
            Vector3 spawnLocation = GetSpawnLocation(item.spawnPosition);
            GameObject spawnedObj = Instantiate(item.gameObjectToSpawn, spawnLocation, Quaternion.identity);

        }
    }

    private Vector3 GetSpawnLocation(SpawnType spawnPos)
    {
        return spawnPos switch
        {
            SpawnType.WORLD_SPAWN => Vector3.zero,
            SpawnType.BOSS_POSITION => attachedGameObject.transform.position,
            _ => Vector3.zero,
        };
    }
    private void DestroyBoss()
    {
        GameManager.Instance.EventSender.SwitchCameraStateEventSend();
        GameManager.Instance.EventSender.BossIsDefeatedSend();

        GameManager.Instance.GameEnded = true;
        GameManager.Instance.BossIsDefeated = true;

        attachedGameObject.SetActive(false);
    }

    public override void StartEventMethod()
    {
        throw new System.NotImplementedException();
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

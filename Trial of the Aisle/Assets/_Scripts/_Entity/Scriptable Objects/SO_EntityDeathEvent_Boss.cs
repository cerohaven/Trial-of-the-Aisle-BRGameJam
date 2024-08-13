using FMODUnity;
using UnityEngine;


[CreateAssetMenu(fileName = "Boss Death Event", menuName = "Boss Scriptable Objects/Events/Boss Death Event Default")]
public class SO_EntityDeathEvent_Boss : SO_EntityHealthEventBase
{
    [SerializeField] private ObjectsToSpawnIn[] spawnOnDefeat;

    private GameObject _attachedGameObject;

    FMOD.Studio.EventInstance SFX_BossDeath;
    
    FMOD.Studio.EventInstance SFX_BossScream;

    public override void Initialize(GameObject gameObject)
    {
        
        SFX_BossDeath = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/Boss_Death");
        SFX_BossScream = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/BossScream");

        _attachedGameObject = gameObject;
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
            SpawnType.BOSS_POSITION => _attachedGameObject.transform.position,
            _ => Vector3.zero,
        };
    }
    private void DestroyBoss()
    {
        GameManager.Instance.EventSender.SwitchCameraStateEventSend();
        GameManager.Instance.EventSender.BossIsDefeatedSend();
        GameManager.Instance.EventSender.FlickerScreenSend();

        GameManager.Instance.GameEnded = true;
        GameManager.Instance.BossIsDefeated = true;

        // Play Sound Effects 
        SFX_BossDeath.start();

        SFX_BossScream.start();

        GameManager.Instance.Boss_BGM_Postbattle.start();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("SceneTransition", "PostBattleEntered");

        _attachedGameObject.SetActive(false);

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

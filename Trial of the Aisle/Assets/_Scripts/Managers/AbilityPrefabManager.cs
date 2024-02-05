using UnityEngine;
using System.Collections.Generic;

public class AbilityPrefabManager : MonoBehaviour
{
    public static AbilityPrefabManager Instance { get; private set; }

    public GameObject highVelocityShotPrefab;
    public GameObject explosiveShotPrefab;
    public GameObject healingPrefab;

    // Add other ability prefabs as needed

    private Dictionary<System.Type, GameObject> abilityPrefabs = new Dictionary<System.Type, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            InitializeAbilityPrefabs();
        }
    }

    private void InitializeAbilityPrefabs()
    {
        abilityPrefabs.Add(typeof(HighVelocityShot), highVelocityShotPrefab);
        abilityPrefabs.Add(typeof(ExplosiveShot), explosiveShotPrefab);
        abilityPrefabs.Add(typeof(HealingShot), healingPrefab);

        // Add other ability prefabs here
    }

    public GameObject GetAbilityPrefab(IAbility ability)
    {
        if (abilityPrefabs.ContainsKey(ability.GetType()))
        {
            return abilityPrefabs[ability.GetType()];
        }
        else
        {
            Debug.LogError($"Prefab for {ability.AbilityName} is missing.");
            return null;
        }
    }
}

using UnityEngine;

public class AbilityPrefabManager : MonoBehaviour
{
    public static AbilityPrefabManager Instance { get; private set; }

    public GameObject highVelocityShotPrefab;
    public GameObject ExplosiveShotPrefab;
    public GameObject HealingPrefab;

    // Add other ability prefabs as needed

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}

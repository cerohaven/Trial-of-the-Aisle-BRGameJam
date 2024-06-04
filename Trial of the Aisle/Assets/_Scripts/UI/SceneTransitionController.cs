using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public static SceneTransitionController Instance;

    //Classes
    private LevelLoader levelLoader;

    //Variables
    [SerializeField] private BossSceneTransitionProperties painKillerTransitionProperties;
    [SerializeField] private BossSceneTransitionProperties alexanderTransitionProperties;
    [SerializeField] private BossSceneTransitionProperties dairyTransitionProperties;

    //Properties
    public LevelLoader LevelLoader { get => levelLoader; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        levelLoader = GetComponentInChildren<LevelLoader>();

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("EnteredScene");

        //Lerp Colours
        StartCoroutine(levelLoader.LerpColour());
        
    }


    //Called from the Level Loader
    public BossSceneTransitionProperties GetColourToUse()
    {
        //find the boss and get the name of the boss
        string sceneName = SceneManager.GetActiveScene().name;

        switch(sceneName)
        {
            case "Boss_PainKiller":
                return painKillerTransitionProperties;

            case "Boss_AlexanderTheGrape":
                return alexanderTransitionProperties;

            case "Boss_DairyDominator":
                return dairyTransitionProperties;

            default:
                return painKillerTransitionProperties;
        }
    }
}

[System.Serializable]
public class BossSceneTransitionProperties
{
    public Texture2D _maskTexture2D;
    public Color _backgroundColour;
    public Color _itemsColour;

}

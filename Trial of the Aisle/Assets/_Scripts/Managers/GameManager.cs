using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //Scriptable Objects
    private SO_EventSender _eventSender;
    private SO_HealthAdjustments _healthAdjustments;

    //Async Loaded objects
    private PauseGameMenu pauseMenu;
    private GameObject pauseMenuPrefab;
    private SceneTransitionController sceneTransitionController;
    [SerializeField] private TransitionType transitionType;

    private PlayerInputHandler playerInputHandler;

    //Game State
    private bool canPause = false;
    private bool canMove = true;
    private bool isGamePaused = false;
    private bool gameEnded = false;
    private bool bossIsDefeated = false;

    FMOD.Studio.EventInstance SFX_BossDeath;
    FMOD.Studio.EventInstance Boss_BGM_Postbattle; 
    FMOD.Studio.EventInstance SFX_BossScream;

    //Holds the Boss Profile of this scene
    [SerializeField] private SO_BossProfile bossProfile;
    
    //Keeps a list of all the active UI on screen, so if we click ESC it gets rid of the most recent UI
    //If the List is >0, then simply close the UI and remove it from the List
    //If the List is 0, then Pause
    //If we're paused and we click again, unpause
    private List<GameObject> uiInstances = new List<GameObject>();


    public bool dragging;

    //Properties
    public List<GameObject> UiInstances { get => uiInstances; set => uiInstances = value; }
    public SO_EventSender EventSender { get => _eventSender;}
    public TransitionType TransitionType { get => transitionType; set => transitionType = value; }
    public SceneTransitionController SceneTransitionController { get => sceneTransitionController;}
    public GameObject PauseMenuPrefab { get => pauseMenuPrefab;}
    public bool CanPause { get => canPause; set => canPause = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public PlayerInputHandler PlayerInputHandler { get => playerInputHandler;}
    public bool GameEnded { get => gameEnded; set => gameEnded = value; }
    public bool BossIsDefeated { get => bossIsDefeated; set => bossIsDefeated = value; }
    public bool IsGamePaused { get => isGamePaused; set => isGamePaused = value; }
    public SO_BossProfile BossProfile { get => bossProfile; set => bossProfile = value; }
    public SO_HealthAdjustments HealthAdjustments { get => _healthAdjustments;}

    private void Awake()
    {
        DontDestroyOnLoad(this);


        AsyncOperation sceneTransitAsync = SceneManager.LoadSceneAsync("Load_SceneTransitionController", LoadSceneMode.Additive);
        AsyncOperation pauseMenuAsync = SceneManager.LoadSceneAsync("Load_PauseMenu", LoadSceneMode.Additive);
        AsyncOperation playerInputAsync = SceneManager.LoadSceneAsync("Load_PlayerInput", LoadSceneMode.Additive);

        sceneTransitAsync.completed += (AsyncOperation a) =>
        {
            sceneTransitionController = FindObjectOfType<SceneTransitionController>();
            DontDestroyOnLoad(sceneTransitionController);
        };

        pauseMenuAsync.completed += (AsyncOperation a) =>
        {
            pauseMenu = FindObjectOfType<PauseGameMenu>();
            pauseMenuPrefab = pauseMenu.gameObject;
            pauseMenuPrefab.SetActive(false);
            DontDestroyOnLoad(pauseMenuPrefab);
        };
        playerInputAsync.completed += (AsyncOperation a) =>
        {
            playerInputHandler = FindObjectOfType<PlayerInputHandler>();
            DontDestroyOnLoad(playerInputHandler.gameObject);
        };

        _eventSender = Resources.Load<SO_EventSender>("Event Sender");
        _healthAdjustments = Resources.Load<SO_HealthAdjustments>("Health Adjustments");
        //Calls when a player presses the pause button
        _eventSender.pauseGameEvent.AddListener(PauseTheGame);

        //Calls whenever a player presses the resume button
        _eventSender.resumeGameEvent.AddListener(ResumeTheGame);

        //When the boss is defeated
        _eventSender.bossIsDefeatedEvent.AddListener(IsDefeated);
    }



    private void Start()
    {
        gameEnded = false;
        //find the playerInputHandler in the game.
        //May need to move inside function if errors when someone unpluggs controller

        Boss_BGM_Postbattle = RuntimeManager.CreateInstance("event:/Music/BGM/PostBattle");
        SFX_BossDeath = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/Boss_Death");
        SFX_BossScream = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/BossScream");
    }

    //Scene Transitions
    #region IEnumerator for Exit Scene Transition
    public void LoadNextScene()
    {
        StartCoroutine(sceneTransitionController.WaitForAnimationAndLoadNextScene());
    }

    public void LoadSpecificSceneString(string sceneName)
    {
        StartCoroutine(sceneTransitionController.WaitForAnimationAndLoadSpecificScene(sceneName));
    }
   

    //Used for when the TimeScale is 0 so we have to manually play the animations since they won't play
    public void LoadSpecificSceneStringPaused(string sceneName)
    {
        StartCoroutine(sceneTransitionController.WaitForAnimationAndLoadSpecificScenePaused(sceneName));
    }

    public void LoadSpecificSceneBuildIndex(int buildIndex)
    {
        StartCoroutine(sceneTransitionController.WaitForAnimationAndLoadSpecificSceneBuildIndex(buildIndex));
    }

    #endregion

    #region Player Input Controls




    #endregion



    private void PauseTheGame()
    {
        //checks to see if we should pause the game, or remove any active UI elements. Only pause if there are no active UI elements.
        if(uiInstances.Count >0)
        {
            DestroyUIElement();
        }
        else
        {
            Pause();
        }


    }

    private void DestroyUIElement()
    {
        if (gameEnded)
        {
            Boss_BGM_Postbattle.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //if the game is ended and they destroy a UI element, that means it is the Ability Selection UI and we can load the next level
            LoadNextScene();
        }

        uiInstances[uiInstances.Count - 1].SetActive(false);
        uiInstances.RemoveAt(uiInstances.Count - 1);
    }
    private void Pause()
    {

        playerInputHandler.PlayerInput.SwitchCurrentActionMap("UI");

        //Reveal the Pause Menu
        if (!isGamePaused)
        {
            pauseMenuPrefab.SetActive(true);
        }
            

        isGamePaused = true;

        //connect all the player's inputs to that pause menu's input module
        pauseMenu.GetComponent<PauseGameMenu>().ConnectControllersToPauseMenu(playerInputHandler.PlayerInput);

        Time.timeScale = 0;

        uiInstances.Add(pauseMenuPrefab);
    }
    private void ResumeTheGame()
    {
        playerInputHandler.PlayerInput.SwitchCurrentActionMap("Player");

        isGamePaused = false;

        uiInstances.Remove(pauseMenuPrefab);


        if(pauseMenu != null)
            Destroy(pauseMenu);

        Time.timeScale = 1;
    }

    private void IsDefeated()
    {
        //Once we defeat the boss, we will do some stuff


        //AudioManager.instance.Play("ui_bossDefeated");
        SFX_BossDeath.start();

        bossIsDefeated = true;

        //Flicker Screen
        _eventSender.FlickerScreenSend();

        SFX_BossScream.start();
        Boss_BGM_Postbattle.start();
        //AudioManager.instance.Play("boss_scream");

        //the star and defeat animation is spawned in a class on the boss called 'BossCheckDefeat'
    }
}

using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ControlScheme
{
    Mouse,
    Gamepad,
    None
}

public class GameManager : Singleton<GameManager>
{

    //Scriptable Objects
    private SO_EventSender _eventSender;
    private SO_HealthAdjustments _healthAdjustments;

    //Async Loaded objects
    private PauseGameMenu pauseMenu;
    private GameObject pauseMenuPrefab;
    private SceneTransitionController sceneTransitionController;
    private GamepadCursor gamepadCursor;
    private RectTransform virtualCursorCanvas;

    private GameObject currentDragDrop;

    [SerializeField] private TransitionType transitionType;
    [SerializeField] private ControlScheme controlScheme;


    private PlayerInputHandler playerInputHandler;


    [SerializeField] private int currentProjectilesInScene = 0;


    //Game State
    private bool isInMainMenu = true;
    private bool canPause = false;
    private bool canMove = true;
    private bool isGamePaused = false;
    private bool gameEnded = false;
    private bool bossIsDefeated = false;

    public FMOD.Studio.EventInstance Boss_BGM_Postbattle;
    FMOD.Studio.EventInstance SFX_PauseEvent;
    FMOD.Studio.EventInstance SFX_UnPauseEvent;

    //Holds the Boss Profile of this scene
    [SerializeField] private SO_BossProfile bossProfile;
    [SerializeField] private Transform _bossTransform;
    [SerializeField] private Transform _playerTransform;
    
    //Keeps a list of all the active UI on screen, so if we click ESC it gets rid of the most recent UI
    //If the List is >0, then simply close the UI and remove it from the List
    //If the List is 0, then Pause
    //If we're paused and we click again, unpause
    private List<GameObject> uiInstances = new List<GameObject>();


    public bool dragging, dropped;

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
    public Transform BossTransform { get => _bossTransform; set => _bossTransform = value; }
    public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }
    public GamepadCursor GamepadCursor { get => gamepadCursor; set => gamepadCursor = value; }
    public ControlScheme ControlScheme { get => controlScheme; set => controlScheme = value; }
    public int CurrentProjectilesInScene { get => currentProjectilesInScene; set => currentProjectilesInScene = value; }
    public bool IsInMainMenu { get => isInMainMenu; set => isInMainMenu = value; }

    public GameObject CurrentDragDrop { get => currentDragDrop; set => currentDragDrop = value; }


    
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
            gamepadCursor = FindObjectOfType<GamepadCursor>();
            virtualCursorCanvas = gamepadCursor.CanvasRectTransform;
            gamepadCursor.GamepadPlayerInput = playerInputHandler.PlayerInput;

            DontDestroyOnLoad(playerInputHandler.gameObject);
            DontDestroyOnLoad(gamepadCursor.gameObject);
            DontDestroyOnLoad(virtualCursorCanvas);
        };

        _eventSender = Resources.Load<SO_EventSender>("Event Sender");
        _healthAdjustments = Resources.Load<SO_HealthAdjustments>("Health Adjustments");


        //Calls when a player presses the pause button
        _eventSender.pauseGameEvent.AddListener(PauseTheGame);

        //Calls whenever a player presses the resume button
        _eventSender.resumeGameEvent.AddListener(ResumeTheGame);

    }


    

    private void Start()
    {
        gameEnded = false;

        Boss_BGM_Postbattle = RuntimeManager.CreateInstance("event:/Music/BGM/PostBattle");


    }

    #region Public Methods
    public void FreezePlayerMovement()
    {
        canMove = false;
    }

    public void UnFreezePlayerMovement()
    {
        canMove = true;
    }

    #endregion


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


    #region Pause Game Methods
    private void PauseTheGame()
    {
        if(isInMainMenu)
        {
            return;
        }

        if(!bossIsDefeated)
            Pause();

    }

    private void GoToNextBossScene()
    {
        //if the game is ended and they destroy a UI element, that means it is the Post Battle Canvas UI and we can load the next level
        if (gameEnded)
        {
            if (gamepadCursor != null) gamepadCursor.EnableCursor(false); 
           

            Boss_BGM_Postbattle.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            transitionType = TransitionType.BossBattle;
            
            LoadNextScene();
        }

    }
    private void Pause()
    {
        Debug.Log("Clicked Pause");
        if (sceneTransitionController.GetSceneName().Equals("MainMenu")) return;
           
        if (bossIsDefeated) return;

        SFX_PauseEvent = RuntimeManager.CreateInstance("event:/UI/Buttons/Pause");
        SFX_PauseEvent.start();

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
    }
    private void ResumeTheGame()
    {
        Debug.Log("Clicked UnPause");
        if (bossIsDefeated)
        {
            GoToNextBossScene();
            return;
        }

        if (sceneTransitionController.GetSceneName().Equals("MainMenu")) return;
        playerInputHandler.PlayerInput.SwitchCurrentActionMap("Player");

        GeneralResumeLogic();
        if(controlScheme == ControlScheme.Gamepad)
        {
            gamepadCursor.EnableCursor(false);
        }   
  
        SFX_UnPauseEvent = RuntimeManager.CreateInstance("event:/UI/Buttons/Unpause");
        SFX_UnPauseEvent.start();

    }
    public void GeneralResumeLogic()
    {
        isGamePaused = false;

        if (pauseMenu != null)
            pauseMenuPrefab.SetActive(false);

        Time.timeScale = 1;
    }
    #endregion

}

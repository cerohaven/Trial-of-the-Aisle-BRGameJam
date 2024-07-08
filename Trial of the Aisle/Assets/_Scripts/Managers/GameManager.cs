using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //Scriptable Objects
    private SO_EventSender _eventSender;

    //Async Loaded objects
    private PauseGameMenu pauseMenu;
    private GameObject pauseMenuPrefab;
    private SceneTransitionController sceneTransitionController;
    [SerializeField] private TransitionType transitionType;


    //Game State
    private bool canPause = false;
    public static bool isGamePaused;

    private PlayerInput playerInput;
    

    FMOD.Studio.EventInstance SFX_BossDeath;
    FMOD.Studio.EventInstance Boss_BGM_Postbattle; 
    FMOD.Studio.EventInstance SFX_BossScream;


    //Boss Defeated Variables
    public static bool gameEnded = false;
    public static bool bossIsDefeated = false;

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

    private void Awake()
    {
        DontDestroyOnLoad(this);


        AsyncOperation sceneTransitAsync = SceneManager.LoadSceneAsync("Load_SceneTransitionController", LoadSceneMode.Additive);
        AsyncOperation pauseMenuAsync = SceneManager.LoadSceneAsync("Load_PauseMenu", LoadSceneMode.Additive);

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



        _eventSender = Resources.Load<SO_EventSender>("Event Sender");

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
        playerInput = GameObject.FindObjectOfType<PlayerInput>();

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

        playerInput.SwitchCurrentActionMap("UI");

        //Reveal the Pause Menu
        if (!isGamePaused)
        {
            pauseMenuPrefab.SetActive(true);
        }
            

        isGamePaused = true;

        //connect all the player's inputs to that pause menu's input module
        pauseMenu.GetComponent<PauseGameMenu>().ConnectControllersToPauseMenu(playerInput);

        Time.timeScale = 0;

        uiInstances.Add(pauseMenuPrefab);
    }
    private void ResumeTheGame()
    {
        playerInput.SwitchCurrentActionMap("Player");

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

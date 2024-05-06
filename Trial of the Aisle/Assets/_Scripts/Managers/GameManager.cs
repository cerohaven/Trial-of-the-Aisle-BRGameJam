using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private SO_PauseMenuEventSender pauseMenuEvent;
    [SerializeField] private GameObject pauseMenuPrefab;

    public static bool isGamePaused;

    private PlayerInput playerInput;
    FMOD.Studio.EventInstance SFX_BossDeath;
    FMOD.Studio.EventInstance SFX_BossScream;
    private GameObject pauseMenu;


    //Boss Defeated Variables
    public static bool gameEnded = false;
    public static bool bossIsDefeated = false;

    //Keeps a list of all the active UI on screen, so if we click ESC it gets rid of the most recent UI
    //If the List is >0, then simply close the UI and remove it from the List
    //If the List is 0, then Pause
    //If we're paused and we click again, unpause
    private List<GameObject> uiInstances = new List<GameObject>();


    //Scriptable Objects
    [SerializeField] private SO_BossDefeatedEventSender SObossDefeat;

    //Properties

    public List<GameObject> UiInstances { get => uiInstances; set => uiInstances = value; }

    public bool dragging;



    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Calls when a player presses the pause button
        pauseMenuEvent.pauseGameEvent.AddListener(PauseTheGame);

        //Calls whenever a player presses the resume button
        pauseMenuEvent.resumeGameEvent.AddListener(ResumeTheGame);

        //When the boss is defeated
        SObossDefeat.bossIsDefeatedEvent.AddListener(IsDefeated);
    }

    private void Start()
    {
        gameEnded = false;
        //find the playerInputHandler in the game.
        //May need to move inside function if errors when someone unpluggs controller
        playerInput = GameObject.FindObjectOfType<PlayerInput>();
        SFX_BossDeath = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/Boss_Death");
        SFX_BossScream = RuntimeManager.CreateInstance("event:/SFX/Bosses/General/BossScream");
    }
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
        if(gameEnded)
        {
            //if the game is ended and they destroy a UI element, that means it is the Ability Selection UI and we can load the next level
            LevelLoader.Instance.LoadNextScene();
        }

        Destroy(uiInstances[uiInstances.Count-1]);
        uiInstances.RemoveAt(uiInstances.Count - 1);
    }
    private void Pause()
    {

        playerInput.SwitchCurrentActionMap("UI");

        //Spawn in the pause menu ONLY IF IT'S THE FIRST TIME
        if (!isGamePaused)
            pauseMenu = Instantiate(pauseMenuPrefab);

        isGamePaused = true;

        //connect all the player's inputs to that pause menu's input module
        pauseMenu.GetComponent<PauseGameMenu>().ConnectControllersToPauseMenu(playerInput);

        Time.timeScale = 0;

        uiInstances.Add(pauseMenu);
    }
    private void ResumeTheGame()
    {
        playerInput.SwitchCurrentActionMap("Player");

        isGamePaused = false;

        uiInstances.Remove(pauseMenu);


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
        SObossDefeat.FlickerScreenSend();

        SFX_BossScream.start();
        //AudioManager.instance.Play("boss_scream");

        //the star and defeat animation is spawned in a class on the boss called 'BossCheckDefeat'
    }
}

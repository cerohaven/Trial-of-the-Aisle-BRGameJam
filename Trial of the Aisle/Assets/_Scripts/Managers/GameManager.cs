using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SO_PauseMenuEventSender pauseMenuEvent;
    [SerializeField] private GameObject pauseMenuPrefab;

    public static bool isGamePaused;

    private PlayerInput playerInput;

    private GameObject pauseMenu;


    //Boss Defeated Variables
    public static bool gameEnded = false;
    public static bool bossIsDefeated = false;


    //Scriptable Objects
    [SerializeField] private SO_BossDefeatedEventSender SObossDefeat;

    private void Awake()
    {

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
    }
    private void PauseTheGame()
    {
        playerInput.SwitchCurrentActionMap("UI");
   
        //Spawn in the pause menu ONLY IF IT'S THE FIRST TIME
        if (!isGamePaused)
            pauseMenu = Instantiate(pauseMenuPrefab);

        isGamePaused = true;

        //connect all the player's inputs to that pause menu's input module
        pauseMenu.GetComponent<PauseGameMenu>().ConnectControllersToPauseMenu(playerInput);

        Time.timeScale = 0;
    }

    private void ResumeTheGame()
    {
        playerInput.SwitchCurrentActionMap("Player");

        isGamePaused = false;

        if(pauseMenu != null)
            Destroy(pauseMenu);

        Time.timeScale = 1;
    }

    private void IsDefeated()
    {
        //Once we defeat the boss, we will do some stuff

        AudioManager.instance.Play("ui_bossDefeated");

        bossIsDefeated = true;

        //Flicker Screen
        SObossDefeat.FlickerScreenSend();

        AudioManager.instance.Play("d_scream");

        //the star and defeat animation is spawned in a class on the boss called 'BossCheckDefeat'
    }
}

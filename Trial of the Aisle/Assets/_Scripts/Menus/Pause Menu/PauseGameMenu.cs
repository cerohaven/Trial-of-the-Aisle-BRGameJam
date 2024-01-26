using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGameMenu : MonoBehaviour
{
    /// <summary>
    /// The Pause Menu Prefab is instantiated into the scene from the 'PlayerInputHandler' when a player
    /// clicks the pause button.
    /// The 'GameManager' is in charge of checking to see if the game is paused or not.
    /// </summary>

    //References
    private PlayerController pc;

    //components
    [SerializeField] private SO_PauseMenuEventSender pauseMenuEvent;

    [SerializeField] private Button resumeButton;

   

    private void Awake()
    {
        pc = GameObject.FindObjectOfType<PlayerController>();
        //hover over the 'resume button' on startup
        if (pc.GetCurrentControlScheme() == "Controller")
            resumeButton.Select();
    }

    public void ConnectControllersToPauseMenu(PlayerInput player)
    {
        //Get all the playuer's inputUI Modules and connect them to this UI input module in child
        player.uiInputModule = GetComponentInChildren<InputSystemUIInputModule>();
  
    }

    public void ResumeGameButton()
    {
        //AudioManager.instance.Play("ClickButton");

        //Send event to the game manager to resume the game
        pauseMenuEvent.ResumeGameEventSend();

        //remove this pause menu
        Destroy(transform.root.gameObject);

    }

    public void PauseMenu_TitleScreen()
    {
        //AudioManager.instance.Play("ClickButton");

        //Send event to the game manager to resume the game
        pauseMenuEvent.ResumeGameEventSend();

        //Load Scene
        //SceneManager.LoadScene("");


    }

}




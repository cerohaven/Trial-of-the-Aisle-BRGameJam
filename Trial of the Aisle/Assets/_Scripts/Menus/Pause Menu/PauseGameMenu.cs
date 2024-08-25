
using UnityEngine;

using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

using UnityEngine.UI;

public class PauseGameMenu : MonoBehaviour
{
    /// <summary>
    /// The Pause Menu Prefab is instantiated into the scene from the 'PlayerInputHandler' when a player
    /// clicks the pause button.
    /// The 'GameManager' is in charge of checking to see if the game is paused or not.
    /// </summary>


    //components
    [SerializeField] private Button resumeButton;

    private void OnEnable()
    {
        if (GameManager.Instance.GamepadCursor == null) return;
        GameManager.Instance.GamepadCursor.EnableCursor(true);
    }
  

    public void ConnectControllersToPauseMenu(PlayerInput player)
    {
        //Get all the playuer's inputUI Modules and connect them to this UI input module in child
        player.uiInputModule = GetComponentInChildren<InputSystemUIInputModule>();
  
    }

    public void ResumeGameButton()
    {
        GameManager.Instance.EventSender.ResumeGameEventSend();

    }

    

    public void PauseMenu_TitleScreen()
    {
        GameManager.Instance.TransitionType = TransitionType.MainMenu;
        GameManager.Instance.LoadSpecificSceneStringPaused("MainMenu");
    }

    public void PauseMenu_QuitGame()
    {
        Application.Quit();
    }

}




using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGameMenu : MonoBehaviour
{
    /// <summary>
    /// The Pause Menu Prefab is instantiated into the scene from the 'PlayerInputHandler' when a player
    /// clicks the pause button.
    /// The 'GameManager' is in charge of checking to see if the game is paused or not.
    /// </summary>

    //components
    [SerializeField] private SO_PauseMenuEventSender pauseMenuEvent;

    [SerializeField] private Button resumeButton;

    //FOr the Settings menu
    //have access to the currently active menu so we can setActive False on button click
    private GameObject activeSettingsButton;
    [SerializeField] private Button videoButton; //for controls to set it on startup
    [SerializeField] private GameObject videoPanel, audioPanel, controlsPanel;


    [SerializeField] private RectTransform controlsPanelRect;
    private float minPosY;
    private float maxPosY;

    private void Awake()
    {
        //hover over the 'resume button' on startup
        if (PlayerController.Instance.GetCurrentControlScheme() == "Controller")
            resumeButton.Select();

        //Setting the button to the video from the beginning
        activeSettingsButton = videoPanel;

    }


    public void ConnectControllersToPauseMenu(PlayerInput player)
    {
        AudioManager.Instance.Play("Positive Interact");

        //Get all the playuer's inputUI Modules and connect them to this UI input module in child
        player.uiInputModule = GetComponentInChildren<InputSystemUIInputModule>();
  
    }

    public void PauseMenu_ResumeGameButton()
    {
       
        AudioManager.Instance.Play("Negative Interact");


        //Send event to the game manager to resume the game
        pauseMenuEvent.ResumeGameEventSend();


        //remove this pause menu
        Destroy(transform.root.gameObject);

    }



    public void PauseMenu_TitleScreen()
    {
       
        AudioManager.Instance.Play("Negative Interact");

        //Send event to the game manager to resume the game
        pauseMenuEvent.ResumeGameEventSend();

        //Load Scene
        SceneManager.LoadScene("MainMenu");


    }


    // CONTROLS SCREEN IN THE PAUSE MENU //

    //Called from the button in the inspector
    public void PauseMenu_SettingsButton()
    {
        //Leen Tween from top of frame to bottom
        LeanTween.value(1080f, 0, 0.5f).setOnUpdate(UpdatePanelMinY).setEaseOutQuad().setIgnoreTimeScale(true);
        LeanTween.value(1080f, 0, 0.5f).setOnUpdate(UpdatePanelMaxY).setEaseOutQuad().setIgnoreTimeScale(true);

        AudioManager.Instance.Play("Positive Interact");

        //Connect to the Video button on startup
        if (PlayerController.Instance.GetCurrentControlScheme() == "Controller")
            videoButton.Select();
    }



    public void PauseMenu_QuitGameButton()
    {
        SceneManager.LoadScene("MenuScreen");
    }




    // SETTINGS MENU BUTTONS //
    public void Settings_VideoButton()
    {
        //hide previously active button contents and reveal this one's
        if (activeSettingsButton != null) activeSettingsButton.SetActive(false);

        activeSettingsButton = videoPanel;
        activeSettingsButton.SetActive(true);
    }


    public void Settings_AudioButton()
    {
        //hide previously active button contents and reveal this one's
        
        if(activeSettingsButton != null) activeSettingsButton.SetActive(false);

        activeSettingsButton = audioPanel;
        activeSettingsButton.SetActive(true);
    }


    public void Settings_ControlsButton()
    {
        //hide previously active button contents and reveal this one's
        if (activeSettingsButton != null) activeSettingsButton.SetActive(false);
        activeSettingsButton = controlsPanel;
        activeSettingsButton.SetActive(true);
    }


    //Called from the button in the inspector
    public void PauseMenu_ControlsBackButton()
    {
        //Lean tween from the bottom to the top
        LeanTween.value(0, 1080, 0.5f).setOnUpdate(UpdatePanelMinY).setEaseInQuad().setIgnoreTimeScale(true);
        LeanTween.value(0, 1080, 0.5f).setOnUpdate(UpdatePanelMaxY).setEaseInQuad().setIgnoreTimeScale(true);

        AudioManager.Instance.Play("Negative Interact");

    }
    public void Settings_BackButton()
    {
        PauseMenu_ControlsBackButton();
    }

   
    private void UpdatePanelMinY(float _value)
    {
        if (controlsPanelRect == null) return;

        minPosY = _value;
        controlsPanelRect.offsetMin = new Vector2(controlsPanelRect.offsetMin.x, minPosY);
    }
    private void UpdatePanelMaxY(float _value)
    {
        if (controlsPanelRect == null) return;

        maxPosY = _value;
        controlsPanelRect.offsetMax = new Vector2(controlsPanelRect.offsetMax.x, maxPosY);
    }
}




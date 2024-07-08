
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.CanPause = false;
    }

    // Function to be called when the "Start" button is pressed
    public void StartGame()
    {
        // Load the next scene in the build index
        GameManager.Instance.TransitionType = TransitionType.BossBattle;
        GameManager.Instance.LoadSpecificSceneString("Boss_Painkiller_LD_Redesign");

    }

    // Function to be called when the "Tutorial" button is pressed

    public void GoToControls()

    {
        // Load the Tutorial Scene
        // Make sure the Tutorial scene is added in the build settings and its name is exactly "Tutorial"
        GameManager.Instance.TransitionType = TransitionType.MainMenu;
        GameManager.Instance.LoadSpecificSceneString("Tutorial");

    }

    public void GoToMenu()
    {
        // Load the Tutorial Scene
        // Make sure the Tutorial scene is added in the build settings and its name is exactly "Tutorial"
        GameManager.Instance.TransitionType = TransitionType.MainMenu;
        GameManager.Instance.LoadSpecificSceneString("MainMenu");

    }

    // Function to be called when the "About" button is pressed

    public void GoToCredits()

    {
        // Load the About Scene
        // Make sure the About scene is added in the build settings and its name is exactly "About"
        GameManager.Instance.TransitionType = TransitionType.MainMenu;
        GameManager.Instance.LoadSpecificSceneString("Credits");

    }


    public void GoToExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}


using UnityEngine;

public class MainMenuController : MonoBehaviour
{

   
    // Function to be called when the "Start" button is pressed
    public void StartGame()
    {
        // Load the next scene in the build index
        SceneTransitionController.Instance.TransitionType = TransitionType.BossBattle;
        SceneTransitionController.Instance.LoadSpecificSceneBuildIndex(3);

    }

    // Function to be called when the "Tutorial" button is pressed
    public void GoToTutorial()
    {
        // Load the Tutorial Scene
        // Make sure the Tutorial scene is added in the build settings and its name is exactly "Tutorial"
        SceneTransitionController.Instance.TransitionType = TransitionType.MainMenu;
        SceneTransitionController.Instance.LoadSpecificSceneString("Tutorial");

    }

    public void GoToMenu()
    {
        // Load the Tutorial Scene
        // Make sure the Tutorial scene is added in the build settings and its name is exactly "Tutorial"
        SceneTransitionController.Instance.TransitionType = TransitionType.MainMenu;
        SceneTransitionController.Instance.LoadSpecificSceneString("MainMenu");

    }

    // Function to be called when the "About" button is pressed
    public void GoToAbout()
    {
        // Load the About Scene
        // Make sure the About scene is added in the build settings and its name is exactly "About"
        SceneTransitionController.Instance.TransitionType = TransitionType.MainMenu;
        SceneTransitionController.Instance.LoadSpecificSceneString("About");

    }


    public void GoToExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}

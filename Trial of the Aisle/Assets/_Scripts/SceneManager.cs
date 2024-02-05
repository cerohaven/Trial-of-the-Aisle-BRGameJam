using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Function to be called when the "Start" button is pressed
    public void StartGame()
    {
        // Load the next scene in the build index
        SceneManager.LoadScene(3);
    }

    // Function to be called when the "Tutorial" button is pressed
    public void GoToTutorial()
    {
        // Load the Tutorial Scene
        // Make sure the Tutorial scene is added in the build settings and its name is exactly "Tutorial"
        SceneManager.LoadScene("Tutorial");
    }

    public void GoToMenu()
    {
        // Load the Tutorial Scene
        // Make sure the Tutorial scene is added in the build settings and its name is exactly "Tutorial"
        SceneManager.LoadScene("MainMenu");
    }

    // Function to be called when the "About" button is pressed
    public void GoToAbout()
    {
        // Load the About Scene
        // Make sure the About scene is added in the build settings and its name is exactly "About"
        SceneManager.LoadScene("About");
    }
}

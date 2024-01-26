using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private SO_PauseMenuEventSender pauseMenuEvent;

    public void QuitGame()
    {
        //AudioManager.instance.Play("ClickButton");
        Application.Quit();
    }


    public void Options()
    {
        //AudioManager.instance.Play("ClickButton");
    }

    public void Back()
    {
        SceneManager.LoadScene("MenuScreen");
        //AudioManager.instance.Play("ClickButton");
    }

    public void HowToPlay()
    {
        //AudioManager.instance.Play("ClickButton");
        SceneManager.LoadScene("HowToScreen");
    }


}

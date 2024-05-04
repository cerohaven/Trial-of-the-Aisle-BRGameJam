using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    private Animator animator;

    private int currentScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject.transform.parent);
        }

        animator = GetComponent<Animator>();
    }
    
    //Called from the Animation's Last Frame Event. Do not use this with other scripts. Use the LoadNextScene instead.
    public void SwitchScene()
    {

        Debug.Log("Test");
        currentScene = SceneManager.GetActiveScene().buildIndex;
    
        SceneManager.LoadScene(currentScene + 1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadNextScene()
    {
        animator.SetTrigger("Start");
    }
}

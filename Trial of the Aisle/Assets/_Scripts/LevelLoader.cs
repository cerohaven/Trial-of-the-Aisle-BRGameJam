using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public int currentScene;
    public void SwitchScene()
    {

        
        currentScene = SceneManager.GetActiveScene().buildIndex;
    
        SceneManager.LoadScene(currentScene + 1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(2);
    }

    public void SetTrigger()
    {
        animator.SetTrigger("Start");
    }
}

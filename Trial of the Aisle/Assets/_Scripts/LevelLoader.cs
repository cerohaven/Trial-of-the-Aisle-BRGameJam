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

    public void SetTrigger()
    {
        animator.SetTrigger("Start");
    }
}

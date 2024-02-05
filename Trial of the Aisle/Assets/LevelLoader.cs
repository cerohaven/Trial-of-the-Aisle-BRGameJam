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
    public static int previousScene;
    public void SwitchScene()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        int sceneToLoad = previousScene;
        while(sceneToLoad == previousScene)
        {
            sceneToLoad = Random.Range(0, 3);
        }
        
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetTrigger()
    {
        animator.SetTrigger("Start");
    }
}

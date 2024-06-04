using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class LevelLoader : MonoBehaviour
{
    private SceneTransitionController transitionController;


    private Animator animator;
    [SerializeField] private Image image;

    private int currentScene;


    //Animator components
    public float maskSize;
    public float maskRotationSpeed;
    public Color backgroundColour;
    public Color itemsColour;

    private readonly int _maskSizeID = Shader.PropertyToID("_circleSize");
    private readonly int _maskRotationSpeedID = Shader.PropertyToID("_rotationSpeed");
    private readonly int _backgroundColourID = Shader.PropertyToID("_backgroundColour");
    private readonly int _itemsColourID = Shader.PropertyToID("_circlesColour");
    private void Awake()
    {
      
        animator = GetComponent<Animator>();
        image = GetComponentInChildren<Image>();
        transitionController = GetComponentInParent<SceneTransitionController>();
    }



    private void Update()
    {
        image.materialForRendering.SetFloat(_maskSizeID, maskSize);
    }


    //Called from the Animation's Last Frame Event. Do not use this with other scripts. Use the LoadNextScene instead.
    public void SwitchScene()
    {
        Debug.Log("Test");

        currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScene + 1);

    }

    //Called from the SceneTransitionController
    public IEnumerator LerpColour()
    {
        Color startColour = image.materialForRendering.GetColor(_backgroundColourID);
        Color endColour = transitionController.GetColourToUse()._backgroundColour;
        Color currentColor;

        Color startColourItems = image.materialForRendering.GetColor(_itemsColourID);
        Color endColourItems = transitionController.GetColourToUse()._itemsColour;
        Color currentColourItems;

        float currentTime = 0;

        while(currentTime < 1)
        {
            currentTime += Time.deltaTime;
            currentColor = Color.Lerp(startColour, endColour, currentTime);
            currentColourItems = Color.Lerp(startColourItems, endColourItems, currentTime);
            image.materialForRendering.SetColor(_backgroundColourID, currentColor);
            image.materialForRendering.SetColor(_itemsColourID, currentColourItems);
       
            yield return null;
        }
        
        yield return null;


        PlayEnterSceneAnimation();
    }


    public void MainMenu()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadNextScene()
    {
        animator.SetBool("exitScene", true);
    }
    public void PlayEnterSceneAnimation()
    {
        animator.SetBool("exitScene", false);
    }
}

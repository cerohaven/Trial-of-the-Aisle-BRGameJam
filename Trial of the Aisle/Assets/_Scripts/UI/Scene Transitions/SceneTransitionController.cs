using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    Default,
    MainMenu,
    BossBattle,
    WinGame,
    LoseGame
}


[System.Serializable]
public class TransitionTypesData
{
    [HideInInspector] public string _transitionTypeName;
    public GameObject _transitionGameObject;
}


public class SceneTransitionController : MonoBehaviour
{

    //Components
    private Animation animationComp;

    //Variables

    [SerializeField] private List<TransitionTypesData> arrTransitionTypes; // Array to store Animation Game Objects
    [SerializeField] private AnimationClip animationClipToPlay;

    private GameObject prevGO; // the previous game Object animation
    private int transitionTypeEnumLength = 0;


    [SerializeField] private TransitionType transitionType;

    //Properties

    public AnimationClip AnimationClipToPlay { get => animationClipToPlay; set => animationClipToPlay = value; }
    public Animation AnimationComp { get => animationComp; set => animationComp = value; }

    private void Awake()
    {
        animationComp = GetComponent<Animation>();
    }


    #region Scene Loaded Code
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name.Equals("MainMenu"))
        {
            GameManager.Instance.TransitionType = TransitionType.MainMenu;
            GameManager.Instance.GeneralResumeLogic();

        }
        //Lerp Colours
        PlayEnterSceneAnimation();

    }
    #endregion


    #region OnEnter and OnExit Functions to Play Animations
    //Function to play the scene loaded animation
    public void PlayEnterSceneAnimation()
    {
        transitionType = GameManager.Instance.TransitionType;
        animationComp.Stop();

        GameObject go = GetSceneTransitionGameObject();

        if (go == null) return;

        if (prevGO == null)
        {
            go.SetActive(true);
        }

        ISceneTransition ist = go.GetComponent<ISceneTransition>();

        if (!HasInterfaceComponent(go, ist)) return;

        ist.Initialize();

        PlayClip(ist.EnterSceneAnimationClip);
    }

    //Function to play the scene loading animation
    public void PlayExitSceneAnimation()
    {
        transitionType = GameManager.Instance.TransitionType;
        GameObject go = GetSceneTransitionGameObject();

        if (go == null) return;

        go.SetActive(true);

        prevGO = go;

        ISceneTransition ist = go.GetComponent<ISceneTransition>();

        if (!HasInterfaceComponent(go, ist)) return;

        PlayClip(ist.ExitSceneAnimationClip);
    }
    #endregion

    #region Transition Animation
    public IEnumerator WaitForAnimationAndLoadNextScene()
    {

        PlayExitSceneAnimation();

        yield return new WaitForSeconds(animationClipToPlay.averageDuration);

        Scene currentScene = SceneManager.GetActiveScene();
        int nextScene = currentScene.buildIndex + 1;
        SceneManager.LoadScene(nextScene);

    }

    public IEnumerator WaitForAnimationAndLoadSpecificScene(string sceneName)
    {

        PlayExitSceneAnimation();


        yield return new WaitForSeconds(animationClipToPlay.averageDuration);

        SceneManager.LoadScene(sceneName);

    }

    public IEnumerator WaitForAnimationAndLoadSpecificScenePaused(string sceneName)
    {

        PlayExitSceneAnimation();

        AnimationState currentState = animationComp[animationComp.clip.name];

        while (currentState.time < animationClipToPlay.averageDuration)
        {
            currentState.time += Time.unscaledDeltaTime;
            animationComp.Sample();
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
        yield return null;

    }

    public IEnumerator WaitForAnimationAndLoadSpecificSceneBuildIndex(int buildIndex)
    {

        PlayExitSceneAnimation();

        yield return new WaitForSeconds(animationClipToPlay.averageDuration);

        SceneManager.LoadScene(buildIndex);

    }
    #endregion

    #region Animation Events and Clip Functions

    //The order of Operations:
    // 1. The OnSceneLoaded Event activates calling the PlayEnterSceneAnimation() Method
    // 2. The PlayEnterSceneAnimation() hides the previous transition object
    // 3. The Animation Event calls on the FinishedSceneAnimation() method which sets the current transition object to visible
    public void FinishedSceneAnimation()
    {
        GetSceneTransitionGameObject().SetActive(true);

        if (prevGO != null && prevGO != GetSceneTransitionGameObject())
        {
            prevGO.SetActive(false);
        }
    }

    public void HideTransitionGO()
    {
        GetSceneTransitionGameObject().SetActive(false);
    }

    private void PlayClip(AnimationClip transitionAnimation)
    {
        animationClipToPlay = transitionAnimation;
        animationComp.clip = animationClipToPlay;

        animationComp.Play();
    }


    #endregion

    #region Helper Functions
    public GameObject GetSceneTransitionGameObject()
    {
        TransitionType tt = GetTransition();

        int index = 0;

        //Find the GameObject associated with the TransitionType
        foreach (string name in Enum.GetNames(typeof(TransitionType)))
        {

            if (tt.ToString().Equals(name))
            {
                return arrTransitionTypes[index]._transitionGameObject;
            }

            index++;
        }

        return arrTransitionTypes[0]._transitionGameObject;

    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // Method to get the transition we're going to play
    public TransitionType GetTransition()
    {
        return GameManager.Instance.TransitionType;
    }

  

    //Function to Check if the object put into the arrTransitionTypes list has the ISceneTransition script.
    //This script is needed to store Animation Clips and perform logic for they scene transition's animation.
    private bool HasInterfaceComponent(GameObject go, ISceneTransition ist)
    {
        if (ist == null)
        {
            Debug.LogWarning($"{go} does not have a script that implements ISceneTransition attached to it. \n" +
                             $"Please add a script that implements the ISceneTransition interface");
            return false;
        }

        return true;
    }

    #endregion

    #region Editor Script Functionality

    //Called from the SceneTransitionControllerEditor.cs class
    //Updates the arrTransitionTypes List to the amount of enum values in the TransitionType enum in the MsgOperator.cs class
    //sets the name of each element in the list to the name of the enum value at the index
    public void UpdateTransitionTypeArray()
    {

        transitionTypeEnumLength = Enum.GetValues(typeof(TransitionType)).Length;

        //Create a List of the transition types
        List<TransitionTypesData> temp = new List<TransitionTypesData>();
        for (int j = 0; j < transitionTypeEnumLength; j++)
        {
            temp.Add(new TransitionTypesData());
        }

        int index = 0;

        foreach (string name in Enum.GetNames(typeof(TransitionType)))
        {

            temp[index]._transitionTypeName = name;

            for (int j = 0; j < arrTransitionTypes.Count; j++)
            {
                if (temp[index]._transitionTypeName.Equals(arrTransitionTypes[j]._transitionTypeName))
                {
                    temp[index]._transitionGameObject = arrTransitionTypes[j]._transitionGameObject;
                }
            }
            index++;
        }


        arrTransitionTypes = temp;
    }

    #endregion
}


using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FMODMusicManager : MonoBehaviour
{
    public GameObject MusicManager;

   /*
    private FMOD.Studio.EventInstance ATGMusicInstance;
    private FMOD.Studio.EventInstance PKMusicInstance;
    private FMOD.Studio.EventInstance DDMusicInstance;
   */
    private FMOD.Studio.EventInstance ATGIntroInstance;
    private FMOD.Studio.EventInstance PKIntroInstance;
    private FMOD.Studio.EventInstance DDIntroInstance;

    private FMOD.Studio.EventInstance AdaptiveMusicInstance;


    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(MusicManager);
        Scene thisScene = SceneManager.GetActiveScene();
        string sceneName = thisScene.name;


        AdaptiveMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/Adaptive_Music");
        AdaptiveMusicInstance.start();

        if (sceneName == "Boss_AlexanderTheGrape 1")
        {
            // ATGMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/AtG_Arena");
            ATGIntroInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Dialogue/Introductions/AtG_Intro");

            // ATGMusicInstance.start();
            ATGIntroInstance.start();

            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("SceneTransition", "ATGArenaEntered");

        }
        else if (sceneName == "Boss_Painkiller_LD_Redesign")
        {
            //  PKMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/PK_Arena");
            PKIntroInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Dialogue/Introductions/PK_Intro");

            // PKMusicInstance.start();
            PKIntroInstance.start();


            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("SceneTransition", "PKArenaEntered");

        }
        else if(sceneName == "Boss_DairyDominator 1")
        {
            //  DDMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/DD_Arena");
            DDIntroInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Dialogue/Introductions/DD_Intro");

            //  DDMusicInstance.start();
            DDIntroInstance.start();

            FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel("SceneTransition", "DDArenaEntered");

        }

        if (sceneName == "MainMenu")
        {
            AdaptiveMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FMODMusicManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance ATGMusicInstance;
    private FMOD.Studio.EventInstance PKMusicInstance;
    private FMOD.Studio.EventInstance DDMusicInstance;


    // Start is called before the first frame update
    void Start()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        string sceneName = thisScene.name;

        if (sceneName == "Boss_AlexanderTheGrape")
        {
            ATGMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/AtG_Arena");
            ATGMusicInstance.start();
        }

        else if (sceneName == "Boss_Painkiller")
        {
            PKMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/PK_Arena");
            PKMusicInstance.start();
        }

        else if (sceneName == "Boss_DairyDominator")
        {
            DDMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/BGM/DD_Arena");
            DDMusicInstance.start();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

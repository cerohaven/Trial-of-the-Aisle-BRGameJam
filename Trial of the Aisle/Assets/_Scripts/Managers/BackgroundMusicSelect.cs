using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BackgroundMusicSelector
{
    None, 
    MenuTheme,
    BossTheme1, 
    BossTheme2, 
    BossTheme3
}
public class BackgroundMusicSelect : MonoBehaviour
{
    public static BackgroundMusicSelect Instance;

    [SerializeField] private BackgroundMusicSelector backgroundMusicSelector;

    private AudioManager am;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

        

        
    }

    public void PlayBGMusic()
    {
        AudioManager[] audioManager = FindObjectsOfType<AudioManager>();

        if (audioManager.Length == 0)
            return;

        if (audioManager.Length > 1)
        {
            //play the audio delayed. Work around because right now it is trying to find the audio manager in the scene
            //instead of the DontDestroyOnLoad One
            Invoke("TryFindingAgain", 0.1f);
        }
        else
        {
            //Play Background Music
            am = audioManager[0];
            audioManager[0].BgPlay(backgroundMusicSelector);
        }
    }

    private void OnValidate()
    {
        //Play Audio when we switch it during game so we can test easier!
        if(am != null)
        {
            am.BgPlay(backgroundMusicSelector);
        }
    }


    private void TryFindingAgain()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.BgPlay(backgroundMusicSelector);
    }
}

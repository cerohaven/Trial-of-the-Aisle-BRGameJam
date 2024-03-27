using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BackgroundMusicSelector
{
    None, TitleTheme, GameTheme, GameTheme2
}
public class BackgroundMusicSelect : MonoBehaviour
{
    
    [SerializeField] private BackgroundMusicSelector backgroundMusicSelector;

    private void Start()
    {

        AudioManager[] audioManager = FindObjectsOfType<AudioManager>();

        if (audioManager.Length == 0)
            return;

        if(audioManager.Length>1)
        {
            //play the audio delayed. Work around because right now it is trying to find the audio manager in the scene
            //instead of the DontDestroyOnLoad One
            Invoke("TryFindingAgain", 0.1f);
        }
        else
        {
            //Play Background Music
            audioManager[0].BgPlayOnAwake(backgroundMusicSelector);
        }

        
    }

    private void TryFindingAgain()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.BgPlayOnAwake(backgroundMusicSelector);
    }
}

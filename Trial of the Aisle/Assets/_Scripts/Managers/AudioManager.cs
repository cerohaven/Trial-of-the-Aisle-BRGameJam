using UnityEngine.Audio;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;
    public BgSounds[] bgSounds;

    //a reference to the current instance of the audio manager we have in our scene
    public static AudioManager instance;
    private IEnumerator coroutine;

    private void Awake()
    {
        //we want to check if there's only 1 in our scene. Singleton Pattern
        if(instance == null)
            instance = this;

        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        //set up audio managers so we can play them in the start
        //loop through each class array
        foreach (Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.clip = s.clip[Random.Range(0, s.clip.Length)];
        }

        //set up audio managers so we can play them in the start
        //loop through each class array
        foreach (BgSounds s in bgSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.clip = s.clip;
        }

    }

    public void Play(string name)
    {
        //randomize all the sounds that have multiple clips
        foreach (Sounds a in sounds)
        {
            a.source.clip = a.clip[Random.Range(0, a.clip.Length)];
        }

        //find a sound in the sounds array where sound.name == name the name inputted
        Sounds s = Array.Find(sounds, sound => sound.name == name);
      
        //if they didn't find a sound with that name in the array, throw an error
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " Was not Found!");
            return;
        }

        s.source.Play();
    }
    public void BgPlay(BackgroundMusicSelector backgroundMusic)
    {

        //find a sound in the sounds array where sound.name == name the name inputted
        BgSounds s = Array.Find(bgSounds, sound => sound.backgroundMusic == backgroundMusic);

        //if they didn't find a sound with that name in the array, throw an error
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Was not Found!");
            return;
        }

        //if the name is the same, then don't play a new background track
        if(s.source.isPlaying)
        {
            return;
        }
        //if it's not the same than the current one then stop all other bg musics and play this one
        else
        {
            //stop all the other background tracks and then play the new one
            foreach(BgSounds bg in bgSounds)
            {
                bg.source.Stop();

            }

            s.source.Play();
        }
        
    }

}



[System.Serializable]
public class Sounds
{
    public string name;
    public AudioClip[] clip;

    [Range(0f,1f)]
    public float volume = 1;
    [Range(0f, 3f)]
    public float pitch = 1;

    public bool loop;
    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public class BgSounds
{
    public BackgroundMusicSelector backgroundMusic;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1;
    [Range(0f, 3f)]
    public float pitch = 1;

    public bool loop;
    [HideInInspector]
    public AudioSource source;
}

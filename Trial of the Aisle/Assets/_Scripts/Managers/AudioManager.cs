using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private readonly float startingVolume = 0.5f;
    private float dafaultMusicVolume;

    [Space()]
    public Sounds[] sounds;
    public BgSounds[] bgSounds;

    //a reference to the current instance of the audio manager we have in our scene
    public static AudioManager Instance;
    private IEnumerator musicCoroutine;
    private IEnumerator musicCoroutine2;
    private IEnumerator fadeAudioCoroutine;
    private IEnumerator fadeBgAudioCoroutine;

    //Properties
    public AudioMixer m_AudioMixer { get => audioMixer; }

    public float m_StartingVolume => startingVolume;

    private void Awake()
    {
        //we want to check if there's only 1 in our scene. Singleton Pattern
        if (Instance == null)
            Instance = this;

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
            s.source.outputAudioMixerGroup = sfxMixerGroup;
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
            s.source.outputAudioMixerGroup = musicMixerGroup;
        }

    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Master")) return;

        SetAudioMixer();
    }

    private void SetAudioMixer()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(startingVolume) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(startingVolume) * 20);
        audioMixer.SetFloat("Music", Mathf.Log10(startingVolume) * 20);

        //Saving the value
        PlayerPrefs.SetFloat("MasterVolume", startingVolume);
        PlayerPrefs.SetFloat("SFXVolume", startingVolume);
        PlayerPrefs.SetFloat("MusicVolume", startingVolume);

        dafaultMusicVolume = GetMusicVolume();
    }


    #region SFX Play, Pause, and Stop Methods
    public void Play(string name, float fadeTime = 0, float waitTilStart = 0)
    {
        //randomize all the sounds that have multiple clips
        foreach (Sounds a in sounds)
        {
            a.source.clip = a.clip[Random.Range(0, a.clip.Length)];
        }

        //find a sound in the sounds array where sound.name == name the name inputted
        Sounds s = Array.Find(sounds, sound => sound.name == name);

        //if they didn't find a sound with that name in the array, throw an error
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Was not Found!");
            return;
        }

        fadeAudioCoroutine = IEPlay(s.source, fadeTime, waitTilStart);
        StartCoroutine(fadeAudioCoroutine);
    }

    public void Pause(string name, float fadeTime = 0)
    {

        //find a sound in the sounds array where sound.name == name the name inputted
        Sounds s = Array.Find(sounds, sound => sound.name == name);

        //if they didn't find a sound with that name in the array, throw an error
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Was not Found!");
            return;
        }

        fadeAudioCoroutine = IEStop(s.source, fadeTime, true);
        StartCoroutine(fadeAudioCoroutine);
    }

    public void Stop(string name, float fadeTime = 0)
    {
        //find a sound in the sounds array where sound.name == name the name inputted
        Sounds s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Was not Found!");
            return;
        }

        fadeAudioCoroutine = IEStop(s.source, fadeTime, false);
        StartCoroutine(fadeAudioCoroutine);
    }



    #endregion

    #region Background Music Play, Pause, Stop Methods

    /// <summary>
    /// You should use this method only in the Background Music Selector classor when you want to stop all other
    /// background audios from playing
    /// </summary>
    /// <param name="backgroundMusic" >The music track you'd like to select</param> 
    /// <param name="fadeInTime">How long (in seconds) that this audio track will fade in for</param> 
    /// <param name="waitTilStart">The time (in seconds) that this audio will start playing after</param> 
    /// /// <param name="fadeOutTime">How long (in seconds) that all the other background sounds should stop playing</param> 
    public void BgPlayOnAwake(BackgroundMusicSelector backgroundMusic, float fadeInTime = 0, float waitTilStart = 0, float fadeOutTime = 0)
    {

        //find a sound in the sounds array where sound.name == name the name inputted
        BgSounds s = Array.Find(bgSounds, sound => sound.backgroundMusic == backgroundMusic);

        //if the name is the same, then don't play a new background track
        if (s.source.isPlaying) return;

        //if it's not the same than the current one then stop all other bg musics and play this one

        //stop all the other background tracks and then play the new one
        foreach (BgSounds bg in bgSounds)
        {

            if (bg.source.isPlaying)
            {
                fadeBgAudioCoroutine = IEStop(bg.source, fadeOutTime, false);
                StartCoroutine(fadeBgAudioCoroutine);

            }


        }

        fadeAudioCoroutine = IEPlay(s.source, fadeInTime, waitTilStart);
        StartCoroutine(fadeAudioCoroutine);


    }

    public void BgPlay(BackgroundMusicSelector backgroundMusic, float fadeInTime = 0, float waitTilStart = 0)
    {

        //find a sound in the sounds array where sound.name == name the name inputted
        BgSounds s = Array.Find(bgSounds, sound => sound.backgroundMusic == backgroundMusic);

        //if the name is the same, then don't play a new background track
        if (s.source.isPlaying) return;

        //if it's not the same than the current one then stop all other bg musics and play this one
        fadeAudioCoroutine = IEPlay(s.source, fadeInTime, waitTilStart);
        StartCoroutine(fadeAudioCoroutine);


    }
    public void BgPause(BackgroundMusicSelector backgroundMusic, float fadeTime = 0)
    {

        //find a sound in the sounds array where sound.name == name the name inputted
        BgSounds s = Array.Find(bgSounds, sound => sound.backgroundMusic == backgroundMusic);

        fadeAudioCoroutine = IEStop(s.source, fadeTime, true);
        StartCoroutine(fadeAudioCoroutine);
    }

    public void BgStop(BackgroundMusicSelector backgroundMusic, float fadeTime = 0)
    {

        //find a sound in the sounds array where sound.name == name the name inputted
        BgSounds s = Array.Find(bgSounds, sound => sound.backgroundMusic == backgroundMusic);

        fadeAudioCoroutine = IEStop(s.source, fadeTime, false);
        StartCoroutine(fadeAudioCoroutine);
    }



    #endregion

    #region IEnumerators for Playing and Stopping Audio
    private IEnumerator IEPlay(AudioSource a, float fadeTime, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Store the current audio
        float initialVolume = a.volume;

        //Now store the currentAudio volume in a float we can work with
        float currentLevel = fadeTime > 0 ? 0 : initialVolume;
        a.volume = currentLevel;

        //Lerp time variables
        float currentTime = 0;

        a.Play();

        while (a.volume < initialVolume)
        {

            float newVol = Mathf.Lerp(currentLevel, initialVolume, currentTime / fadeTime);

            a.volume = newVol;

            currentTime += Time.deltaTime;

            yield return null;
        }


        a.volume = initialVolume;

        yield break;
    }

    private IEnumerator IEStop(AudioSource a, float fadeTime, bool pauseAudio)
    {
        //Store the current volume of the audioSource
        float initialAudio = a.volume;
        float currentLevel = initialAudio;

        //Lerp time variables
        float currentTime = 0;

        while (a.volume > 0.001f)
        {

            float newVol = Mathf.Lerp(currentLevel, 0, currentTime / fadeTime);

            a.volume = newVol;

            currentTime += Time.deltaTime;

            yield return null;
        }

        //now pause or stop the audio based on the method we used
        if (pauseAudio)
            a.Pause();
        else
            a.Stop();

        //now reset the volume back to what it was originally before the fade
        a.volume = initialAudio;

        yield break;
    }
    #endregion

    #region Audio Helper Functions
    public bool IsAudioPlaying(string name)
    {
        //find a sound in the sounds array where sound.name == name the name inputted
        Sounds s = Array.Find(sounds, sound => sound.name == name);

        //if they didn't find a sound with that name in the array, throw an error
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " Was not Found!");
            return false;
        }

        bool isAudioPlaying = s.source.isPlaying ? true : false;
        return isAudioPlaying;

    }

    #endregion

    #region Audio Settings Methods
    public void LerpAudioToLevel(float _targetLevel)
    {
        musicCoroutine = IEChangeAudio(_targetLevel);
        StartCoroutine(musicCoroutine);
    }


    private IEnumerator IEChangeAudio(float _targetLevel)
    {
        //It can't be set to 0 for this math equation or else we'll get infinity
        _targetLevel = Mathf.Clamp(_targetLevel, 0.0001f, 1f);
        float waitTime = 0.5f;
        float currentTime = 0;

        float currentLevel = GetMusicVolume();

        while (currentTime < waitTime)
        {
            if (!GameManager.isGamePaused) yield break;

            float newVol = Mathf.Lerp(currentLevel, _targetLevel, currentTime / waitTime);

            audioMixer.SetFloat("MusicLowered", Mathf.Log10(newVol) * 20);

            currentTime += Time.unscaledDeltaTime;

            yield return null;
        }

        yield break;

    }


    public void LerpAudioToPrevLevel()
    {
        musicCoroutine2 = IERevertAudio();
        StartCoroutine(musicCoroutine2);
    }

    private IEnumerator IERevertAudio()
    {
        yield return new WaitForSeconds(0.1f);
        //It can't be set to 0 for this math equation or else we'll get infinity

        dafaultMusicVolume = Mathf.Clamp(dafaultMusicVolume, 0.0001f, 1f);

        float waitTime = 0.5f;
        float currentTime = 0;

        float currentLevel = GetMusicVolume();

        while (currentTime < waitTime)
        {
            if (GameManager.isGamePaused) yield break;
            float newVol = Mathf.Lerp(currentLevel, dafaultMusicVolume, currentTime / waitTime);

            audioMixer.SetFloat("MusicLowered", Mathf.Log10(newVol) * 20);

            currentTime += Time.deltaTime;

            yield return null;
        }

        yield break;

    }

    private float GetMusicVolume()
    {
        float currentLevel;
        bool result = m_AudioMixer.GetFloat("MusicLowered", out currentLevel);

        if (result)
        {
            return Mathf.Pow(10, currentLevel / 20);
        }
        else
        {
            return 0;
        }
    }

    #endregion
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

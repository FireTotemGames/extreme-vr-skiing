using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static MusicController Instance;


    private BusStruct busList;
    private MusicStruct musicList;
    private SnapshotStruct snapshotList;

    private float musicVolume;
    private float soundVolume;

    public float MusicVolume
    {
        get => Instance.musicVolume;
        set
        {
            Instance.musicVolume = value;
            PlayerPrefs.SetFloat(nameof(musicVolume), value);
            UpdateBusList();
        }
    }

    public float SoundVolume
    {
        get => Instance.soundVolume;
        set
        {
            Instance.soundVolume = value;
            PlayerPrefs.SetFloat(nameof(soundVolume), value);
            UpdateBusList();
        }
    }

    public BusStruct BusList => busList;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartMusic();
    }

    private void Update()
    {
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        busList.music.setPaused(!hasFocus);
    }

    // private void OnApplicationPause(bool pauseStatus)
    // {
    //     busList.music.setPaused(pauseStatus);
    // }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public EventInstance PlaySound(EventReference soundReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(soundReference);
        eventInstance.start();
        return eventInstance;
    }
    
    public void PlaySound(string soundName)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(soundName);
        eventInstance.start();
    }
    
    public void PlaySound(string soundName, float delay)
    {
        StartCoroutine(PlaySoundDelayed(soundName, delay));
    }

    public void PlaySoundNoReturn(string soundName)
    {
        PlaySound(soundName);
    }

    private IEnumerator PlaySoundDelayed(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        EventInstance eventInstance = RuntimeManager.CreateInstance(soundName);
        eventInstance.start();
    }
    
    /* ======================================================================================================================== */
    /* GENERAL FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void Initialize()
    {
        InitializeBusList();
        InitializeMusicList();
        InitializeSnapShotList();
    }

    public void Pause()
    {
        snapshotList.pause.start();
    }

    public void Continue()
    {
        Instance.snapshotList.pause.stop(STOP_MODE.ALLOWFADEOUT);
    }

    /* ======================================================================================================================== */
    /* VOLUME FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    private void InitializeBusList()
    {
        Instance.busList.music = RuntimeManager.GetBus("bus:/music");
        Instance.busList.sound = RuntimeManager.GetBus("bus:/sounds");

        musicVolume = PlayerPrefs.GetFloat(nameof(musicVolume), 0.5f);
        soundVolume = PlayerPrefs.GetFloat(nameof(soundVolume), 0.5f);

        UpdateBusList();
    }

    private void UpdateBusList()
    {
        Instance.busList.music.setVolume(Instance.musicVolume);
        Instance.busList.sound.setVolume(Instance.soundVolume);
    }

    /* ======================================================================================================================== */
    /* MUSIC FUNCTIONS                                                                                                          */
    /* ======================================================================================================================== */

    private void InitializeMusicList()
    {
        Instance.musicList.music = RuntimeManager.CreateInstance("event:/music/main");
    }

    public void StartMusic()
    {
        Continue();

        PLAYBACK_STATE state;
        Instance.musicList.music.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.PLAYING)
        {
            Instance.musicList.music.start();
        }
        
        Instance.musicList.music.setParameterByName("progress", 0f);
    }

    public void StopMusic()
    {
        Instance.musicList.music.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void IncrementProgressParameter()
    {
        Instance.musicList.music.getParameterByName("progress", out float value);
        value++;
        Instance.musicList.music.setParameterByName("progress", value);
        Debug.Log(value);
    }

    /* ======================================================================================================================== */
    /* SNAPSHOT FUNCTIONS                                                                                                       */
    /* ======================================================================================================================== */

    private void InitializeSnapShotList()
    {
        Instance.snapshotList.pause = RuntimeManager.CreateInstance("snapshot:/pause");
    }

    /* ======================================================================================================================== */
    /* SOUND FUNCTIONS                                                                                                          */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
}

public struct BusStruct
{
    public Bus music;
    public Bus sound;
}

public struct MusicStruct
{
    public EventInstance music;
}

public struct SnapshotStruct
{
    public EventInstance pause;
}

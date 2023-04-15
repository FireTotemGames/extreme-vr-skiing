using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static GameController Instance;

    [SerializeField] private InputActionReference restartButton;
    [SerializeField] private ParticleSystem snowStorm;
    

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MusicController.Instance.StartMusic();
    }

    private void Update()
    {
        if (restartButton.action.WasPerformedThisFrame() == true)
        {
            Debug.Log("Restart");
            Restart();
        }
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void Restart()
    {
        MusicController.Instance.BusList.sound.stopAllEvents(STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void ActivateSnowStorm()
    {
        snowStorm.Play();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
}
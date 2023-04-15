using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class GameController : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    public static GameController Instance;

    [SerializeField] private InputActionReference restartButton;
    [SerializeField] private ParticleSystem snowStorm;
    [SerializeField] private XRRayInteractor leftRay;
    [SerializeField] private XRRayInteractor rightRay;
    [SerializeField] private InputActionReference pauseInputAction;
    [SerializeField] private Transform head;

    [Header("UI")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TextMeshProUGUI scoreText;

    private GameState state;

    public enum GameState
    {
        MainMenu,
        Running,
        Paused,
        GameOver
    }

    public GameState State => state;

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

        state = GameState.MainMenu;
        leftRay.enabled = true;
        rightRay.enabled = true;

        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    private void Update()
    {
        // if (restartButton.action.WasPerformedThisFrame() == true)
        // {
        //     Debug.Log("Restart");
        //     Restart();
        // }

        if (pauseInputAction.action.WasPerformedThisFrame() == true)
        {
            Pause();
        }

        pauseMenu.transform.LookAt(new Vector3(head.position.x, pauseMenu.transform.position.y, head.position.z));
        pauseMenu.transform.forward *= -1f;
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    public void Restart()
    {
        Time.timeScale = 1f;
        MusicController.Instance.BusList.sound.stopAllEvents(STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ActivateSnowStorm()
    {
        snowStorm.Play();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverMenu.SetActive(true);
        
        leftRay.enabled = true;
        rightRay.enabled = true;
        
        gameOverMenu.transform.position =
            head.position + new Vector3(head.forward.x, 0f, head.forward.z).normalized * 5f;
        
        gameOverMenu.transform.LookAt(new Vector3(head.position.x, gameOverMenu.transform.position.y, head.position.z));
        gameOverMenu.transform.forward *= -1f;

        scoreText.text = $"{Mathf.Floor(head.position.z).ToString()} m";

        MusicController.Instance.BusList.sound.stopAllEvents(STOP_MODE.ALLOWFADEOUT);
        MusicController.Instance.GameOver();
    }

    public void Pause()
    {
        if (state != GameState.Running)
        {
            return;
        }

        leftRay.enabled = true;
        rightRay.enabled = true;
        
        Time.timeScale = 0f;
        state = GameState.Paused;
        pauseMenu.SetActive(true);

        pauseMenu.transform.position = head.position + new Vector3(head.forward.x, 0f, head.forward.z).normalized * 5f;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        state = GameState.Running;
        pauseMenu.SetActive(false);
        
        leftRay.enabled = false;
        rightRay.enabled = false;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        mainMenu.SetActive(false);
        state = GameState.Running;
        
        leftRay.enabled = false;
        rightRay.enabled = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
}
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class Avalanche : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private GameObject deathTrigger;
    [SerializeField] private StudioEventEmitter avalancheSound;
    [SerializeField] private Transform player;
    [SerializeField] private float dangerSoundStartDistance;
    [SerializeField] private float dangerSoundStopDistance;
    
    private EventInstance dangerSound;
    private bool avalancheActive;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        deathTrigger.SetActive(false);
        dangerSound = RuntimeManager.CreateInstance("event:/sounds/danger");
    }

    private void Update()
    {
        if (avalancheActive == false)
        {
            return;
        }

        float distance = Mathf.Abs(transform.position.z - player.position.z);
        if (distance < dangerSoundStartDistance)
        {
            dangerSound.getPlaybackState(out PLAYBACK_STATE state);
            if (state != PLAYBACK_STATE.PLAYING)
            {
                Debug.Log("start danger sound");
                dangerSound.start();
            }

            float speed = 1f - Mathf.Clamp01(distance / dangerSoundStartDistance);
            dangerSound.setParameterByName("speed", speed);
        }
        else if (distance > dangerSoundStopDistance)
        {
            dangerSound.getPlaybackState(out PLAYBACK_STATE state);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                Debug.Log("stop danger sound");
                dangerSound.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
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

    public void ActivateDeathTrigger()
    {
        avalancheActive = true;
        deathTrigger.SetActive(true);
        avalancheSound.Play();
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}
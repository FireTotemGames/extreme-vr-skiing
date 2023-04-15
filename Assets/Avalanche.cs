using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private ParticleSystem fogParticles;
    [FormerlySerializedAs("rockContainer")] [SerializeField] private Transform stoneContainer;

    private EventInstance dangerSound;
    private bool avalancheActive;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        deathTrigger.SetActive(false);
        dangerSound = RuntimeManager.CreateInstance("event:/sounds/danger");
        foreach (Stonemover stone in stoneContainer.GetComponentsInChildren<Stonemover>())
        {
            stone.isRotating = false;
        }
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
        foreach (Stonemover stone in stoneContainer.GetComponentsInChildren<Stonemover>())
        {
            stone.isRotating = true;
        }

        fogParticles.Play();
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}
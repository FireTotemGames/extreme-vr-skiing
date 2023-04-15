using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stonemover : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */
    public float minAmplitude = 1f;
    public float maxAmplitude = 5f;
    public float minSpeed = 1f;
    public float maxSpeed = 5f;

    public float minRotationSpeed = 30f;
    public float maxRotationSpeed = 90f;

    private float targetAmplitude;
    private float speed;
    private float timeOffset;

    private float rotationSpeed;

    public bool isRotating;
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        // Set the initial target height, speed, and time offset
        targetAmplitude = Random.Range(minAmplitude, maxAmplitude);
        speed = Random.Range(minSpeed, maxSpeed);
        timeOffset = Random.Range(0f, 2f * Mathf.PI); // Randomize the starting point of the sine wave
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    private void Update()
    {
        if (isRotating == false)
        {
            return;
        }
        
        // Calculate the y position based on the current time and the sine wave
        float y = Mathf.Sin((Time.time + timeOffset) * speed) * targetAmplitude;

        // Move the object to the new position
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);

        transform.RotateAround(transform.position, Vector3.right, rotationSpeed * Time.deltaTime);
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

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */

}
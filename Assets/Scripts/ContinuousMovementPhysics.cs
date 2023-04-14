using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementPhysics : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private float speed = 1f;
    [SerializeField] private InputActionReference moveInputSource;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform directionSource;
    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private Transform skiLeft;
    [SerializeField] private Transform skiRight;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float steeringFactor;
    [SerializeField] private Transform playerHead;

    private Vector2 inputMoveAxis;
    private float skiAngleY;
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        
    }

    private void Update()
    {
        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Quaternion yaw = Quaternion.Euler(0f, directionSource.eulerAngles.y, 0f);
        Vector3 direction = yaw * new Vector3(inputMoveAxis.x, 0f, inputMoveAxis.y);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        // Vector3 velocity = rb.velocity;
        // velocity.x = direction.x * speed;
        // velocity.z = direction.z * speed;
        // rb.velocity = velocity;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

        RotateSkies();

        float roll = playerHead.rotation.z * Vector3.Dot(playerHead.forward, Vector3.forward);
        Vector3 steeringForce = Vector3.right * -roll * steeringFactor;
        Debug.Log(steeringForce);
        // rb.AddTorque(steeringForce, ForceMode.Force);
        rb.AddForce(steeringForce, ForceMode.Force);
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void RotateSkies()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2f + 0.1f;

        RaycastHit[] hit = Physics.RaycastAll(start, Vector3.down, rayLength);
        if (hit.Length > 0)
        {
            if (rb.velocity.magnitude > 0.1f)
            {
                skiAngleY = Mathf.Atan2(rb.velocity.z, -rb.velocity.x) * Mathf.Rad2Deg - 90f;
            }
            else
            {
                skiAngleY = 0f;
            }

            skiLeft.rotation =
                Quaternion.Lerp(skiLeft.rotation,
                    Quaternion.Euler(0f, skiAngleY, 0f) * Quaternion.FromToRotation(transform.up, hit[0].normal), 0.5f)
                * transform.rotation;
            skiRight.rotation =
                Quaternion.Lerp(skiRight.rotation,
                    Quaternion.Euler(0f, skiAngleY, 0f) * Quaternion.FromToRotation(transform.up, hit[0].normal), 0.5f)
                * transform.rotation;
        }
    }
    
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
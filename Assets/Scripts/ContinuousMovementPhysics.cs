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
    [SerializeField] private Transform skies;
    
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

        if (rb.velocity.magnitude > 0.1f)
        {
            skiAngleY = Mathf.Atan2(rb.velocity.z, -rb.velocity.x) * Mathf.Rad2Deg - 90f;
            skies.rotation = Quaternion.Euler(0f, skiAngleY, 0f);
        }
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void GroundCheck()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2f + 0.1f;

        RaycastHit[] hit = Physics.RaycastAll(start, Vector3.down, rayLength);
        if (hit[0].transform != null)
        {
            
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
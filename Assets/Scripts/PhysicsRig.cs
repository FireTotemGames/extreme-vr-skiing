using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [SerializeField] private Transform playerHead;
    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    [SerializeField] private ConfigurableJoint headJoint;
    [SerializeField] private ConfigurableJoint leftHandJoint;
    [SerializeField] private ConfigurableJoint rightHandJoint;
    
    [SerializeField] private float minBodyHeight = 0.5f;
    [SerializeField] private float maxBodyHeight = 2f;
    
    
    
    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
         
    }
  
    private void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, minBodyHeight, maxBodyHeight);
        bodyCollider.center =
            new Vector3(playerHead.localPosition.x, bodyCollider.height / 2f, playerHead.localPosition.z);
        
        leftHandJoint.targetPosition = leftController.position;
        leftHandJoint.targetRotation = leftController.rotation;
        
        rightHandJoint.targetPosition = rightController.position;
        rightHandJoint.targetRotation = rightController.rotation;

        headJoint.targetPosition = playerHead.localPosition;
        headJoint.targetRotation = playerHead.localRotation;
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
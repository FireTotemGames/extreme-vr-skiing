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
    [SerializeField] private float decelerationFactor;
    [SerializeField] private float racingBoost;
    [SerializeField] private float racingHeightThreshold = 0.9f;
    [SerializeField] private float racingAngleThreshold = 50f;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform skiStickLeft;
    [SerializeField] private Transform skiStickRight;

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

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

        bool isGrounded = RotateSkies();

        // float roll = playerHead.rotation.z * Vector3.Dot(playerHead.forward, Vector3.forward);
        // Vector3 steeringForce = Vector3.right * -roll * steeringFactor;
        // rb.AddForce(steeringForce, ForceMode.Force);

        if (isGrounded == false)
        {
            return;
        }
        
        float leftHandSteering = Vector3.Dot(skiStickLeft.up, Vector3.right);
        float rightHandSteering = Vector3.Dot(skiStickRight.up, Vector3.right);
        Vector3 steeringForce = Vector3.right * steeringFactor * (leftHandSteering + rightHandSteering) / 2f;
        if (leftHandSteering > 0.1f && rightHandSteering > 0.1f)
        {
            rb.AddForce(steeringForce);
        }
        else if (leftHandSteering < -0.1 && rightHandSteering < -0.1f)
        {
            rb.AddForce(steeringForce);
        }

        float downhillAlignment = Vector3.Dot(Vector3.forward, rb.velocity.normalized);
        Vector3 decelerationForce = -rb.velocity * (1f - downhillAlignment) * decelerationFactor;
        rb.AddForce(decelerationForce, ForceMode.Force);

        if (leftHand.localPosition.y < racingHeightThreshold && rightHand.localPosition.y < racingHeightThreshold &&
            Vector3.Dot(skiStickLeft.up, Vector3.forward) > 0.5f && Vector3.Dot(skiStickRight.up, Vector3.forward) > 0.5f)
            // leftHand.localRotation.eulerAngles.x > racingAngleThreshold && rightHand.localRotation.eulerAngles.x > racingAngleThreshold)
        {   
            Vector3 racingForce = Quaternion.Euler(15f, 0f, 0f) * Vector3.forward * racingBoost;
            rb.AddForce(racingForce, ForceMode.Force);
        }
    }

    /* ======================================================================================================================== */
    /* COROUTINES                                                                                                               */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private bool RotateSkies()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2f + 0.1f;

        RaycastHit[] hit = Physics.RaycastAll(start, Vector3.down, rayLength);
        bool isGrounded = false;
        if (hit.Length > 0)
        {
            isGrounded = true;
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
                    Quaternion.Euler(0f, skiAngleY, 0f) * Quaternion.FromToRotation(transform.up, hit[0].normal), 1f)
                * transform.rotation;
            skiRight.rotation =
                Quaternion.Lerp(skiRight.rotation,
                    Quaternion.Euler(0f, skiAngleY, 0f) * Quaternion.FromToRotation(transform.up, hit[0].normal), 1f)
                * transform.rotation;
        }

        return isGrounded;
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
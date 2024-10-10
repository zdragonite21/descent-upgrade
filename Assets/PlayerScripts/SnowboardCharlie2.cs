using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SnowboardCharlie2 : MonoBehaviour
{
    [Header("Fields")]
    public float autoSpeed;
    public float fasterAutoSpeed;
    public float sideLeanForce;
    public float tiltingFactor;
    public float gravityScale;
    public float jumpForce;
    public float airSpinSpeed;
    public float airSommersault;
    public float rotationAdjustionFactor;
    [Header("References")]
    public Grounded playerGrounded;
    public Rigidbody rb;
    private float currentAutoSpeed;
    public Vector3 rbv;
    public PlayerState CurrentState
    {
        get => m_CurrentState;
        set
        {
            switch (value)
            {
                case PlayerState.Grounded:
                    OnGrounded();
                    m_UpdateHandler = GroundedBehavior;
                    break;
                case PlayerState.Midair:
                    OnMidair();
                    m_UpdateHandler = MidairBehavior;
                    break;
                default:
                    break;
            }

            m_CurrentState = value;
        }
    }
    private PlayerState m_CurrentState;
    private delegate void UpdateHandler();
    private UpdateHandler m_UpdateHandler;
    private Quaternion groundTargetRotation;
    public enum PlayerState
    {
        Grounded,
        Midair
    }
    public enum GroundState
    {
        Sliding, //no input
        Carving, //SA, SD
        Drifting, //A, D
        Shortturning, //WA, WD
        Straightening //W
    }
    public GroundState currGroundState;
    public float stateTimeCounter;
    public Vector3 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        currentAutoSpeed = autoSpeed;
        CurrentState = PlayerState.Midair;
    }
    float xInput;
    float yInput;
    // Update is called once per frame
    void Update()
    {
        // Gravity
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        m_UpdateHandler?.Invoke();
    }
    private void OnGrounded() {}
    private void GroundedBehavior()
    {
        print("IN Ground");
        // Get player input for movement
        if (xInput != 0 && yInput < 0 && currGroundState != GroundState.Carving) {
            currGroundState = GroundState.Carving;
            stateTimeCounter = 0;
        } else if (xInput != 0 && yInput == 0 && currGroundState != GroundState.Drifting){
            currGroundState = GroundState.Drifting;
            stateTimeCounter = 0;
        } else if (xInput != 0 && yInput > 0 && currGroundState != GroundState.Shortturning) {
            currGroundState = GroundState.Shortturning;
            stateTimeCounter = 0;
        } else if (xInput == 0 && yInput != 0) {
            currGroundState = GroundState.Straightening;
            stateTimeCounter = 0;
        } else {
            currGroundState = GroundState.Sliding;
        }
        SetSlopeSlideVelocity();
        rb.velocity += transform.right * xInput * sideLeanForce * Time.deltaTime;
        float xVel = rb.velocity.x;
        currentAutoSpeed = Mathf.Clamp(currentAutoSpeed + Time.deltaTime * yInput, autoSpeed, fasterAutoSpeed);
        
        rb.velocity += transform.forward * currentAutoSpeed * Time.deltaTime;
        rb.velocity += transform.forward * Time.deltaTime;
        // Jumps
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            float jumpFactor = 1;
            if (currGroundState == GroundState.Straightening) {
                jumpFactor = 1 + stateTimeCounter * 0.01f;
            }
            rb.AddForce(new Vector3(0f, jumpForce * jumpFactor * 10f, 0f), ForceMode.Impulse);
            print(jumpFactor);  
        }
    }
    private void OnMidair()
    {
        rb.AddTorque(transform.up * airSommersault * xInput * tiltingFactor * 10f, ForceMode.Impulse);
        rb.AddTorque(transform.right * airSpinSpeed * yInput * tiltingFactor * 10f, ForceMode.Impulse);
    }
    private void MidairBehavior()
    {
        print("IN AIR");
        rb.velocity += 1.5f * Vector3.down * gravityScale * Time.deltaTime;
        rb.velocity += transform.right * xInput * sideLeanForce * Time.deltaTime * 0.5f;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, rb.velocity) * transform.rotation;
        rb.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
            Time.fixedDeltaTime * tiltingFactor * 0.5f);
        float maxAngVel = 7f;

        if (rb.angularVelocity.magnitude < maxAngVel)
        {
            rb.AddTorque(transform.right * yInput * tiltingFactor * Time.fixedDeltaTime * (maxAngVel - rb.angularVelocity.x) / maxAngVel, ForceMode.VelocityChange);
            rb.AddTorque(transform.up * xInput * tiltingFactor * Time.fixedDeltaTime * (maxAngVel - rb.angularVelocity.y) / maxAngVel, ForceMode.VelocityChange);
        }
    }
    private void SetSlopeSlideVelocity()
    {
        float directionalForce = 0f;
        float angle = 0f;

        if (Physics.Raycast(transform.position + Vector3.up + transform.forward, Vector3.down, out RaycastHit hitInfo, 5f, playerGrounded.layerMask))
        {
            groundTargetRotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
            if (currGroundState == GroundState.Straightening) {
                transform.rotation = Quaternion.Slerp(transform.rotation, groundTargetRotation, Time.deltaTime * tiltingFactor);  
            }
            angle = Vector3.Angle(hitInfo.normal, Vector3.up); // Angle from ground up
            directionalForce = gravityScale * Mathf.Abs(Mathf.Sin(angle));
        }
        // Flat Rotation
        groundTargetRotation = Quaternion.FromToRotation(transform.forward, (Vector3.right * xInput)) * transform.rotation;
       
        if (currGroundState == GroundState.Sliding) {
            groundTargetRotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
            // Adhere to normals
            transform.rotation = Quaternion.Slerp(transform.rotation, groundTargetRotation, rotationAdjustionFactor * Time.fixedDeltaTime);
        }
        if (currGroundState == GroundState.Carving) {
            groundTargetRotation = Quaternion.FromToRotation(transform.TransformDirection(-1 * xInput, 1, 0), hitInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, groundTargetRotation, rotationAdjustionFactor * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(groundTargetRotation.x, 90 * xInput, groundTargetRotation.z), Time.fixedDeltaTime * tiltingFactor);
            if (stateTimeCounter < 100) {
                rb.AddForce(rb.velocity * -1f * Mathf.Abs(xInput));
            } 
        }
        if (currGroundState == GroundState.Drifting) {
            groundTargetRotation = Quaternion.FromToRotation(transform.TransformDirection(-1 *xInput, 1, 0), hitInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, groundTargetRotation, rotationAdjustionFactor * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(groundTargetRotation.x, 90 * xInput, groundTargetRotation.z), Time.fixedDeltaTime * tiltingFactor);
            if (stateTimeCounter < 100) {
                rb.AddForce(rb.velocity *0.2f * Mathf.Abs(xInput));
            } else {
                rb.AddForce(rb.velocity *-0.5f * Mathf.Abs(xInput));
            }
        }

        if (currGroundState == GroundState.Shortturning) {
            groundTargetRotation = Quaternion.FromToRotation(transform.TransformDirection(-0.75f *xInput, 1, 0), hitInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, groundTargetRotation, rotationAdjustionFactor * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(groundTargetRotation.x, 45 * xInput, groundTargetRotation.z), Time.fixedDeltaTime * tiltingFactor);
            if (stateTimeCounter < 100) {
                rb.AddForce(rb.velocity *0.3f * Mathf.Abs(xInput));
            }
        }
        if (currGroundState == GroundState.Straightening) {
            groundTargetRotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, groundTargetRotation, rotationAdjustionFactor * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(groundTargetRotation.x, 0 * xInput, groundTargetRotation.z), Time.fixedDeltaTime * tiltingFactor);
        }

        // Diagonal Gravity
        rb.velocity += directionalForce * Vector3.down * Time.fixedDeltaTime;
        // Horizontal "Gravity" to make the player go up slopes
        if (Mathf.Sin(angle) < 0)
        {
            rb.velocity += Mathf.Abs(Mathf.Cos(angle)) * transform.forward * Time.fixedDeltaTime * currentAutoSpeed;
        }

    }
    void FixedUpdate(){
        rbv = rb.velocity;
        stateTimeCounter++;
    }
}
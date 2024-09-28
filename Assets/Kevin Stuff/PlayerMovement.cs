using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    [Header("Fields")]
    public float autoSpeed;

    public float forwardLeanForce;

    public float sideLeanForce;

    public float tiltingFactor;

    public float gravityScale;

    public float jumpForce;

    public float rotationAdjustionFactor;

    [Header("References")]
    public Grounded playerGrounded;
    public Rigidbody rb;



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

    public enum PlayerState
    {
        Grounded,
        Midair
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentState = PlayerState.Midair;
    }


    float xInput;
    float yInput;
    // Update is called once per frame
    void Update()
    {
        // Gravity
        //
        //rb.velocity += transform.forward * autoSpeed * Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        m_UpdateHandler?.Invoke();
    }

    private void OnGrounded()
    {


    }
    private void GroundedBehavior()
    {
        print("IN Ground");
        // Get player input for movement



        SetSlopeSlideVelocity();
        rb.velocity += transform.right * xInput * sideLeanForce * Time.deltaTime;

        float xVel = rb.velocity.x;

        rb.velocity += (transform.forward * yInput * forwardLeanForce * Time.deltaTime);
        rb.velocity += (transform.forward * Time.deltaTime);


        // Jumps
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity += new Vector3(0f, jumpForce, 0f);
        }

        // Leaning

    }

    private void OnMidair()
    {
        rb.AddTorque(transform.up * xInput * tiltingFactor, ForceMode.Impulse);

        rb.AddTorque(transform.right * yInput * tiltingFactor, ForceMode.Impulse);

    }
    private void MidairBehavior()
    {
        print("IN AIR");
        rb.velocity += Vector3.down * gravityScale * Time.deltaTime;
        rb.velocity += transform.right * xInput * sideLeanForce * Time.deltaTime * 0.5f;

        rb.rotation = Quaternion.RotateTowards(transform.rotation, 
            Quaternion.AngleAxis(-xInput * Time.deltaTime * 10, transform.forward),
            Time.deltaTime * 100f);

        //Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, (Vector3.right * xInput)) * transform.rotation;
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
        //    Time.fixedDeltaTime * tiltingFactor);

        rb.AddTorque(transform.up * xInput * tiltingFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);

        rb.AddTorque(transform.right * yInput * tiltingFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);


    }


    private void SetSlopeSlideVelocity()
    {
        float directionalForce = 0f;
        float angle = 0f;

        if (Physics.Raycast(transform.position + Vector3.up + transform.forward, Vector3.down, out RaycastHit hitInfo, 5f, playerGrounded.layerMask))
        {
            angle = Vector3.Angle(hitInfo.normal, Vector3.up); // Angle from ground up

            directionalForce = gravityScale * Mathf.Abs(Mathf.Sin(angle));

        }

        //transform.up = Vector3.Lerp(transform.up, hitInfo.normal, rotationAdjustionFactor * Time.deltaTime);

        Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, (Vector3.right * xInput) + Vector3.forward * Mathf.Abs(xInput)) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
            Time.fixedDeltaTime * tiltingFactor);

        targetRotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationAdjustionFactor * Time.fixedDeltaTime);


        // Diagonal Gravity
        rb.velocity += directionalForce * Vector3.down * Time.deltaTime;

        // Horizontal "Gravity" to make the player go up slopes
        if (Mathf.Sin(angle) < 0)
        {
            //print("YO");
            rb.velocity += Mathf.Abs(Mathf.Cos(angle)) * transform.forward * Time.deltaTime * autoSpeed;
            //Debug.DrawRay(transform.position, (Mathf.Abs(Mathf.Cos(angle)) * transform.forward * Time.deltaTime * autoSpeed).normalized, Color.red);
        }



        //print(directionalForce);
    }
}
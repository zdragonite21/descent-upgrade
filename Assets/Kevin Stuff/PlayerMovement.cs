using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Fields")]
    public float autoSpeed;

    public float leanForce;

    public float gravityScale;


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

    // Update is called once per frame
    void Update()
    {
        // Gravity
        //
        //rb.velocity += transform.forward * autoSpeed * Time.deltaTime;

        m_UpdateHandler?.Invoke();
    }


    private void OnGrounded()
    {


    }
    private void GroundedBehavior()
    {
        print("IN Ground");
        // Get player input for movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxisRaw("Vertical");


        SetSlopeSlideVelocity();
        rb.velocity += transform.right * x * leanForce * Time.deltaTime;

        float xVel = rb.velocity.x;

        rb.velocity += transform.forward * Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity += new Vector3(0f, 2f * gravityScale * 0.7f, 0f);
        }

    }

    private void OnMidair()
    {
    }
    private void MidairBehavior()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        rb.velocity += Vector3.down * gravityScale * Time.deltaTime;
        rb.velocity += transform.right * x * leanForce * Time.deltaTime * 0.5f;

    }


    private void SetSlopeSlideVelocity()
    {
        float directionalForce = 0f;
        float angle = 0f;

        if (Physics.Raycast(transform.position + Vector3.up + transform.forward, Vector3.down, out RaycastHit hitInfo, 5f, playerGrounded.layerMask))
        {
            angle = Vector3.Angle(hitInfo.normal, Vector3.up); // Angle from ground up

            directionalForce = gravityScale * Mathf.Abs(Mathf.Sin(angle));
            transform.up = hitInfo.normal;
        }

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
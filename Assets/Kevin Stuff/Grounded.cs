using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Grounded : MonoBehaviour
{
    public bool IsGrounded;

    public LayerMask layerMask;

    public SnowboardCharlie2 playerMovement;
    public CameraMovement cameraMovement;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (IsGrounded)
    //        return;

    //    if (layerMask == (layerMask | (1 << other.gameObject.layer)))
    //    {
    //        IsGrounded = true;
    //        playerMovement.CurrentState = PlayerMovement.PlayerState.Grounded;
    //        cameraMovement.CurrentState = CameraMovement.CameraState.Grounded;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (layerMask == (layerMask | (1 << other.gameObject.layer)))
    //    {

    //        IsGrounded = false;
    //        playerMovement.CurrentState = PlayerMovement.PlayerState.Midair;
    //        cameraMovement.CurrentState = CameraMovement.CameraState.Midair;

    //    }
    //}

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 2f, layerMask))
        {
            if (!IsGrounded)
            {
                IsGrounded = true;
                playerMovement.CurrentState = SnowboardCharlie2.PlayerState.Grounded;
                cameraMovement.CurrentState = CameraMovement.CameraState.Grounded;
            }
        }
        else
        {
            if (IsGrounded)
            {
                IsGrounded = false;
                playerMovement.CurrentState = SnowboardCharlie2.PlayerState.Midair;
                cameraMovement.CurrentState = CameraMovement.CameraState.Midair;
            }
        }

    }

}

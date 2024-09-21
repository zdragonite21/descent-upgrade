using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Collider))]
public class Grounded : MonoBehaviour
{
    public bool IsGrounded;

    public LayerMask layerMask;

    public PlayerMovement playerMovement;
    public CameraMovement cameraMovement;


    private void OnTriggerStay(Collider other)
    {
        if (IsGrounded)
            return;

        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            IsGrounded = true;
            playerMovement.CurrentState = PlayerMovement.PlayerState.Grounded;
            cameraMovement.CurrentState = CameraMovement.CameraState.Grounded;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        

        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            IsGrounded = false;
            playerMovement.CurrentState = PlayerMovement.PlayerState.Midair;
            cameraMovement.CurrentState = CameraMovement.CameraState.Midair;

        }
    }
}

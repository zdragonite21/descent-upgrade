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
    public Death death;

    private void OnTriggerEnter(Collider other)
    {   
        if (!IsGrounded) {death.CheckDeath(); IsGrounded = false; print("Hitbox Check");}
    }

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
        if (!death.isDead) {
            if (Physics.Raycast(transform.parent.position, -1 * transform.parent.up, 2f, layerMask))
            {
                if (!IsGrounded)
                {
                    print("Landing Check");
                    death.CheckDeath();
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

}

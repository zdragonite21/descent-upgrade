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

    public BoxCollider groundedCollider;

    List<Collider> hitColliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (IsGrounded)
            return;

        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            hitColliders.Add(other);
            IsGrounded = true;
            playerMovement.CurrentState = PlayerMovement.PlayerState.Grounded;
            cameraMovement.CurrentState = CameraMovement.CameraState.Grounded;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            hitColliders.Clear();

            IsGrounded = false;
            playerMovement.CurrentState = PlayerMovement.PlayerState.Midair;
            cameraMovement.CurrentState = CameraMovement.CameraState.Midair;

        }
    }

    private void Update()
    {

        if (hitColliders.Count == 0 && IsGrounded)
        {
            IsGrounded = false;
            playerMovement.CurrentState = PlayerMovement.PlayerState.Midair;
            cameraMovement.CurrentState = CameraMovement.CameraState.Midair;
        }

    }

}

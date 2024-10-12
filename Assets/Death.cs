using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Build.Content;
using UnityEditor.Callbacks;
using UnityEngine;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    public bool isDead;
    public SnowboardCharlie2 pmovement;
    public float deathDelay;
    public Grounded playerGrounded;
    private Rigidbody playerRB;
    private Vector3 deathlaunch;
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        deathDelay = 0;
        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (deathDelay == 1) {
            Die();
        }
    }

    public bool CheckDeath() {
        bool isToDie = false;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 5f, playerGrounded.layerMask))
        {   
            float landingAngle = 0;
            if (Mathf.Abs(this.transform.parent.rotation.eulerAngles.x -360) < 30) {
                landingAngle = Mathf.Abs(this.transform.parent.rotation.eulerAngles.x -360) %360;
            } else {
                landingAngle = this.transform.parent.rotation.eulerAngles.x %360;
            }
             
            float groundAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
            // if (this.transform.parent.rotation.eulerAngles.x > 180) {
            //     landingAngle = 360 - this.transform.parent.rotation.eulerAngles.x;
            // } else {
            //     landingAngle = this.transform.parent.rotation.eulerAngles.x;
            // }
            isToDie = !(landingAngle > groundAngle - 45 && landingAngle < groundAngle+45);
            print(landingAngle+  " " + groundAngle);
            if (isToDie) {
                deathDelay += 5f;
                playerRB.drag = 0.75f;
                //deathlaunch = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
                
            }
        }
       
        return isToDie;
    }

    private void OnTriggerEnter(Collider other)
    {
        Die();
    }
    public void Die()
    {   
        pmovement.enabled = false;
        // playerRB.AddForce(Vector3.Normalize(deathlaunch), ForceMode.Impulse);
        // playerRB.AddTorque(transform.up * 15f, ForceMode.Impulse);
        isDead = true;
        gameManager.GameOver();
    }
    void FixedUpdate() {
        if (deathDelay >= 1) {
            deathDelay--;
        }
    }

}

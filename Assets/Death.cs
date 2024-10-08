using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    public bool isDead;
    public SnowboardCharlie2 pmovement;
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        isDead = true;
        gameManager.GameOver();
        pmovement.enabled = false;
    }
}

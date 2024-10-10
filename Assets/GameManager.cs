using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float points;
    private GameObject player;
    public float lastZ;
    public TextMeshProUGUI hudPointsText;

    public GameOverScreen gmover;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lastZ = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        if (player.transform.position.z > lastZ) {
            points += player.transform.position.z - lastZ;
        }
        lastZ = player.transform.position.z;
        hudPointsText.text = ((int) points).ToString();
    }

    public void GameOver() {
        gmover.SetUp((int) points);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public String mainGameSceneName;
    public String sandGameSceneName;
    public void SetUp(int score) {
        gameObject.SetActive(true);
        pointsText.text = score.ToString();
    }

    public void Restart() {
        int scene = UnityEngine.Random.Range(0,2);
        if (scene == 0) {
            SceneManager.LoadScene(mainGameSceneName);
        } else {
            SceneManager.LoadScene(sandGameSceneName);
        }
        
    }

}

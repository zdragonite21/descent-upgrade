using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float airScoreAdd;
    [SerializeField] private float spinScoreAdd;
    [SerializeField] private float vSpinScoreAdd;
    [SerializeField] TMP_Text pointTotalText;
    [SerializeField] TMP_Text pointAddText;
    [SerializeField] TMP_Text pointGradeText;
    private int pointTotal;
    private float pointTrickAdd;
    private Rigidbody player;
    private bool trickActive;
    private Vector3 angleDeltas;
    private float timeTricking;
    private bool trickHSpin;
    private bool trickVSpin;

    private void Start()
    {
        trickActive = false;
        pointAddText.gameObject.SetActive(false);
    }

    public void Setup(Rigidbody rb)
    {
        player = rb;
    }

    public void ScoreTrick(bool score)
    {
        trickActive = score;
        if (score)
        {
            timeTricking = 0f;
            pointTrickAdd = 0f;
            angleDeltas = Vector3.zero;
            pointAddText.gameObject.SetActive(true);
            pointGradeText.gameObject.SetActive(true);
            pointGradeText.text = "";
            trickHSpin = false;
            trickVSpin = false;
        }
        else
        {
            pointTotal += Mathf.RoundToInt(pointTrickAdd);
            pointAddText.gameObject.SetActive(false);
            pointGradeText.gameObject.SetActive(false);
            pointTotalText.text = pointTotal + "";
        }
    }

    private void Update()
    {
        if (trickActive)
        {
            timeTricking += Time.deltaTime;
            angleDeltas += new Vector3(Mathf.Rad2Deg * player.angularVelocity.x, Mathf.Rad2Deg * player.angularVelocity.y, Mathf.Rad2Deg * player.angularVelocity.z) * Time.deltaTime;
            //Debug.Log(angleDeltas);
            if (Mathf.Abs(player.angularVelocity.y) >= 3f)
                trickHSpin = true;
            if (Mathf.Abs(player.angularVelocity.x) >= 4f)
                trickVSpin = true;
            pointTrickAdd += airScoreAdd * Time.deltaTime;
            if (trickHSpin)
                pointTrickAdd += spinScoreAdd * Time.deltaTime;
            if (trickVSpin)
                pointTrickAdd += vSpinScoreAdd * Time.deltaTime;
            pointAddText.text = "+" + Mathf.RoundToInt(pointTrickAdd);
            string gradeText = "";
            if (trickHSpin)
                gradeText = "Good!";
            if (trickVSpin)
                gradeText = "Great!";
            if (trickHSpin && trickVSpin && timeTricking >= 4f)
                gradeText = "Excellent!";
            pointGradeText.text = gradeText;
        }
    }

    /*
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
    */

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Scoreboard : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject managerObj;

    [Header("ui text fields")]
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text yourScoreText;


    void Awake()
    {
        managerObj = GameObject.Find("GameManager");
        gameManager = managerObj.GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        //display values
        highScoreText.text = $"high score: {PlayerPrefs.GetInt("HighScore", 0)} pts";
        yourScoreText.text = $"your score: {gameManager.points.ToString()} pts";

    }
}
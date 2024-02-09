using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class UIText : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject managerObj;

    private float points = 0;

    [Header("ui text fields")]
    [SerializeField] TMP_Text lifeComponenet;
    [SerializeField] TMP_Text asteroidsLeftComponenet;
    [SerializeField] TMP_Text gameOverText;
    [SerializeField] TMP_Text pausedText;


    void Awake()
    {
        managerObj = GameObject.Find("GameManager");
        gameManager = managerObj.GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        //calculate score
        points = (gameManager.totalAsteroidsDestroyed) * 150f;

        //display values
        lifeComponenet.text = gameManager.lives.ToString() + " lives";
        asteroidsLeftComponenet.text = points.ToString() + " points";

        //ternary operator to display game over text
        //if game is over display game over text
        gameOverText.text = gameManager.gameIsOver ? "GAME OVER" : "";
        pausedText.text = gameManager.gamePaused ? "PAUSED" : "";
    }
}

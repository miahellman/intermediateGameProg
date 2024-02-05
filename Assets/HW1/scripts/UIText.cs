using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIText : MonoBehaviour
{

    public GameManager gameManager;
    public GameObject managerObj;

    [Header("ui text fields")]
    [SerializeField] TMP_Text levelComponenet;
    [SerializeField] TMP_Text asteroidsLeftComponenet;

    void Awake()
    {
        managerObj = GameObject.Find("GameManager");
        gameManager = managerObj.GetComponent<GameManager>();

    }
    // Update is called once per frame
    void Update()
    {
        //display values
        levelComponenet.text = "level " + gameManager.level.ToString();
        asteroidsLeftComponenet.text = "asteroids destroyed: " + gameManager.totalAsteroidsDestroyed.ToString();
    }
}

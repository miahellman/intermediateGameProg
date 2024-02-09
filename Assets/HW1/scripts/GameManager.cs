using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Asteroid asteroidPrefab;

    public int asteroidCount = 0;
    public int totalAsteroidsDestroyed = 0;
    public int level = 0;
    public int lives = 3;
    public float points = 0;
    public bool gameIsOver = false;
    public bool gamePaused = false;

    //only one gamemanager!!!
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


    }


    private void Update()
    {
        points = totalAsteroidsDestroyed * 150f;


        //scene selection
        if (Input.GetKeyUp("space"))
        {
            if (SceneManager.GetActiveScene().name == "Start")
            {
                //reset game values + load main scene
                SceneManager.LoadScene("Main");
                totalAsteroidsDestroyed = 0;
                asteroidCount = 0;
                level = 0;
                lives = 3;
                gameIsOver = false;
            }

            if (SceneManager.GetActiveScene().name == "end")
            {
                SceneManager.LoadScene("Start");
            }
        }


        if (SceneManager.GetActiveScene().name == "Main")
        {
            //spawn new asteroids if all are destroyed
            if (asteroidCount == 0)
            {
                //+1 level
                level++;

                //spawn enemies for level
                int numAsteroids = 2 + (2 * level);
                for (int i = 0; i < numAsteroids; i++)
                {
                    SpawnAsteroid();
                }
            }

            //pause game function for gameplay scene only
            if (Input.GetKeyUp(KeyCode.Escape) && gamePaused == false) {gamePaused = true;} 
            else if (Input.GetKeyUp(KeyCode.Escape) && gamePaused == true) { gamePaused = false;}
            Time.timeScale = gamePaused ? 0 : 1;
        }
    }

    private void SpawnAsteroid()
    {
        //spawn offscreen
        float offset = Random.Range(0f, 1f);
        Vector2 viewportSpawnPosition = Vector2.zero;

        //edge check
        int edge = Random.Range(0, 4);
        if (edge == 0)
        {
            viewportSpawnPosition = new Vector2(offset, 0);
        }
        else if (edge == 1)
        {
            viewportSpawnPosition = new Vector2(offset, 1);
        }
        else if (edge == 2)
        {
            viewportSpawnPosition = new Vector2(0, offset);
        }
        else if (edge == 3)
        {
            viewportSpawnPosition = new Vector2(1, offset);
        }

        //instantiate asteroids
        Vector2 worldSpawnPosition = Camera.main.ViewportToWorldPoint(viewportSpawnPosition);
        Asteroid asteroid = Instantiate(asteroidPrefab, worldSpawnPosition, Quaternion.identity);
        asteroid.gameManager = this;
    }

    //gameover function -- calls respawn or restart
    public void GameOver()
    {
        if (lives <= 0)
        {
            StartCoroutine(Restart());
        }
        else
        {
            lives--;
            StartCoroutine(Respawn());
        }
        
    }

    //respawn coroutine
    private IEnumerator Respawn()
    {
        Debug.Log("Respawning");
        
        // Wait a bit before respawning.
        yield return new WaitForSeconds(3.5f);

        // Respawn player.
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        yield return null;
    }

    //gameover coroutine
    private IEnumerator Restart()
    {
        lives = 0;
        Debug.Log("Game Over");
        gameIsOver = true;
        // Wait a bit before restarting.
        yield return new WaitForSeconds(3.5f);

        // Restart scene.
        SceneManager.LoadScene("end");

        yield return null;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Asteroid asteroidPrefab;

    public int asteroidCount = 0;

    public int totalAsteroidsDestroyed = 0;

    public int level = 0;

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
        //scene selection
        if (Input.anyKey)
        {
            if (SceneManager.GetActiveScene().name == "Start")
            {
                SceneManager.LoadScene("Main");
                totalAsteroidsDestroyed = 0;
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

    public void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("Start");
    }

}
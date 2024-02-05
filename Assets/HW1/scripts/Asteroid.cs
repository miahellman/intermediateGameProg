using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyedParticles;
    public int size = 3;

    public GameManager gameManager;

    private void Start()
    {
        transform.localScale = 0.5f * size * Vector3.one;

        //size based movement (bigger = more slow)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(Random.value, Random.value).normalized;
        float spawnSpeed = Random.Range(4f - size, 5f - size);
        rb.AddForce(direction * spawnSpeed, ForceMode2D.Impulse);

        //add to asteroid counter
        //i was gonna use a list but isnt that lowk inefficient
        gameManager.asteroidCount++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //bullet collisions
        if (collision.CompareTag("Bullet"))
        {
            gameManager.totalAsteroidsDestroyed++;
            //if destroyed subtract from asteroid counter
            gameManager.asteroidCount--;

            //destroy bullet on collison
            Destroy(collision.gameObject);

            //if bigger asteroid => spawn smaller asteroids
            if (size > 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    Asteroid newAsteroid = Instantiate(this, transform.position, Quaternion.identity);
                    newAsteroid.size = size - 1;
                    newAsteroid.gameManager = gameManager;
                }
            }

            //destruction particle fx
            Instantiate(destroyedParticles, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
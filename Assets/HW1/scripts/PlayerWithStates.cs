using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWithStates : MonoBehaviour
{
    //enum to represent player states
    private enum PlayerState
    {
        Alive,
        Dead
    }

    private PlayerState currentState = PlayerState.Alive;

    // Your existing variables...
    [Header("player parameters")]
    [SerializeField] private float shipAcceleration = 10f;
    [SerializeField] private float shipMaxVelocity = 10f;
    [SerializeField] private float shipRotationSpeed = 180f;
    [SerializeField] private float bulletSpeed = 8f;

    [Header("bullets + fx")]
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Rigidbody2D bulletPrefab;
    [SerializeField] private ParticleSystem destroyedParticles;

    private Rigidbody2D shipRigidbody;
    private bool isAccelerating = false;

    public GameManager gameManager;

    private void Start()
    {
        //find gamemanager when respawning
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        //initialize player rigidbody
        shipRigidbody = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Alive:
                HandleAliveState();
                break;
            case PlayerState.Dead:
                //handle Dead state
                //(theres nothing here bc if dead player can't interact w game)
                break;
            default:
                break;
        }
    }

    private void HandleAliveState()
    {
        HandleShipAcceleration();
        HandleShipRotation();
        HandleShooting();
    }

    private void FixedUpdate()
    {
        if (currentState == PlayerState.Alive && isAccelerating)
        {
            //increase velocity until maxVelocity 
            shipRigidbody.AddForce(shipAcceleration * transform.up);
            shipRigidbody.velocity = Vector2.ClampMagnitude(shipRigidbody.velocity, shipMaxVelocity);
        }
    }

    private void HandleShipAcceleration()
    {
        //acceleration
        isAccelerating = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
    }

    private void HandleShipRotation()
    {
        //rotation
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Rotate(shipRotationSpeed * Time.deltaTime * transform.forward);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-shipRotationSpeed * Time.deltaTime * transform.forward);
        }
    }

    private void HandleShooting()
    {
        if (!gameManager.gamePaused)
        {
            //shooting
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {

                Rigidbody2D bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

                Vector2 shipVelocity = shipRigidbody.velocity;
                Vector2 shipDirection = transform.up;
                float shipForwardSpeed = Vector2.Dot(shipVelocity, shipDirection);

                //make sure no issues w/ bullet velocity 
                if (shipForwardSpeed < 0)
                {
                    shipForwardSpeed = 0;
                }

                bullet.velocity = shipDirection * shipForwardSpeed;

                //add force to "shoot" bullet
                bullet.AddForce(bulletSpeed * transform.up, ForceMode2D.Impulse);
            }
        }
        
    }

    //asteroid collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            currentState = PlayerState.Dead;

            GameManager gameManager = FindAnyObjectByType<GameManager>();

            //gameover on collision
            gameManager.GameOver();

            //destruction particle effect
            Instantiate(destroyedParticles, transform.position, Quaternion.identity);

            //destroy player obj
            Destroy(gameObject);
        }
    }
}
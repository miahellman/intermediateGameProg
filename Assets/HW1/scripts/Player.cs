using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
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
    private bool isAlive = true;
    private bool isAccelerating = false;

    private void Start()
    {
        //initialize player rigidbody
        shipRigidbody = GetComponent<Rigidbody2D>();
        Vector3 shipStartingPos = transform.position;
    }

    private void Update()
    {
        //if player is alive do [x] function
        //i assume this could become much more streamlined with a state machine

        if (isAlive)
        {
            HandleShipAcceleration();
            HandleShipRotation();
            HandleShooting();
        }
    }

    private void FixedUpdate()
    {
        if (isAlive && isAccelerating)
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
        //rotation -- should probs use input system but im just hardcoding -- which is better in this case??
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            isAlive = false;

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
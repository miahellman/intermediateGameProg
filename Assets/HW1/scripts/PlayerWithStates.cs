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

    //invincibility timer variables
    [Header("invincibility")]
    [SerializeField] private float invincibilityDuration = 3f;
    [SerializeField] bool isInvincible = true;
    //sprite renderer component for alpha manipulation
    private SpriteRenderer spriteRenderer;

    //ONLY ONE INSTANCE OF THE PLAYER IS ALLOWED!!!
    public static PlayerWithStates Instance { get; private set; }
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


    private void Start()
    {
        //initialize player rigidbody
        shipRigidbody = GetComponent<Rigidbody2D>();

        //start invincibility timer
        StartCoroutine(InvincibilityTimer());

        //get spriterenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //invincibility coroutine
    private IEnumerator InvincibilityTimer()
    {
        //wait for invincibility duration
        yield return new WaitForSeconds(invincibilityDuration);
        //set invincibility to false when timer ends
        isInvincible = false;
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

        //update sprite opacity
        UpdateSpriteOpacity();
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


    //modify opacity function
    private void UpdateSpriteOpacity()
    {
        if (isInvincible)
        {
            //set color
            Color color = spriteRenderer.color;
            //decrease opacity gradually using lerp and pingpong
            color.a = Mathf.Lerp(1f, 0f, Mathf.PingPong(Time.time, 1)); 
            spriteRenderer.color = color;
        }
        else
        {
            //reset opacity when not invincible
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    //asteroid collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInvincible && collision.CompareTag("Asteroid"))
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
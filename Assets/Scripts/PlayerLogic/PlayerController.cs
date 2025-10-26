using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float upSpeed = 10f;
    public float maxUpSpeed = 200;
    public Transform gameCamera;

    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;
    private Vector2 movement;
    [SerializeField] private float fallThreshold = -0.1f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator _animator;

    private bool faceRightState = true;
    private bool onGroundState = true;
    private bool jumpedState = false;
    private int jumpCount = 0;
    [SerializeField] private int maxJumps = 2;

    private bool moving = false;
    private bool alive = true;
    private bool isDead = false;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();

        
        _animator.SetBool("onGround", true);
        _animator.SetBool("isRunning", false);

    }
    void Update()
    {
        IsGrounded();
    }

    void FixedUpdate()
    {

        FallDetector();
        if (alive)
        {
            if (moving)
            {
                HandleMovement(faceRightState == true ? 1 : -1);
                _animator.SetFloat("xSpeed", Mathf.Abs(playerBody.linearVelocity.x));
                _animator.SetBool("isRunning", true);
            }
            else
            {

                _animator.SetBool("isRunning", false);
            }
        }

        
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipPlayerSprite(value);
            moving = true;
            HandleMovement(value);
        }
    }

    void FlipPlayerSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            playerSprite.flipX = true;
        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            playerSprite.flipX = false;
        }
    }

    void HandleMovement(int value)
    {
        movement.x = value * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    public void Jump()
    {
        if (!alive) return;

        if (jumpCount < maxJumps) // to allow double jumps
        {
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, 0f); // reset vertical velocity for consistent jumps
            playerBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);

            if (playerBody.linearVelocityY > maxUpSpeed)
                playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, maxUpSpeed);
            
            jumpCount++;
            jumpedState = true;
            
            _animator.SetBool("isJumping", true);
            _animator.SetBool("isFalling", false);
            _animator.SetInteger("jumpCount", jumpCount);

            if (jumpCount == 1)
            {
                _animator.SetTrigger("jump");
            }
            else if (jumpCount > 1)
            {
                _animator.SetTrigger("doubleJump");
            }

            Debug.Log("i jumped " + jumpCount + " time(s)");
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // _animator.SetBool("isJumpingMore", true);
            playerBody.AddForce(Vector2.up * upSpeed * 50, ForceMode2D.Force);
            jumpedState = false;
        }
    }

    public void FallDetector()
    {
        if (playerBody.linearVelocityY < fallThreshold)
        {
            _animator.SetBool("isFalling", true);
            _animator.SetBool("isJumping", false);
        }
        else
        {
            _animator.SetBool("isFalling", false);
        }
    }

    // Check if onGround
    public void IsGrounded()
    {
        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (groundedNow)
        {
            onGroundState = true;
            jumpCount = 0; // reset double jump

            _animator.SetBool("onGround", true);
            _animator.SetBool("isFalling", false);
            _animator.SetInteger("jumpCount", 0);
        }
        else
        {
            onGroundState = false;
            _animator.SetBool("onGround", false);

            if (playerBody.linearVelocityY < -0.1f)
            {
                _animator.SetBool("isFalling", true);
            }
        }
    }

    public bool IsAlive()
    {
        return alive;
    }
    
    public void DisableMovement()
    {
        alive = false;
        movement = Vector2.zero;
    }

    public void Kill()
    {
        Debug.Log("Player has died..");
        gameManager.GameOver();
    }

    public void GameRestart()
    {
        playerBody.transform.position = new Vector3(-6.54f, -0.15f, 0);
        playerBody.linearVelocity = Vector3.zero;
        
        faceRightState = true;
        playerSprite.flipX = false;
        
        onGroundState = true;
        alive = true;
        
        gameManager.ResetScore();
    }
}
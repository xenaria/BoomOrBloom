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

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();

    }

    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {

        IsGrounded(); // always check if player is grounded

        if (alive)
        {
            if (moving)
            {
                //_animator.SetBool("isRunning", moving);
                HandleMovement(faceRightState == true ? 1 : -1);
                Debug.Log("<color-pink>moving");
            }
            else
            {
                //_animator.SetBool("isRunning", false);
            }
        }

        if (playerBody.linearVelocityY < fallThreshold)
        {
            //_animator.SetBool("isFalling", true);
        }
        else
        {
            //_animator.SetBool("isFalling", false);
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
            Debug.Log("<color-pink>MoveCheck() called");
            HandleMovement(value);
        }
    }

    void FlipPlayerSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            playerSprite.flipX = true;
            // if (echoBody.linearVelocity.x > 0.05f)
            //     echoAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            playerSprite.flipX = false;
            // if (echoBody.linearVelocity.x < -0.05f)
            //     echoAnimator.SetTrigger("onSkid");
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

    // Check if onGround
    public void IsGrounded()
    {
        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        if (groundedNow)
        {
            onGroundState = true;
            jumpCount = 0; // reset double jump
        }
        else
        {
            onGroundState = false;
        }
        //_animator.SetBool("onGround", onGroundState);
    }

    public void Kill()
    {
        Debug.Log("Player has died..");
        alive = false;

        Time.timeScale = 0.0f;
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 100;
    private bool isInvulnerable = false;


    //movement variables
    float moveInput;
    private bool jumpPressed;

    private float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;

    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public float jumpHoldAcceleration = 50f;
    public float maxJumpHoldTime = 0.15f;
    public float maxJumpSpeed = 15f;


    public Transform groundCheck;
    public LayerMask groundLayer;
    public Image healthImage;

    public AudioClip jumpClip;
    public AudioClip hurtClip;
    public AudioClip deathClip;

    private Rigidbody2D rb;

    private Animator animator;

    private AudioSource audioSource;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject whipObject;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.1f;
    private bool isTouchingWall;

    [SerializeField] private Whip whip;

    private bool isAttacking;
    private float attackCooldown = 0.4f;
    private float attackTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (!(isTouchingWall && !isGrounded))
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }



        // Flip the sprite based on movement direction
        if (moveInput < 0f && transform.localScale.x > 0f ||
            moveInput > 0f && transform.localScale.x < 0f)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isTouchingWall)
        {
            Debug.Log("WallJumpCheck");
            float pushDirection = transform.localScale.x > 0 ? -1f : 1f;
            rb.linearVelocity = new Vector2(pushDirection * moveSpeed, maxJumpSpeed);           
        }
        PlayerJump();



        SetAnimation(moveInput);
        {
            healthImage.fillAmount = health / 100f;
        }

        attackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X) && attackTimer <= 0f)
        {
            Debug.Log("Whip Pressed");
            Attack();
        }
    }
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        CheckWall();

        if (isTouchingWall)
        {
            //rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.gravityScale = 0f;
            Debug.Log("WallCheck");
        }
        else
        {
            rb.gravityScale = 4f; // your normal gravity
        }

    }
    //Method used to grab ledges
    private void CheckWall()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        

        isTouchingWall = Physics2D.Raycast(
            wallCheck.position,
            direction,
            wallCheckDistance,
            groundLayer
        );

        if (isTouchingWall)
        {
            //Debug.Log("TOUCHING WALL");
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }

        Debug.DrawRay(wallCheck.position, direction * wallCheckDistance, Color.red);
    }

    private void Attack()
    {
        attackTimer = attackCooldown;

        isAttacking = true;

        whipObject.SetActive(true);

        whip.UseWhip();
    }


    public void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isTouchingWall)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpHoldTimer = 0f;

            PlaySFX(jumpClip, 0.1f);
        }


        // --- Hold to jump higher ---
        if (isJumping && Input.GetKey(KeyCode.Space))
        {
            // Keep adding upward acceleration while within hold window
            if (jumpHoldTimer < maxJumpHoldTime)
            {
                jumpHoldTimer += Time.deltaTime;

                // Add upward accel, with a clamp to avoid extreme speeds
                float newY = rb.linearVelocity.y + jumpHoldAcceleration * Time.deltaTime;
                newY = Mathf.Min(newY, maxJumpSpeed);

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);
            }
            else
            {
                // We reached the max hold time
                isJumping = false;
            }

        }

        // --- Stop extending jump if key released or if no longer going up ---
        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;


        // If the character starts falling or hits the ceiling, stop the extension

        if (Input.GetKeyUp(KeyCode.Space) || rb.linearVelocity.y <= 0f)
            isJumping = false;
    }

    public void Stomp()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        PlaySFX(jumpClip, 0.1f);
    }
    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0 || isTouchingWall)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }/*
        else
        {
            if(rb.linearVelocityY > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }*/

    }
    public void SetHealthUI(Image image)
    {
        healthImage = image;
    }


    public void TakeDamage(int amount = 1)
    {
        if (isInvulnerable)
            return;

        isInvulnerable = true;
        PlaySFX(hurtClip, 0.1f);
        health -= 25 * amount;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxJumpSpeed);
        StartCoroutine(BlinkRed());
        if (health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            TakeDamage();
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        isInvulnerable = false;
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
        //Dying is punished by restarting the game from the first level, but we haven't implemented more levels
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
    }

    private void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}

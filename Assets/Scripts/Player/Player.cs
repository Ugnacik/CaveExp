using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 100;

    //movement variables
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpHoldAcceleration = 30f;
    public float maxJumpHoldTime = 0.2f;
    public float maxJumpSpeed = 16f;


    public Transform groundCheck;
    public LayerMask groundLayer;
    public Image healthImage;

    public AudioClip jumpClip;
    public AudioClip hurtClip;

    private Rigidbody2D rb;

    private Animator animator;

    private AudioSource audioSource;

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);


        // Flip the sprite based on movement direction
        if (moveInput != 0f)
        {
            spriteRenderer.flipX = moveInput < 0f;
        }

        PlayerJump();



        SetAnimation(moveInput);
        {

            healthImage.fillAmount = health / 100f;
        }


        if(transform.position.y < -14) // fall of the map
        {
            Die();
        }
    }
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
            if (moveInput == 0)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }
        /*else
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

    public void TakeDamage(int amount = 1)
    {
        PlaySFX(hurtClip);
        health -= 25 * amount;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            PlaySFX(hurtClip);
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}

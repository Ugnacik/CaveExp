using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AudioSource audioSource;

    protected bool isWaiting;

    [SerializeField] protected float speed = -4f;
    [SerializeField] protected int contactDamage = 1;
    public int ContactDamage => contactDamage;

    public void EnemyInit()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    protected void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
    public virtual void Flip()
    {
        if (isWaiting)
            return;

        StartCoroutine(FlipDelay());
    }

    private IEnumerator FlipDelay()
    {
        isWaiting = true;

        // stop movement
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        yield return new WaitForSeconds(1f);

        // flip direction
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);

        speed *= -1;

        isWaiting = false;
    }

}

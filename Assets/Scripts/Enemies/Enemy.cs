using UnityEngine;
using UnityEngine.Audio;

public class Enemy : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AudioSource audioSource;

    [SerializeField] protected float speed = 2f;

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
    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        speed *= -1;
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    [SerializeField] private float speed = 2f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocityX = speed;
    }
    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
        speed *= -1;
    }
}

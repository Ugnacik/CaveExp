using UnityEngine;

public class GreenSnake : MonoBehaviour
{
    public float speed = 2f;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    void Update()
    {
        changeDirection();

    }


    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only respond to collisions with obstacle layers
        if (((1 << collision.gameObject.layer) & obstacleLayers) == 0)
            return;

        // If we hit something mostly from the side, reverse
        foreach (var contact in collision.contacts)
        {
            // A strong horizontal normal means a side impact
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                TurnAround();
                break;
            }
        }
    }*/
    public void changeDirection()
    {
        if (rb.linearVelocityX != 0)
        {
            if (rb.linearVelocityX > 0)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}

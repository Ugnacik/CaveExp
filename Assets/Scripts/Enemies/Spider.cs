using UnityEngine;

public class Spider : Enemy
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    void Start()
    {
        EnemyInit();
        speed = 4f;
        isWaiting = false;
    }
    private void FixedUpdate()
    {
        if (isWaiting)
        {
            animator.Play("Spider_Idle");
            return;
        }
        animator.Play("Spider_Run");
        rb.linearVelocityX = speed;
        CheckLedgeAndWall();
    }
    private void CheckLedgeAndWall()
    {
        Vector2 direction = speed > 0 ? Vector2.right : Vector2.left;

        Vector2 origin = (Vector2)groundCheck.position + direction * groundCheckDistance;

        bool noGroundAhead = !Physics2D.Raycast(
            origin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        bool wallAhead = Physics2D.Raycast(
            origin,
            direction,
            0.01f,
            groundLayer
        );

        if (noGroundAhead || wallAhead)
        {
            Flip();
        }
    }
}

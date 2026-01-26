using UnityEngine;

public class CaveMan : Enemy
{
    private GameObject player;
    private bool runTrigger;

    //CaveMan is not supposed to stop when hitting a ledge, only a wall
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    void Start()
    {
        EnemyInit();
        speed = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        runTrigger = false;
    }
    void FixedUpdate()
    {
        if (!runTrigger)
        {
            if (Vector2.Distance(transform.position, player.transform.position) <= 4f)
            {
                if (transform.position.x > player.transform.position.x)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y); ;
                    speed *= -1;
                }
                runTrigger = true;
            }
        }
        else
        {
            if (isWaiting)
            {
                animator.Play("CaveMan_Idle");
                return;
            }
            animator.Play("CaveMan_Run");
            rb.linearVelocityX = speed;
            CheckWall();
        }
    }

    private void CheckWall()
    {
        Vector2 direction = speed > 0 ? Vector2.right : Vector2.left;

        Vector2 origin = (Vector2)wallCheck.position + direction * wallCheckDistance;

        bool wallAhead = Physics2D.Raycast(
            origin,
            direction,
            0.01f,
            groundLayer
        );

        if (wallAhead)
        {
            Flip();
        }
    }

}

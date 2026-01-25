using Unity.VisualScripting;
using UnityEngine;

public class Snake : Enemy
{
    void Start()
    {
        EnemyInit();
        animator.Play("Snake_Walk");
    }
    private void FixedUpdate()
    {
        rb.linearVelocityX = speed;
    }
}

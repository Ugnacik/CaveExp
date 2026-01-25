using UnityEngine;

public class CaveMan : Enemy
{
    private GameObject player;
    private bool runTrigger;

    void Start()
    {
        EnemyInit();
        player = GameObject.FindGameObjectWithTag("Player");
        runTrigger = false;
    }
    void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= 4f)
        {
            runTrigger = true;
            animator.Play("CaveMan_Run");
        }
        if (!runTrigger)
            return;
        if (runTrigger)
            rb.linearVelocityX = speed;
    }
}

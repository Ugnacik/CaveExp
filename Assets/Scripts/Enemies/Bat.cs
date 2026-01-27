using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bat : Enemy
{
    public bool chase = false;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = 3f;
        EnemyInit();
    }

    void FixedUpdate()
    {
        if (!chase)
        {
            if (Vector2.Distance(transform.position, player.transform.position) <= 4f)
            {
                animator.Play("Bat_Fly");
                chase = true;
            }
        }
        else
        {
            Chase();
            Flip();
        }
    }
    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
    public override void Flip()
    {
        
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}

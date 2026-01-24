using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bat : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 6;
    public float updateRate = 0.2f;


    //[SerializeField] private Transform _targetToMoveTowards;
    private Transform player;
    private NavMeshAgent agent;
    private float nextUpdateTime;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindWithTag("Player").transform;
    }
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if(Time.time >= nextUpdateTime)
            {
                agent.SetDestination(player.position);
                nextUpdateTime = Time.time + updateRate;
            }
        }
        // Will stop moving if the player is too far
        /*else
        {
            agent.ResetPath();
        }*/

        changeDirection();
    }

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
    /*public void moveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, _targetToMoveTowards.position, speed * Time.deltaTime);
    }*/
}

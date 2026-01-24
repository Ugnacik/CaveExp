using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bat : MonoBehaviour
{
    public float speed;
    public bool chase = false;
    public Transform startingPoint;
    private GameObject player;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= 4f)
        {
            animator.Play("Bat_Fly");
            chase = true;
        }
        if (chase == true)
            Chase();
        Flip();
    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime); 
    }

    private void Flip()
    {
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}

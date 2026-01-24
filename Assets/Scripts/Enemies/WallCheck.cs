using UnityEngine;
using UnityEngine.Tilemaps;

public class WallCheck : MonoBehaviour
{
    private Snake snake;

    private void Awake()
    {
        snake = GetComponentInParent<Snake>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            snake.Flip();
        }
    }
}

using UnityEngine;

public class EnemyHeadCheck : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<StompCheck>())
        {
            player.Stomp();
        }
    }
}

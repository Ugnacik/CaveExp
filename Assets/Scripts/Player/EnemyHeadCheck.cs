using UnityEngine;

public class EnemyHeadCheck : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var stomp = collision.GetComponent<StompCheck>();
        if (stomp)
        {
            stomp.Die();
            player.Stomp();
        }
    }
}

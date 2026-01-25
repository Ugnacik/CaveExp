using UnityEngine;

public class EnemyCheck : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogError("EnemyCheck has no Player parent!");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            player.TakeDamage();
            Debug.Log("TakeDamage");
        }
    }
}

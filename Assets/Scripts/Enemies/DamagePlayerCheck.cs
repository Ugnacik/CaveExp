using UnityEngine;

public class DamagePlayerCheck : MonoBehaviour
{

    [SerializeField] private Player player;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyCheck>())
        {
            player.TakeDamage();
        }
    }

}

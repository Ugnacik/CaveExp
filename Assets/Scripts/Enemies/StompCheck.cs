using UnityEngine;

public class StompCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyHeadCheck>())
        {
            Destroy(transform.parent.gameObject);
        }
    }
}

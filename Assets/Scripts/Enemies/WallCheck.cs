using UnityEngine;
using UnityEngine.Tilemaps;

public class WallCheck : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            transform.parent.localScale *= -1;
        }
    }
}

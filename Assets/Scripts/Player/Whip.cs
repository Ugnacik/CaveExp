using UnityEngine;

public class Whip : MonoBehaviour
{
    private float activeTime = 0.5f;
    private float timer;

    private void OnEnable()
    {
        timer = activeTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(1); // implement in Enemy
        }
    }
}

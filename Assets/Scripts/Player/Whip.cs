using UnityEngine;

public class Whip : MonoBehaviour
{
    private float activeTime = 0.5f;
    private float timer;

    private Animator animator;
    private Collider2D whipCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        whipCollider = GetComponent<Collider2D>();
        whipCollider.enabled = false;
    }

    public void UseWhip()
    {
        animator.Play("Whip_Attack");
    }
    // Called via Animation Event
    public void EnableHitbox()
    {
        whipCollider.enabled = true;
    }

    // Called via Animation Event
    public void DisableHitbox()
    {
        whipCollider.enabled = false;
    }
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

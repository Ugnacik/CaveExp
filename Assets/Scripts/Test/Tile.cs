using UnityEngine;

public class Tile : MonoBehaviour
{
    // Tile size in pixels (Spelunky default)
    public const int Width = 16;
    public const int Height = 16;

    [Header("Tile Settings")]
    [Range(0, 100)]
    public int spawnProbability = 100;

    [HideInInspector] public int x;
    [HideInInspector] public int y;
    [HideInInspector] public bool debug;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeTile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetupTile()
    {
        // Later: choose sprite based on neighbors
        // For now, this is a stub
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

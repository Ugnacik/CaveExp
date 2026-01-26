using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Openings")]
    public bool top;
    public bool right;
    public bool down;
    public bool left;

    [HideInInspector] public Vector2Int Index;
    [HideInInspector] public bool debug;

    private Tile[] tiles;

    private void Awake()
    {
        tiles = GetComponentsInChildren<Tile>();
    }

    public Tile GetSuitableEntranceOrExitTile()
    {
        // Find the top-most solid tile
        Tile bestTile = null;
        int highestY = int.MinValue;

        foreach (Tile tile in tiles)
        {
            if (tile.transform.position.y > highestY)
            {
                highestY = (int)tile.transform.position.y;
                bestTile = tile;
            }
        }

        return bestTile;
    }
}

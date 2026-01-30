using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Header("Room Grid Position")]
    public Vector2Int GridIndex { get; private set; }

    [Header("Room Openings")]
    public bool top;
    public bool right;
    public bool down;
    public bool left;

    [Header("Tilemap")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TilemapRenderer tilemapRenderer;

    [Header("Connections (runtime)")]
    public bool ConnectTop { get; private set; }
    public bool ConnectRight { get; private set; }
    public bool ConnectDown { get; private set; }
    public bool ConnectLeft { get; private set; }

    [System.NonSerialized]
    public bool IsMainPath;


    private void Awake()
    {
        // Reset runtime-only state
        IsMainPath = false;
        ConnectTop = false;
        ConnectRight = false;
        ConnectDown = false;
        ConnectLeft = false;
    }

    public void ConnectTo(Vector2Int direction)
    {
        if (direction == Vector2Int.up) ConnectTop = true;
        if (direction == Vector2Int.right) ConnectRight = true;
        if (direction == Vector2Int.down) ConnectDown = true;
        if (direction == Vector2Int.left) ConnectLeft = true;
    }

    public void GenerateInterior(System.Random rng)
    {
        if (groundTilemap == null)
            return;

        // Skip if main path (optional)
        if (IsMainPath)
            return;

        // 50% chance
        if (rng.NextDouble() > 0.5)
            return;

        int width = LevelGenerator.RoomWidth;
        int height = LevelGenerator.RoomHeight;

        int platformWidth = rng.Next(3, 7);
        int startX = rng.Next(1, width - platformWidth - 1);
        int y = rng.Next(3, height - 3);

        for (int i = 0; i < platformWidth; i++)
        {
            Vector3Int pos = new Vector3Int(startX + i, y, 0);

            if (!groundTilemap.HasTile(pos))
            {
                // Use existing border tile as platform tile
                groundTilemap.SetTile(pos,
                    groundTilemap.GetTile(new Vector3Int(0, 0, 0)));
            }
        }

        groundTilemap.RefreshAllTiles();
    }

    public void CarveDoors()
    {
        if (groundTilemap == null)
            return;

        int width = LevelGenerator.RoomWidth;   // 10
        int height = LevelGenerator.RoomHeight; // 8

        int centerX = width / 2;   // 5
        int centerY = height / 2;  // 4

        int doorHalfWidth = 1; // 3 tiles wide

        // TOP
        if (ConnectTop)
        {
            for (int i = -doorHalfWidth; i <= doorHalfWidth; i++)
            {
                Vector3Int pos = new Vector3Int(centerX + i, height - 1, 0);
                groundTilemap.SetTile(pos, null);
            }
        }

        // DOWN
        if (ConnectDown)
        {
            for (int i = -doorHalfWidth; i <= doorHalfWidth; i++)
            {
                Vector3Int pos = new Vector3Int(centerX + i, 0, 0);
                groundTilemap.SetTile(pos, null);
            }
        }

        // LEFT
        if (ConnectLeft)
        {
            for (int i = -doorHalfWidth; i <= doorHalfWidth; i++)
            {
                Vector3Int pos = new Vector3Int(0, centerY + i, 0);
                groundTilemap.SetTile(pos, null);
            }
        }

        // RIGHT
        if (ConnectRight)
        {
            for (int i = -doorHalfWidth; i <= doorHalfWidth; i++)
            {
                Vector3Int pos = new Vector3Int(width - 1, centerY + i, 0);
                groundTilemap.SetTile(pos, null);
            }
        }

        groundTilemap.RefreshAllTiles();
    }


    public void MarkAsMainPath()
    {
        IsMainPath = true;
        //tilemapRenderer.material.color = Color.green;
    }

    public void SetGridIndex(int x, int y)
    {
        GridIndex = new Vector2Int(x, y);
    }

    public Tilemap GetGroundTilemap()
    {
        return groundTilemap;
    }
    public void Clear()
    {
        groundTilemap.ClearAllTiles();
    }

    public void SetTile(Vector3Int position, TileBase tile)
    {
        groundTilemap.SetTile(position, tile);
    }

    public Vector3Int GetEntranceTile()
    {
        // top-center by default
        return new Vector3Int(
            LevelGenerator.RoomWidth / 2,
            LevelGenerator.RoomHeight - 1,
            0
        );
    }
    public Vector3 GetWorldPositionOfCell(Vector3Int cell)
    {
        return groundTilemap.CellToWorld(cell) + groundTilemap.tileAnchor;
    }

    public Vector3Int GetEntranceCell()
    {
        return new Vector3Int(
            LevelGenerator.RoomWidth / 2,
            LevelGenerator.RoomHeight - 1,
            0
        );
    }
    public void UpdateDebugColor()
    {
        if (IsMainPath)
        {
            tilemapRenderer.material.color = Color.green;
            return;
        }

        if (ConnectTop || ConnectRight || ConnectDown || ConnectLeft)
        {
            tilemapRenderer.material.color = Color.red;
            return;
        }

        tilemapRenderer.material.color = Color.white;
    }


    /*private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.cyan;

        Vector3 center = transform.position +
            new Vector3(
                LevelGenerator.RoomWidth * 0.5f,
                LevelGenerator.RoomHeight * 0.5f,
                0f
            );

        if (ConnectTop)
            Gizmos.DrawLine(center, center + Vector3.up * 3f);
        if (ConnectRight)
            Gizmos.DrawLine(center, center + Vector3.right * 3f);
        if (ConnectDown)
            Gizmos.DrawLine(center, center + Vector3.down * 3f);
        if (ConnectLeft)
            Gizmos.DrawLine(center, center + Vector3.left * 3f);
    }*/

}

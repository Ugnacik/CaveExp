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

    public bool IsMainPath { get; private set; }

    private void OnDrawGizmos()
    {
        
        if (!Application.isPlaying)
            return;
        
        if (IsMainPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(
                transform.position + new Vector3(5f, 4f, 0f),
                new Vector3(10f, 8f, 0.1f)
            );
        }
    }


    public void MarkAsMainPath()
    {
        IsMainPath = true;
        tilemapRenderer.material.color = Color.green;
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

}

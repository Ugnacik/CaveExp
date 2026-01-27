using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Header("Room Openings")]
    public bool top;
    public bool right;
    public bool down;
    public bool left;

    [Header("Tilemap")]
    [SerializeField] private Tilemap groundTilemap;

    public void Clear()
    {
        groundTilemap.ClearAllTiles();
    }

    public void SetTile(Vector3Int position, TileBase tile)
    {
        groundTilemap.SetTile(position, tile);
    }

    public Tilemap GetGroundTilemap()
    {
        return groundTilemap;
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

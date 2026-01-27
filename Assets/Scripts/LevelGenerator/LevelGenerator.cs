using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int roomsHorizontal = 4;
    [SerializeField] private int roomsVertical = 3;

    [Header("Rooms")]
    [SerializeField] private Room roomPrefab;

    [SerializeField] private Transform roomParent;

    public const int RoomWidth = 10;
    public const int RoomHeight = 8;
    public const float TileSize = 1f;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Vector2 roomSize = new Vector2(
            RoomWidth * TileSize,
            RoomHeight * TileSize
        );

        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Vector3 position = new Vector3(
                    x * roomSize.x,
                    y * roomSize.y,
                    0f
                );

                Instantiate(roomPrefab, position, Quaternion.identity, roomParent);
            }
        }
    }
}

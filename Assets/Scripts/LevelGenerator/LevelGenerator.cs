using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int roomsHorizontal = 4;
    [SerializeField] private int roomsVertical = 3;

    [Header("Rooms")]
    [SerializeField] private Room roomPrefab;
    private Room[,] rooms;


    [SerializeField] private Transform roomParent;

    public const int RoomWidth = 10;
    public const int RoomHeight = 8;
    public const float TileSize = 1f;

    private System.Random rng = new System.Random();


    private void Start()
    {
        rooms = new Room[roomsHorizontal, roomsVertical];
        GenerateGrid();
        GenerateMainPath();
    }

    private void GenerateMainPath()
    {
        Debug.Log("Generating main path");
        int x = rng.Next(0, roomsHorizontal);
        int y = roomsVertical - 1;

        Room currentRoom = rooms[x, y];
        currentRoom.MarkAsMainPath();

        while (y > 0)
        {
            int direction = rng.Next(0, 3); // 0 = left, 1 = right, 2 = down

            int nextX = x;
            int nextY = y;

            if (direction == 0) nextX--;
            else if (direction == 1) nextX++;
            else nextY--;

            // Prevent leaving grid
            if (nextX < 0 || nextX >= roomsHorizontal)
                continue;

            x = nextX;
            y = nextY;

            currentRoom = rooms[x, y];
            currentRoom.MarkAsMainPath();
        }
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

                Room room = Instantiate(
                    roomPrefab,
                    position,
                    Quaternion.identity,
                    roomParent
                );

                room.SetGridIndex(x, y);
                rooms[x, y] = room;
            }
        }
    }

}

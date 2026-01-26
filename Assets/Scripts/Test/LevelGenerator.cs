using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private Tile entranceTilePrefab;
    [SerializeField] private Tile exitTilePrefab;

    [Header("Grid Size")]
    [SerializeField] private int roomsHorizontal = 4;
    [SerializeField] private int roomsVertical = 4;

    [Header("Rooms")]
    [SerializeField] private Room[] normalRooms;
    [SerializeField] private Room[] specialRooms;

    [Header("Parents")]
    [SerializeField] private Transform roomParent;
    [SerializeField] private Transform backgroundParent;
    [SerializeField] private Transform boundsParent;

    public const int RoomWidth = 10;
    public const int RoomHeight = 8;

    public Room[,] Rooms { get; private set; }

    private Vector2Int currentIndex;
    private Vector2Int direction;
    private Vector2Int lastDirection;

    private Room firstRoom;
    private Room lastRoom;

    private System.Random rng;

    private void Awake()
    {
        Rooms = new Room[roomsHorizontal, roomsVertical];
        rng = new System.Random(); // seedable later
    }

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        CreateMainPath();
        FillRemainingRooms();
        PlaceEntranceAndExit();
    }

    private void CreateMainPath()
    {
        currentIndex = new Vector2Int(
            rng.Next(0, roomsHorizontal),
            roomsVertical - 1
        );

        PickDirection();

        while (true)
        {
            Vector2Int nextIndex = currentIndex + direction;

            if (!IsInsideGrid(nextIndex))
            {
                direction = Vector2Int.down;
                continue;
            }

            Room roomPrefab = FindSuitableRoom(currentIndex) ?? GetRandomRoom();
            Room roomInstance = SpawnRoom(roomPrefab, currentIndex);

            if (firstRoom == null)
                firstRoom = roomInstance;

            if (nextIndex.y < 0)
            {
                lastRoom = roomInstance;
                break;
            }

            currentIndex = nextIndex;
            PickDirection();
        }
    }

    private void FillRemainingRooms()
    {
        for (int x = 0; x < roomsHorizontal; x++)
        {
            for (int y = 0; y < roomsVertical; y++)
            {
                if (Rooms[x, y] != null)
                    continue;

                Room prefab = GetRandomRoom();
                SpawnRoom(prefab, new Vector2Int(x, y));
            }
        }
    }

    private Room SpawnRoom(Room prefab, Vector2Int index)
    {
        Vector3 worldPos = new Vector3(
            index.x * RoomWidth * Tile.Width,
            index.y * RoomHeight * Tile.Height,
            0
        );

        Room instance = Instantiate(prefab, worldPos, Quaternion.identity, roomParent);
        instance.Index = index;

        Rooms[index.x, index.y] = instance;
        return instance;
    }

    private void PickDirection()
    {
        lastDirection = direction;

        float roll = (float)rng.NextDouble();

        if (roll < 0.4f)
            direction = Vector2Int.left;
        else if (roll < 0.8f)
            direction = Vector2Int.right;
        else
            direction = Vector2Int.down;
    }

    private void PlaceEntranceAndExit()
    {
        Tile entranceTile = firstRoom.GetSuitableEntranceOrExitTile();
        Instantiate(
            entranceTilePrefab,
            entranceTile.transform.position + Vector3.up,
            Quaternion.identity
        );

        Tile exitTile = lastRoom.GetSuitableEntranceOrExitTile();
        Instantiate(
            exitTilePrefab,
            exitTile.transform.position + Vector3.up,
            Quaternion.identity
        );
    }


    private bool IsInsideGrid(Vector2Int index)
    {
        return index.x >= 0 &&
               index.x < roomsHorizontal &&
               index.y >= 0 &&
               index.y < roomsVertical;
    }

    private Room GetRandomRoom()
    {
        return normalRooms[Random.Range(0, normalRooms.Length)];
    }

    private Room FindSuitableRoom(Vector2Int index)
    {
        // TODO: later check room openings (top/right/down/left)
        return null;
    }


}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int roomsHorizontal = 4;
    [SerializeField] private int roomsVertical = 3;

    [Header("Rooms")]
    [SerializeField] private Room[] roomPrefabs;
    private Room[,] rooms;
    private Room entranceRoom;
    private Room exitRoom;

    [SerializeField] private GameObject entrancePrefab;
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private GameObject playerPrefab;




    [SerializeField] private Transform roomParent;

    public const int RoomWidth = 10;
    public const int RoomHeight = 8;
    public const float TileSize = 1f;

    private System.Random rng = new System.Random();

    private enum PathDirection
    {
        Left,
        Right,
        Down
    }



    private void Start()
    {
        rooms = new Room[roomsHorizontal, roomsVertical];

        //GenerateGrid();
        GenerateMainPath();
        FillSideRooms();
        GenerateConnections();
        CarveAllDoors();
        IdentifyEntranceAndExit();
        PlaceEntranceAndExit();
        SpawnPlayer();

        /*
        //Validation
        ValidateMainPath();
        ValidateConnections();
        ValidateIsolation();
        */
        //Debug
        UpdateRoomDebugColors();

    }

    private void GenerateMainPath()
    {
        int x = rng.Next(0, roomsHorizontal);
        int y = roomsVertical - 1;

        // Place first room
        Room startRoom = GetCompatibleRoom(PathDirection.Down);
        PlaceRoom(startRoom, x, y);
        rooms[x, y].MarkAsMainPath();

        while (y > 0)
        {
            // Optional sideways wandering
            int sidewaysMoves = rng.Next(0, 3); // 0–2 sideways moves

            for (int i = 0; i < sidewaysMoves; i++)
            {
                int direction = rng.Next(0, 2); // 0 = left, 1 = right
                int nextX = x + (direction == 0 ? -1 : 1);

                if (nextX < 0 || nextX >= roomsHorizontal)
                    continue;

                Room sideRoom = GetCompatibleRoomForMainPathSideways(
                    direction == 0 ? PathDirection.Left : PathDirection.Right
                );


                PlaceRoom(sideRoom, nextX, y);

                x = nextX;
                rooms[x, y].MarkAsMainPath();
            }

            // Force downward move
            int nextY = y - 1;

            Room downRoom = GetCompatibleRoom(PathDirection.Down);
            PlaceRoom(downRoom, x, nextY);

            y = nextY;
            rooms[x, y].MarkAsMainPath();
        }
    }
    private void SpawnPlayer()
    {
        if (entranceRoom == null)
            return;

        Vector3 spawnPosition = GetSafeSpawnPosition(entranceRoom);

        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }
    private Vector3 GetSafeSpawnPosition(Room room)
    {
        Tilemap tilemap = room.GetGroundTilemap();

        int width = RoomWidth;
        int height = RoomHeight;

        int centerX = width / 2;

        // Scan downward from top to find floor
        for (int y = height - 1; y >= 0; y--)
        {
            Vector3Int cell = new Vector3Int(centerX, y, 0);

            if (tilemap.HasTile(cell))
            {
                // Spawn one tile above
                Vector3 worldPos = tilemap.CellToWorld(cell);
                worldPos.y += 1f;

                return worldPos;
            }
        }

        // Fallback: room center
        return room.transform.position +
               new Vector3(width / 2f, height / 2f, 0f);
    }

    private Vector3 GetRoomCenterWorldPosition(Room room)
    {
        float roomWidth = RoomWidth;
        float roomHeight = RoomHeight;

        return room.transform.position +
               new Vector3(roomWidth / 2f, roomHeight / 2f, 0f);
    }

    private void IdentifyEntranceAndExit()
    {
        // Top row
        for (int x = 0; x < roomsHorizontal; x++)
        {
            if (rooms[x, roomsVertical - 1].IsMainPath)
            {
                entranceRoom = rooms[x, roomsVertical - 1];
                break;
            }
        }

        // Bottom row
        for (int x = 0; x < roomsHorizontal; x++)
        {
            if (rooms[x, 0].IsMainPath)
            {
                exitRoom = rooms[x, 0];
                break;
            }
        }
    }
    private void PlaceEntranceAndExit()
    {
        if (entranceRoom != null)
        {
            Vector3 pos = GetRoomCenterWorldPosition(entranceRoom);
            Instantiate(entrancePrefab, pos, Quaternion.identity);
        }

        if (exitRoom != null)
        {
            Vector3 pos = GetRoomCenterWorldPosition(exitRoom);
            Instantiate(exitPrefab, pos, Quaternion.identity);
        }
    }

    private void CarveAllDoors()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                if (room != null)
                    room.CarveDoors();
            }
        }
    }

    private void ValidateMainPath()
    {
        int startCount = 0;
        int bottomCount = 0;

        for (int x = 0; x < roomsHorizontal; x++)
        {
            if (rooms[x, roomsVertical - 1].IsMainPath)
                startCount++;

            if (rooms[x, 0].IsMainPath)
                bottomCount++;
        }

        Debug.Log($"Top row main rooms: {startCount}");
        Debug.Log($"Bottom row main rooms: {bottomCount}");
    }
    private void ValidateIsolation()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                if (room == null) continue;

                bool hasConnection =
                    room.ConnectTop ||
                    room.ConnectRight ||
                    room.ConnectDown ||
                    room.ConnectLeft;

                if (!hasConnection)
                    Debug.LogWarning($"Isolated room at {x},{y}");
            }
        }
    }

    private void ValidateConnections()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                if (room == null) continue;

                if (room.ConnectRight)
                {
                    Room neighbor = rooms[x + 1, y];
                    if (neighbor == null || !neighbor.ConnectLeft)
                        Debug.LogError($"One-way connection at {x},{y} → RIGHT");
                }

                if (room.ConnectTop)
                {
                    Room neighbor = rooms[x, y + 1];
                    if (neighbor == null || !neighbor.ConnectDown)
                        Debug.LogError($"One-way connection at {x},{y} → UP");
                }
            }
        }
    }


    private void UpdateRoomDebugColors()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                
                if (room != null)
                {
                    room.UpdateDebugColor();
                    //Debug.Log($"Room {room.GridIndex} | MainPath={room.IsMainPath}");
                }    
            }
        }
    }

    

    private Room GetCompatibleRoomForMainPathSideways(PathDirection direction)
    {
        List<Room> candidates = new List<Room>();

        foreach (Room room in roomPrefabs)
        {
            bool supportsDirection =
                (direction == PathDirection.Left && room.right) ||
                (direction == PathDirection.Right && room.left);

            // IMPORTANT: must also support going down later
            if (supportsDirection && room.down)
                candidates.Add(room);
        }

        if (candidates.Count == 0)
        {
            Debug.LogError("No compatible sideways main-path rooms found!");
            return roomPrefabs[0];
        }

        return candidates[rng.Next(candidates.Count)];
    }

    private Room GetCompatibleRoom(PathDirection direction)
    {
        List<Room> candidates = new List<Room>();

        foreach (Room room in roomPrefabs)
        {
            if (direction == PathDirection.Left && room.right)
                candidates.Add(room);

            if (direction == PathDirection.Right && room.left)
                candidates.Add(room);

            if (direction == PathDirection.Down && room.top)
                candidates.Add(room);
        }

        if (candidates.Count == 0)
        {
            Debug.LogError("No compatible rooms found!");
            return roomPrefabs[0];
        }

        return candidates[rng.Next(candidates.Count)];
    }

    private void FillSideRooms()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                if (rooms[x, y] != null)
                    continue;

                Room prefab = GetRandomRoom();
                PlaceRoom(prefab, x, y);
            }
        }
    }

    private void GenerateConnections()
    {
        Debug.Log($"Connecting rooms");

        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                if (room == null)
                    continue;

                // RIGHT neighbor
                if (IsInsideGrid(x + 1, y))
                {
                    Room right = rooms[x + 1, y];
                    if (right != null &&
                        room.right && right.left)
                    {
                        room.ConnectTo(Vector2Int.right);
                        right.ConnectTo(Vector2Int.left);
                    }
                }

                // UP neighbor
                if (IsInsideGrid(x, y + 1))
                {
                    Room up = rooms[x, y + 1];
                    if (up != null &&
                        room.top && up.down)
                    {
                        room.ConnectTo(Vector2Int.up);
                        up.ConnectTo(Vector2Int.down);
                    }
                }
            }
        }
    }

    private bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < roomsHorizontal &&
               y >= 0 && y < roomsVertical;
    }

    private Room GetRandomRoom()
    {
        return roomPrefabs[rng.Next(roomPrefabs.Length)];
    }

    private PathDirection GetNextDirection(int x, int y)
    {
        // Always go down if possible with high probability
        if (y > 0 && rng.NextDouble() < 0.6)
            return PathDirection.Down;

        // Otherwise go sideways (but stay in bounds)
        if (x == 0)
            return PathDirection.Right;

        if (x == roomsHorizontal - 1)
            return PathDirection.Left;

        return rng.Next(0, 2) == 0
            ? PathDirection.Left
            : PathDirection.Right;
    }


    private void PlaceRoom(Room prefab, int x, int y)
    {
        if (rooms[x, y] != null)
            return;

        Vector2 roomSize = new Vector2(
            RoomWidth * TileSize,
            RoomHeight * TileSize
        );

        Vector3 position = new Vector3(
            x * roomSize.x,
            y * roomSize.y,
            0f
        );

        Room room = Instantiate(prefab, position, Quaternion.identity, roomParent);
        room.SetGridIndex(x, y);
        rooms[x, y] = room;
    }
    /*
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
    }*/

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int roomsHorizontal = 4;
    [SerializeField] private int roomsVertical = 3;

    [Header("Rooms")]
    //[SerializeField] private Room EntranceRoom;
    [SerializeField] private Room[] mainPathRoomPrefabs;
    [SerializeField] private Room[] sideRoomPrefabs;
    [SerializeField] private Transform roomParent;
    
    private Room[,] rooms;
    private Room entranceRoom;
    private Room exitRoom;

    public const int RoomWidth = 16;
    public const int RoomHeight = 12;

    [SerializeField] private GameObject entrancePrefab;
    [SerializeField] private GameObject exitPrefab;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;


    [Header("Enemies")]
    [SerializeField] private GameObject[] enemyPrefabs;
    //[SerializeField] private float enemySpawnChance = 0.5f;
    [SerializeField] private int maxEnemiesPerRoom = 3;

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
        GenerateInteriors();
        IdentifyEntranceAndExit();
        PlaceEntranceAndExit();

        GameManager.Instance.SpawnPlayerAtEntrance(entranceRoom);
        SpawnEnemies();


        /*
        //Validation
        ValidateMainPath();
        ValidateConnections();
        ValidateIsolation();
        */
        //Debug
        UpdateRoomDebugColors();

    }

    private Room GetStartMainPathRoom()
    {
        List<Room> candidates = new List<Room>();

        foreach (Room prefab in mainPathRoomPrefabs)
        {
            // Start room must support going down
            if (prefab.down)
                candidates.Add(prefab);
        }

        return candidates[rng.Next(candidates.Count)];
    }

    private void GenerateMainPath()
    {
        int x = rng.Next(0, roomsHorizontal);
        int y = roomsVertical - 1;

        // Place first room
        Room startRoom = GetStartMainPathRoom();

        PlaceRoom(startRoom, x, y, true);
        //PlaceRoom(EntranceRoom, x, y, true);

        while (y > 0)
        {
            int sidewaysMoves = rng.Next(1, 3);

            for (int i = 0; i < sidewaysMoves; i++)
            {
                int direction = rng.Next(0, 2); // 0 left, 1 right
                int nextX = x + (direction == 0 ? -1 : 1);

                if (nextX < 0 || nextX >= roomsHorizontal)
                    continue;

                PathDirection dir =
                    direction == 0 ? PathDirection.Left : PathDirection.Right;

                Room sideRoom = GetCompatibleMainPathRoom(dir, rooms[x, y]);


                PlaceRoom(sideRoom, nextX, y, true);

                x = nextX;
            }

            // Move down
            int nextY = y - 1;

            Room currentRoom = rooms[x, y];
            Room downRoom = GetCompatibleMainPathRoom(PathDirection.Down, currentRoom);

            PlaceRoom(downRoom, x, nextY, true);

            y = nextY;
        }
    }

    private void GenerateInteriors()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                if (room != null)
                    room.GenerateInterior(rng);
            }
        }
    }

    private void SpawnEnemies()
    {
        for (int y = 0; y < roomsVertical; y++)
        {
            for (int x = 0; x < roomsHorizontal; x++)
            {
                Room room = rooms[x, y];
                if (room == null)
                    continue;

                // Skip entrance room only
                if (room == entranceRoom)
                    continue;


                // Random chance to spawn enemies
                double chance = room.IsMainPath
                    ? 0.25   // safer main path
                    : 0.6;   // more enemies in side rooms

                if (rng.NextDouble() > chance)
                    continue;


                int enemyCount = rng.Next(1, maxEnemiesPerRoom + 1);
                for (int i = 0; i < enemyCount; i++)
                {
                    SpawnEnemyInRoom(room);
                }

            }
        }
    }
    private void SpawnEnemyInRoom(Room room)
    {
        Tilemap tilemap = room.GetGroundTilemap();

        int width = RoomWidth;
        int height = RoomHeight;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            int x = rng.Next(1, width - 1);
            int y = rng.Next(1, height - 2);

            Vector3Int current = new Vector3Int(x, y, 0);
            Vector3Int below = new Vector3Int(x, y - 1, 0);
            Vector3Int above = new Vector3Int(x, y + 1, 0);

            bool hasGroundBelow = tilemap.HasTile(below);
            bool spaceFree = !tilemap.HasTile(current);
            bool spaceAboveFree = !tilemap.HasTile(above);

            if (hasGroundBelow && spaceFree && spaceAboveFree)
            {
                Vector3 worldPos = tilemap.CellToWorld(current);
                worldPos += tilemap.cellSize / 2f;

                GameObject enemyPrefab =
                    enemyPrefabs[rng.Next(enemyPrefabs.Length)];

                Instantiate(enemyPrefab, worldPos, Quaternion.identity);
                return;
            }
        }
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
            //pos.y = -0.1f;
            GameObject entranceObj = Instantiate(entrancePrefab, pos, Quaternion.identity);

            GameManager.Instance.SetEntranceTransform(entranceObj.transform);

        }

        if (exitRoom != null)
        {
            Vector3 pos = GetRoomCenterWorldPosition(exitRoom);
            //pos.y = -0.1f;
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

    //GetCompatibleRoomForMainPathSideways
    /* 
    private Room GetCompatibleRoomForMainPathSideways(PathDirection direction)
    {
        List<Room> candidates = new List<Room>();

        foreach (Room room in mainPathRoomPrefabs)
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
            return mainPathRoomPrefabs[0];
        }

        return candidates[rng.Next(candidates.Count)];
    }
    */
    private Room GetCompatibleMainPathRoom(PathDirection direction, Room currentRoom)
    {
        List<Room> candidates = new List<Room>();

        foreach (Room prefab in mainPathRoomPrefabs)
        {
            bool valid = false;

            switch (direction)
            {
                case PathDirection.Down:
                    valid = currentRoom.down && prefab.top;
                    break;

                case PathDirection.Left:
                    valid = currentRoom.left && prefab.right;
                    break;

                case PathDirection.Right:
                    valid = currentRoom.right && prefab.left;
                    break;
            }

            if (valid)
                candidates.Add(prefab);
        }

        if (candidates.Count == 0)
        {
            Debug.LogError("No compatible main path rooms found!");
            return mainPathRoomPrefabs[0];
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
                PlaceRoom(prefab, x, y, false);
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
        return sideRoomPrefabs[rng.Next(sideRoomPrefabs.Length)];
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


    private void PlaceRoom(Room prefab, int x, int y, bool isMainPath)
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

        // Prevent identical prefab next to left neighbor
        if (x > 0 && rooms[x - 1, y] != null &&
            rooms[x - 1, y].name.Contains(prefab.name))
        {
            int safety = 0;
            while (safety < 10)
            {
                prefab = isMainPath
                    ? mainPathRoomPrefabs[rng.Next(mainPathRoomPrefabs.Length)]
                    : sideRoomPrefabs[rng.Next(sideRoomPrefabs.Length)];

                if (!rooms[x - 1, y].name.Contains(prefab.name))
                    break;

                safety++;
            }
        }

        Room room = Instantiate(prefab, position, Quaternion.identity, roomParent);

        room.SetGridIndex(x, y);

        if (isMainPath)
            room.MarkAsMainPath();

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

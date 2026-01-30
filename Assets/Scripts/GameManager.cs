using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("UI")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private GameObject winScreen;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera virtualCamera;

    private GameObject currentPlayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnPlayerAtEntrance(Room entranceRoom)
    {
        Vector3 spawnPosition = GetSafeSpawnPosition(entranceRoom);

        currentPlayer =
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Assign health UI
        Player player = currentPlayer.GetComponent<Player>();
        player.SetHealthUI(healthBarImage);

        // Assign camera follow
        virtualCamera.Follow = currentPlayer.transform;
    }

    private Vector3 GetSafeSpawnPosition(Room room)
    {
        Tilemap tilemap = room.GetGroundTilemap();

        int width = LevelGenerator.RoomWidth;
        int height = LevelGenerator.RoomHeight;

        int centerX = width / 2;

        for (int y = height - 1; y >= 0; y--)
        {
            Vector3Int cell = new Vector3Int(centerX, y, 0);

            if (tilemap.HasTile(cell))
            {
                Vector3 worldPos = tilemap.CellToWorld(cell);
                worldPos.y += tilemap.cellSize.y;
                return worldPos;
            }
        }

        return room.transform.position;
    }

    public void WinLevel()
    {
        winScreen.SetActive(true);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

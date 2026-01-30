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
    private Transform entranceTransform;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetEntranceTransform(Transform entrance)
    {
        entranceTransform = entrance;
    }
    public void SpawnPlayerAtEntrance(Room entranceRoom)
    {
        if (entranceTransform == null)
            return;

        currentPlayer =
            Instantiate(playerPrefab, entranceTransform.position, Quaternion.identity);

        // Assign health UI
        Player player = currentPlayer.GetComponent<Player>();
        player.SetHealthUI(healthBarImage);

        // Assign camera follow
        virtualCamera.Follow = currentPlayer.transform;
    }

    private Vector3 GetEntranceSpawnPosition(Room room)
    {

        float roomWidth = 16;
        float roomHeight = 12;

        return room.transform.position +
               new Vector3((roomWidth / 2f) + 1.5f, roomHeight / 2f, 0f);
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

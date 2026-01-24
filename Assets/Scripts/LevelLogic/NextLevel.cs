using UnityEngine;

public class NextLevel : MonoBehaviour
{
    public string nextLevelName;
    public void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
        Time.timeScale = 1; // game would be still restarted without this line
    }
}

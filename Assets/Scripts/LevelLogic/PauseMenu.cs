using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool isPaused = false;

    public GameObject container;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            container.SetActive(isPaused);
            Time.timeScale = 1;
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}

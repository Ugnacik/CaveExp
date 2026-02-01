using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Room");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

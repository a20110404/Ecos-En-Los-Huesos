using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string startScene;

    public void startGame()
    {
        SceneManager.LoadScene(startScene);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}

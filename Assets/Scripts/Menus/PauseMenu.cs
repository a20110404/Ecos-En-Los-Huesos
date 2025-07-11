using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string levelSelect, mainMenu;
    public GameObject pauseMenuUI;
    public bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu"))
        {
            PauseUnPause();
        }

    }


    public void PauseUnPause()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        }
        else
        {
            isPaused = true;    
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene(levelSelect);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}

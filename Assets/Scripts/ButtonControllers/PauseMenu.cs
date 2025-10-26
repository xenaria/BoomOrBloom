using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject startScreen;
    public GameObject changeModeScreen;

    public bool isPaused;

    public void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(false);
        startScreen.SetActive(true);
    }
    
    public void ChangeModeScreen()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(false);
        changeModeScreen.SetActive(true);
    }
}

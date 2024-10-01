using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    public GameObject pauseMenuUI; 

    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Pause the game's time
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume the game's time
    }

    public void LoadStartScreen()
    {
        Time.timeScale = 1f; // Ensure game speed is reset
        SceneManager.LoadScene("Start Screen"); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }


    public void ResumeGame()
    {
        paused = false;
        pauseMenu.SetActive(paused);
        pauseButton.SetActive(!paused);

        Time.timeScale = 1;
    }

    public void PuaseGame()
    {
        paused = true;
        pauseMenu.SetActive(paused);
        pauseButton.SetActive(!paused);

        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool gameIsPaused = false;
    private bool optionIsOpen = false;
    public GameObject pauseMenuUI;
    public GameObject optionMenuUI;
    GameObject player;
    private void Start() {
        player = GameObject.Find("Player");
    }
    void Update() {
        if (!player.GetComponent<Player_Move>().readingSign && Input.GetKeyDown(KeyCode.Escape))
            if (gameIsPaused)
                if (optionIsOpen) {
                    optionIsOpen = !optionIsOpen;
                    pauseMenuUI.SetActive(true);
                    optionMenuUI.SetActive(false);
                }
                else
                    Resume();
            else
                Pause();
    }
    public bool isPaused() {
        return gameIsPaused;
    }
    private void Pause() {
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume() {
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Options() {
        optionIsOpen = !optionIsOpen;
    }
    public void Restart() {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }
    public void Quit() {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

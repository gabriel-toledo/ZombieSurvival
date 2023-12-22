using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject Player;
    public static bool isPaused;
    private CameraController camController = null;

    // Update is called once per frame
    void Start()
    {
        GetReferences();
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        camController.UnlockCursor();
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        camController.LockCursor();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void GetReferences()
    {
        camController = GetComponentInChildren<CameraController>();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject hudCanvas = null;
    [SerializeField] private GameObject endCanvas = null;
    [SerializeField] private GameObject PauseCanvas = null;
    [SerializeField] private Text score;
    [SerializeField] private MainGameController gameController;

    public bool isPaused = false;
    private CameraController camController = null;
    private PlayerStats stats = null;

    private void Start()
    {
        GetReferences();
        SetActiveHud(true);
    }

    private void Update()
    {

        if(!stats.IsDead())
        {
            if((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !isPaused)
                SetActivePause(true);
            else if((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && isPaused)
                SetActivePause(false);
        }
    }

    public void SetActiveHud(bool state)
    {
        hudCanvas.SetActive(state);
        endCanvas.SetActive(!state);
        if(!state)
        {
            hudCanvas.SetActive(true);
            camController.UnlockCursor();
            Time.timeScale = 1;
        }

        if(!stats.IsDead())
            PauseCanvas.SetActive(!state);
    }

    public void SetActivePause(bool state)
    {
        hudCanvas.SetActive(!state);
        PauseCanvas.SetActive(state);

        Time.timeScale = state ? 0 : 1;
        if(state)
            camController.UnlockCursor();
        else
            camController.LockCursor();
        isPaused = state;
    }

    public void Resume()
    {
        hudCanvas.SetActive(true);
        PauseCanvas.SetActive(false);

        Time.timeScale = 1;
        camController.LockCursor();
        isPaused = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ReturnMenu()
    {
        if (isPaused) 
        { 
            isPaused = false;
            Time.timeScale = 1;
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void GetReferences()
    {
        camController = GetComponentInChildren<CameraController>();
        stats = GetComponent<PlayerStats>();
    }

}

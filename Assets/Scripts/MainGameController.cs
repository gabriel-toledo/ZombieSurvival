using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
    public int enemiesKilled = 0;
    public TextMeshProUGUI killCount;

    public void CountEnemy(int amount)
    {
        enemiesKilled += amount;
    }

    void Update()
    {
        if(killCount != null)
            killCount.SetText("Enemies killed: " + enemiesKilled);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");   
    }

    public void QuitGame()
    {
        Application.Quit();   
    }
}

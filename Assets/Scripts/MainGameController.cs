using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
    public int enemiesKilled = 0;
    public Text killCount;

    public void CountEnemy(int amount)
    {
        enemiesKilled += amount;
    }

    void Update()
    {
        if(killCount != null)
            killCount.text = "Enemies killed: " + enemiesKilled;
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

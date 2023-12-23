using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    public GameObject[] ObjectToSpawn;
    public Transform[] SpawnPositions;
    public int maxAmount;
    private int spawnedAmount = 0;
    public MainGameController gameController;

    public float minStartTime;
    public float maxStartTime;
    float timeBetween;
    void Update()
    {
        if (gameController.enemiesKilled % 10 == 0)
        {
            maxStartTime -= 0.1f * gameController.enemiesKilled / 10;
            maxAmount += 20 * gameController.enemiesKilled / 10;
        }
        if (timeBetween <= 0 && spawnedAmount < maxAmount + gameController.enemiesKilled)
        {
            int randomObject = Random.Range(0, ObjectToSpawn.Length);
            int randomPosition = Random.Range(0, SpawnPositions.Length);

            Instantiate(ObjectToSpawn[randomObject], SpawnPositions[randomPosition].position, transform.rotation);
            timeBetween = Random.Range(minStartTime, maxStartTime);
            spawnedAmount++;
        }
        else
            timeBetween -= Time.deltaTime;
    }
}
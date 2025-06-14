using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float timeBetweenSpawns;

    private int numOfObstacles;
    private float spawnTime;

    void Start()
    {
        numOfObstacles = obstacles.Length;
        if (numOfObstacles == 0) Destroy(gameObject);
    }
    
    void Update()
    {
        if (Time.time > spawnTime)
        {
            SpawnObstacle();
            spawnTime = Time.time + timeBetweenSpawns;
        }
    }

    void SpawnObstacle()
    {
        int obstacleToSpawnIndex = Random.Range(0, numOfObstacles);
        float spawnX = Random.Range(minX, maxX);
        float spawnY = Random.Range(minY, maxY);
        Instantiate(obstacles[obstacleToSpawnIndex], transform.position + new Vector3(spawnX, spawnY, 0), transform.rotation);
    }
}

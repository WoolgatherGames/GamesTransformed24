using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceshipminigamemanager : MonoBehaviour
{
    [SerializeField] Mini_spaceship player;
    [SerializeField] mini_enemy enemyPrefab;

    [SerializeField] Vector2 spawnBoxSize;

    public void DestroyNodes(int nodeIndex)
    {
        player.DestroyNodesFromIndex(nodeIndex);
    }

    private void Start()
    {
        //SpawnEnemy(); SpawnEnemy(); SpawnEnemy();
        SpawnAsteroid(); SpawnAsteroid();
    }

    public void SpawnEnemy()
    {
        //note ive callibrated the boundaries for the asteroids which want to spawn from outside the screen space whilst these would want to spawn from well inside the screen space
        float spawnPosX = transform.position.x + Random.Range(-spawnBoxSize.x, spawnBoxSize.x);
        float spawnPosY = transform.position.y + Random.Range(-spawnBoxSize.y, spawnBoxSize.y);
        Instantiate(enemyPrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity, transform).GetComponent<mini_enemy>().SetValues(this);
    }
    [SerializeField] GameObject asteroid;

    Vector4 boundaries { get { return new Vector4(transform.position.x - spawnBoxSize.x - 1f, transform.position.x + spawnBoxSize.x + 1f, transform.position.y - spawnBoxSize.y - 1f, transform.position.y + spawnBoxSize.y + 1f); } }

    void SpawnAsteroid()
    {
        float spawnPosX = transform.position.x;
        float spawnPosY = transform.position.y;
        float spawnDir = 0f;
        if (Random.Range(0f, 1f) > 0.5f)
        {
            bool spawnSide = (Random.Range(0f, 1f) > 0.5f);
            spawnPosX += spawnSide ? -spawnBoxSize.x : spawnBoxSize.x;
            spawnPosY += Random.Range(-spawnBoxSize.y, spawnBoxSize.y);

            spawnDir = spawnSide ? Random.Range(225f, 315f) : Random.Range(45f, 135f);
        }
        else
        {
            bool spawnSide = (Random.Range(0f, 1f) > 0.5f);
            spawnPosX += Random.Range(-spawnBoxSize.x, spawnBoxSize.x); 
            spawnPosY += spawnSide ? -spawnBoxSize.y : spawnBoxSize.y;

            spawnDir = spawnSide ? Random.Range(-50f, 50f) : Random.Range(120f, 230f);
        }

        Instantiate(asteroid, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity, transform).GetComponent<mini_asteroid>().SetValues(this, spawnDir, boundaries);
    }

    //change spawn timer so theres always 3 enemies spawned at a time, no more no less. 
    float enemySpawnTime { get { return 3f; } }
    float spawnTimer;
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= enemySpawnTime)
        {
            spawnTimer = 0f;
            SpawnAsteroid();
        }
    }
}

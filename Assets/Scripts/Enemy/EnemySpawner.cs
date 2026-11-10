using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyTankPool enemyPool;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(0.1f)] private float spawnInterval = 5f;
    [SerializeField, Min(0)] private int initialEnemies = 3;
    [SerializeField] private Vector2 enemySize = new Vector2(1f, 1f);

    [SerializeField] private Vector2 MinXYPointPos;
    [SerializeField] private Vector2 MaxXYPointPos;
    [SerializeField] private BattleField BattleField;
    private float nextSpawnTime;
    private int spawnedInitially;
    private int waitForSpawnCount = 0;
    private List<float> NextSpawnTimeQueue = new List<float>();


    public void Start()
    {
        onStartSpawn();

    }
    public void onStartSpawn()
    {
        for (int i = 0; i < initialEnemies; i++)
        {
            SpawnEnemy();
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (NextSpawnTimeQueue.Count < 1)
            return;

        if (Time.time >= NextSpawnTimeQueue[0])
        {
            if (NextSpawnTimeQueue.Count > 1)
            {
                NextSpawnTimeQueue.RemoveAt(0);
            }
            else
            {
                NextSpawnTimeQueue.Clear();
            }
            SpawnEnemy();

        }
    }

    public void SpawnEnemy()
    {
        if (enemyPool == null)
        {
            Debug.LogWarning("Assign Enemy Pool ", this);
            return;
        }

        if (BattleField.TryGetEnemySpawn(
                enemySize,
                out Vector2 position,
                out Quaternion rotation))
        {
            enemyPool.GetEnemy(position, rotation);
        }
        else
        {
            DelayEnemyRespawn();
        }

    }

    public void DelayEnemyRespawn()
    {
        nextSpawnTime = Time.time + spawnInterval;
        NextSpawnTimeQueue.Add(nextSpawnTime);
    }

    public void ReloadEnemies(GameSaveData saveData)
    {
        enemyPool.HideAll();
        NextSpawnTimeQueue.Clear();

        //saveData.enemies
        for (int i = 0; i < saveData.enemies.Length; i++)
        {
            Vector2 enemyPosition = new Vector2(
             saveData.enemies[i].position[0],
             saveData.enemies[i].position[1]
        );

            GameObject enemyItem = enemyPool.GetEnemy(enemyPosition, Quaternion.Euler(0f, 0f, saveData.enemies[i].rotation));
            ITankHealth health = enemyItem.GetComponent<ITankHealth>();
            if (health != null)
                health.SetLoadedHealth(saveData.enemies[i].lives);
        }
        if (saveData.enemies.Length < initialEnemies)
        {
            for (int i = 0; i < initialEnemies - saveData.enemies.Length; i++)
            {
                DelayEnemyRespawn();
            }
        }
    }
}


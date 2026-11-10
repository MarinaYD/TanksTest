using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyTankPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(0)] private int initialSize = 5;

    private readonly Stack<GameObject> pool = new Stack<GameObject>();
    private readonly HashSet<GameObject> pooledObjects =
        new HashSet<GameObject>();

    [SerializeField]
    private UnityEvent EnemyDieEvent;

    private bool GamePaused = false;

    private void Awake()
    {
        // Create pool enemies on Awake
        for (int i = 0; i < initialSize; i++)
        {
            CreateEnemy();
        }
    }

    private GameObject CreateEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned.", this);
            return null;
        }

        GameObject enemy = Instantiate(enemyPrefab, transform);
        enemy.SetActive(false);

        EnemyTankHealth health = enemy.GetComponent<EnemyTankHealth>();

        if (health != null)
        {
            health.SetPool(this);
        }

        pool.Push(enemy);
        pooledObjects.Add(enemy);

        return enemy;
    }

    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject enemy = null;

        //Looking for inactive enemy
        while (pool.Count > 0 && enemy == null)
        {
            enemy = pool.Pop();
            pooledObjects.Remove(enemy);
        }

        // If enemy pool is empty  - create new enemy 
        if (enemy == null)
        {
            enemy = CreateEnemy();

            if (enemy == null)
                return null;

            pool.Pop();
            pooledObjects.Remove(enemy);
        }

        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.SetActive(true);

        return enemy;
    }

    public void ReturnToPool(GameObject enemy)
    {
        if (enemy == null || pooledObjects.Contains(enemy))
            return;

        enemy.SetActive(false);

        pool.Push(enemy);
        pooledObjects.Add(enemy);
        if (!GamePaused)
            EnemyDieEvent.Invoke();

    }

    internal void HideAll()
    {
        for (int i = 0; i < transform.childCount; i++)
        {

            GameObject poolItem = transform.GetChild(i).gameObject;
            if (poolItem.activeSelf)
            {
                ReturnToPool(poolItem);
            }
        }
    }

    internal EnemySaveData[] GetActiveEnemies()
    {
        List<EnemySaveData> activeEnemiesList = new List<EnemySaveData>();

        for (int i = 0; i < transform.childCount; i++)
        {

            GameObject poolItem = transform.GetChild(i).gameObject;
            if (poolItem.activeSelf)
            {
                EnemySaveData item = new EnemySaveData();
                item.position = new float[] { poolItem.transform.position.x, poolItem.transform.position.y };

                item.rotation = poolItem.GetComponent<Rigidbody2D>() ? poolItem.GetComponent<Rigidbody2D>().rotation : 0f;


                if (poolItem.GetComponent<ITankHealth>() != null)
                {
                    item.lives = poolItem.GetComponent<ITankHealth>().GetCurrentHealth;
                }
                activeEnemiesList.Add(item);
            }
        }

        EnemySaveData[] activeEnemies = new EnemySaveData[activeEnemiesList.Count];

        for (int i = 0; i < activeEnemiesList.Count; i++)
        {
            activeEnemies[i] = activeEnemiesList[i];
        }

        return activeEnemies;
    }
}



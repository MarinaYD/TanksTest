using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPlayer;
    [SerializeField] private EnemyTankPool enemyPool;
    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "saveGame.json");

    public void SaveGame()
    {
        ITankHealth playerHealth = mainPlayer.GetComponent<ITankHealth>();
        EnemySaveData[] activeEnemies = enemyPool.GetActiveEnemies();

        GameSaveData saveData = new GameSaveData
        {
            player = new PlayerSaveData
            {
                position = new float[]
                {
                    mainPlayer.transform.position.x,
                    mainPlayer.transform.position.y
                },

                rotation = mainPlayer.GetComponent<Rigidbody2D>() ? mainPlayer.GetComponent<Rigidbody2D>().rotation : 0f,//mainPlayer.transform.rotation.w,
                lives = playerHealth.GetCurrentHealth,
                //killedEnemies = 0,
                // loses = 1               
            },
            enemies = activeEnemies
        };

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"Game saved: {SavePath}");
    }


    public GameSaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Save file not found!");
            return null;
        }

        string json = File.ReadAllText(SavePath);

        return JsonUtility.FromJson<GameSaveData>(json);
    }
}
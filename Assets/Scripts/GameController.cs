using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public bool gameOnPause = false;
    [SerializeField]
    private GameObject pauseUIMenu;

    [SerializeField] private SaveManager saveManager;
    [SerializeField] private EnemyTankPool enemyTankPool;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerTankController playerTankController;
    [SerializeField] private GameObject PlayerGO;
    [SerializeField] private BattleField GameBatleFiled;
    [SerializeField] private Vector2 enemySize = new Vector2(1f, 1f);
    // Start is called before the first frame update

    private bool respawnPlayer = false;
    private float nextTimeRespawn = -1f;
    [SerializeField]
    private float SpawnInterval = 5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        BindUi();
    }
    void Start()
    {
        if (pauseUIMenu != null)
        {
            pauseUIMenu.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    void BindUi()
    {
        if (pauseUIMenu == null)
        {
            return;
        }

        Button[] buttons = pauseUIMenu.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            if (button.name == "ResumeButton")
            {
                button.onClick.AddListener(Resume);
            }
            else if (button.name == "SaveButton")
            {
                button.onClick.AddListener(SaveGame);
            }
            else if (button.name.StartsWith("LoadButton"))
            {
                button.onClick.AddListener(LoadGame);
            }
            else if (button.name.StartsWith("MainMenuButton"))
            {
                button.onClick.AddListener(QuitGame);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPauseGame();

        if (!respawnPlayer)
            return;
        else if (Time.time >= nextTimeRespawn)
        {
            respawnPlayer = false;

            if (GameBatleFiled.TryGetPlayerSpawn(
                enemySize,
                out Vector2 position,
                out Quaternion rotation))
            {
                PlayerGO.transform.position = position;
                PlayerGO.transform.rotation = rotation;
                PlayerGO.SetActive(true);
            }
            else
            {
                PlayerStateUpdate(0);
            }
        }
    }

    public void EnemyStateUpdate()
    {
        enemySpawner.DelayEnemyRespawn();
    }
    public void PlayerStateUpdate(int healthState)
    {
        if (healthState == 0)
        {
            nextTimeRespawn = Time.time + SpawnInterval;
            respawnPlayer = true;
        }

    }

    public void LoadPlayer(GameObject playerGO, GameSaveData saveData)
    {
        if (saveData == null || saveData.player == null)
            return;

        Rigidbody2D rb = playerGO.GetComponent<Rigidbody2D>();
        ITankHealth playerHealth = playerGO.GetComponent<ITankHealth>();

        PlayerSaveData playerData = saveData.player;

        playerGO.transform.position = new Vector2(
            playerData.position[0],
            playerData.position[1]
        );

        playerTankController.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, playerData.rotation);

        playerHealth.SetLoadedHealth(playerData.lives);
        playerTankController.ShowPlayersHealthUpdated();
    }

    //----------------- region PauseGame Metods
    private void CheckPauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (gameOnPause)
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
        if (gameOnPause)
        {
            return;
        }

        gameOnPause = true;
        if (pauseUIMenu != null)
        {
            pauseUIMenu.SetActive(true);
        }

        FreezeGameplay(true);
        SetCursorForUi();
    }

    public void Resume()
    {
        if (!gameOnPause)
        {
            return;
        }

        gameOnPause = false;
        if (pauseUIMenu != null)
        {
            pauseUIMenu.SetActive(false);
        }

        FreezeGameplay(false);
        SetCursorForGameplay();
    }

    public void SaveGame()
    {
        saveManager.SaveGame();
    }

    public void LoadGame()
    {
        GameSaveData loadedGameState = saveManager.LoadGame();
        if (loadedGameState != null)
        {
            LoadPlayer(PlayerGO, loadedGameState);
            enemySpawner.ReloadEnemies(loadedGameState);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    void FreezeGameplay(bool freeze)
    {
        Time.timeScale = freeze ? 0f : 1f;
    }

    static void SetCursorForGameplay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    static void SetCursorForUi()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

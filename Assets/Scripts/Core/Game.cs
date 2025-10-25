using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game I;

    [Header("Core")]
    public StaticConfig config;
    public LifeTimer lifeTimer;
    public HUDController hud;
    public DeathScreen deathScreen;

    [Header("Upgrades")]
    public UpgradeDatabase upgradeDatabase;
    [HideInInspector] public UpgradeSystem upgradeSystem;

    [Header("Player")]
    public GameObject player;

    // Рантайм-переменные
    [HideInInspector] public bool GameReady;
    [HideInInspector] public int enemiesKilled = 0;
    [HideInInspector] public int totalEssenceCollected = 0;
    private float _runSeconds = 0f;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Проверка на наличие всех ключевых ссылок
        if (config == null)
        {
            Debug.LogError("Game: отсутствует StaticConfig! Перетащи его в поле Config.");
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Game: не найден игрок с тегом Player!");
                return;
            }
        }

        // Подготовка игрока
        var stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("Game: на игроке нет PlayerStats!");
            return;
        }

        // Инициализация статов из конфига
        stats.ResetFromConfig(config);
        Debug.Log("Game: PlayerStats успешно инициализирован из StaticConfig.");

        // Инициализация остальных систем
        if (lifeTimer != null)
            lifeTimer.Init(config);
        else
            Debug.LogWarning("Game: LifeTimer не назначен!");

        if (hud != null)
            hud.Init(lifeTimer);
        else
            Debug.LogWarning("Game: HUDController не назначен!");

        // Система апгрейдов
        upgradeSystem = new UpgradeSystem(upgradeDatabase, stats);
        var menu = FindObjectOfType<UpgradeMenu>(true);
        if (menu != null)
            menu.Init(stats);

        if (deathScreen != null)
            deathScreen.Hide();

        // Готово
        GameReady = true;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (GameReady)
            _runSeconds += Time.unscaledDeltaTime;
    }

    // === Публичные методы ===

    public void AddLife(float seconds)
    {
        if (lifeTimer != null)
            lifeTimer.AddTime(seconds);
    }

    public void AddEssence(int amount)
    {
        totalEssenceCollected += Mathf.Max(0, amount);
        if (hud != null)
            hud.UpdateEssence(amount);
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
    }

    public void KillPlayer()
    {
        if (!GameReady) return;
        GameReady = false;
        Time.timeScale = 0f;

        float seconds = _runSeconds;
        int baseSum = totalEssenceCollected + enemiesKilled;
        int finalScore = Mathf.RoundToInt(baseSum * seconds);

        if (deathScreen != null)
            deathScreen.Show(finalScore, seconds, totalEssenceCollected, enemiesKilled);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
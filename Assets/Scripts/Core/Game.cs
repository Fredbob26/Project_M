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

    [Header("Power-ups")]
    public PowerUpManager powerUps; // ВАЖНО: реальная ссылка (НЕ алиас)

    [HideInInspector] public bool GameReady;
    [HideInInspector] public int enemiesKilled = 0;
    [HideInInspector] public int totalEssenceCollected = 0;
    float _runSeconds = 0f;

    void Awake()
    {
        if (I == null) I = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (!config) { Debug.LogError("Game: отсутствует StaticConfig!"); return; }

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (!player) { Debug.LogError("Game: не найден Player!"); return; }
        }

        // Гарантируем PowerUpManager
        if (!powerUps)
        {
            powerUps = FindObjectOfType<PowerUpManager>(true);
            if (!powerUps) Debug.LogError("Game: на сцене нет PowerUpManager!");
        }

        var stats = player.GetComponent<PlayerStats>();
        if (!stats) { Debug.LogError("Game: на игроке нет PlayerStats!"); return; }
        stats.ResetFromConfig(config);
        Debug.Log("Game: PlayerStats успешно инициализирован из StaticConfig.");

        if (lifeTimer) lifeTimer.Init(config);
        else Debug.LogWarning("Game: LifeTimer не назначен!");

        if (hud) hud.Init(lifeTimer);
        else Debug.LogWarning("Game: HUDController не назначен!");

        upgradeSystem = new UpgradeSystem(upgradeDatabase, stats);
        var menu = FindObjectOfType<UpgradeMenu>(true);
        if (menu) menu.Init(stats);

        if (deathScreen) deathScreen.Hide();

        GameReady = true;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (GameReady) _runSeconds += Time.unscaledDeltaTime;
    }

    public void AddLife(float seconds) { if (lifeTimer) lifeTimer.AddTime(seconds); }

    public void AddEssence(int amount)
    {
        totalEssenceCollected += Mathf.Max(0, amount);
        if (hud) hud.UpdateEssence(amount);
    }

    public bool TrySpendEssence(int price)
    {
        if (!hud) return false;
        if (hud.CurrentEssence < price) return false;
        hud.UpdateEssence(-price);
        return true;
    }

    public void OnEnemyKilled() { enemiesKilled++; }

    public void KillPlayer()
    {
        if (!GameReady) return;
        GameReady = false;
        Time.timeScale = 0f;

        float seconds = _runSeconds;
        int baseSum = totalEssenceCollected + enemiesKilled;
        int finalScore = Mathf.RoundToInt(baseSum * seconds);

        if (deathScreen) deathScreen.Show(finalScore, seconds, totalEssenceCollected, enemiesKilled);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit() => Application.Quit();
}

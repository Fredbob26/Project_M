// Path: Assets/Scripts/Core/Game.cs
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

    [Header("PowerUps (опционально)")]
    public PowerUpManager buffs;

    // Рантайм-переменные
    [HideInInspector] public bool GameReady;
    [HideInInspector] public int enemiesKilled = 0;
    [HideInInspector] public int totalEssenceCollected = 0; // статистика "подобрано"
    private float _runSeconds = 0f;

    private void Awake()
    {
        if (I == null) I = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        if (config == null) { Debug.LogError("Game: нет StaticConfig"); return; }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (!player) { Debug.LogError("Game: не найден Player"); return; }
        }

        var stats = player.GetComponent<PlayerStats>();
        if (!stats) { Debug.LogError("Game: на игроке нет PlayerStats"); return; }

        // Инициализация систем
        stats.ResetFromConfig(config);
        Debug.Log("Game: PlayerStats успешно инициализирован из StaticConfig.");

        if (lifeTimer) lifeTimer.Init(config);
        if (hud) hud.Init(lifeTimer);

        upgradeSystem = new UpgradeSystem(upgradeDatabase, stats);
        var menu = FindObjectOfType<UpgradeMenu>(true);
        if (menu) menu.Init(stats);

        if (deathScreen) deathScreen.Hide();

        GameReady = true;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (GameReady) _runSeconds += Time.unscaledDeltaTime;
    }

    // ================= ПУБЛИЧНЫЕ API =================

    public void AddLife(float seconds) => lifeTimer?.AddTime(seconds);

    /// <summary>Добавить в баланс и в статистику "подобрано".</summary>
    public void AddEssence(int amount)
    {
        int a = Mathf.Max(0, amount);
        totalEssenceCollected += a;
        hud?.UpdateEssence(a);
    }

    /// <summary>Добавить ТОЛЬКО в баланс (без изменения статистики). Для ScoreFrenzy.</summary>
    public void AddBalanceOnly(int amount)
    {
        if (amount == 0) return;
        hud?.UpdateEssence(amount);
    }

    /// <summary>Текущий баланс эссенций (для апгрейдов).</summary>
    public int GetEssenceBalance()
    {
        return hud ? hud.CurrentEssence : 0;
    }

    /// <summary>Проверить и списать стоимость из баланса. Возвращает true, если успешно.</summary>
    public bool TrySpendEssence(int price)
    {
        if (price <= 0) return true;
        if (hud == null) return false;

        int balance = hud.CurrentEssence;
        if (balance < price) return false;

        hud.UpdateEssence(-price);
        return true;
    }

    public void OnEnemyKilled() => enemiesKilled++;

    public void KillPlayer()
    {
        if (!GameReady) return;
        GameReady = false;
        Time.timeScale = 0f;

        float seconds = _runSeconds;
        int baseSum = totalEssenceCollected + enemiesKilled;
        int finalScore = Mathf.RoundToInt(baseSum * seconds);

        if (deathScreen)
            deathScreen.Show(finalScore, seconds, totalEssenceCollected, enemiesKilled);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit() => Application.Quit();
}

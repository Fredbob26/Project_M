using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public PlayerScore playerScore;
    public UpgradeMenuUI menuUI;

    [Header("Upgrades Pool")]
    public UpgradeData[] allUpgrades;

    [Header("Economy")]
    public int baseCost = 100;
    public float costGrowth = 1.3f;

    [Header("State (debug)")]
    public int upgradesPurchased = 0;
    public int currentCost;

    [Header("Signals")]
    public UnityEvent onUpgradeAvailable; // можно повесить всплывашку "Нажми E"
    public UnityEvent onUpgradeSpent;

    private bool upgradeAvailable = false;

    private void Awake()
    {
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerScore == null) playerScore = FindFirstObjectByType<PlayerScore>();
        currentCost = CalcCost();
    }

    private void OnEnable()
    {
        if (playerScore != null) playerScore.onScoreChanged += HandleScoreChanged;
    }

    private void OnDisable()
    {
        if (playerScore != null) playerScore.onScoreChanged -= HandleScoreChanged;
    }

    private void Update()
    {
        // Игрок сам инициирует апгрейд (контролируем пейсинг)
        if (upgradeAvailable && Input.GetKeyDown(KeyCode.E))
        {
            OpenMenu();
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (!upgradeAvailable && newScore >= currentCost)
        {
            upgradeAvailable = true;
            onUpgradeAvailable?.Invoke();
        }
    }

    private int CalcCost() => Mathf.RoundToInt(baseCost * Mathf.Pow(costGrowth, upgradesPurchased));

    private void OpenMenu()
    {
        Time.timeScale = 0f;
        // выбираем 3 случайных апгрейда
        var picks = allUpgrades.OrderBy(_ => Random.value).Take(3).ToArray();
        menuUI.Show(picks, OnUpgradeChosen, OnMenuClosed);
    }

    private void OnUpgradeChosen(UpgradeData chosen)
    {
        // списываем очки
        playerScore.Spend(currentCost);
        onUpgradeSpent?.Invoke();

        // применяем апгрейд
        playerStats.ApplyUpgrade(chosen);

        upgradesPurchased++;
        currentCost = CalcCost();
        upgradeAvailable = playerScore.score >= currentCost;

        CloseMenu();
    }

    private void OnMenuClosed()
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        menuUI.Hide();
        Time.timeScale = 1f;
    }
}

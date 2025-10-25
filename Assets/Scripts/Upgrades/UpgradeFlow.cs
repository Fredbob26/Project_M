using UnityEngine;
using TMPro;

public class UpgradeFlow : MonoBehaviour
{
    [Header("Refs")]
    public HUDController hud;
    public UpgradeMenu upgradeMenu;
    public UpgradeDatabase upgradeDatabase;
    public PlayerStats playerStats;
    public TMP_Text hintText;

    [Header("Economy")]
    public int baseCost = 5;
    public int costStep = 5;
    public int optionsCount = 5;

    private int _nextCost;
    private bool _menuOpen;

    private void Start()
    {
        if (!hud) hud = FindObjectOfType<HUDController>(true);
        if (!upgradeMenu) upgradeMenu = FindObjectOfType<UpgradeMenu>(true);
        if (!upgradeDatabase) upgradeDatabase = Game.I.upgradeDatabase;
        if (!playerStats && Game.I.player) playerStats = Game.I.player.GetComponent<PlayerStats>();

        _nextCost = baseCost;

        if (upgradeMenu) upgradeMenu.Init(playerStats);
        if (hintText) hintText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_menuOpen) return;

        if (hud != null && hud.CurrentEssence >= _nextCost)
        {
            if (hintText) hintText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) OpenMenu();
        }
        else
        {
            if (hintText) hintText.gameObject.SetActive(false);
        }
    }

    private void OpenMenu()
    {
        _menuOpen = true;
        if (hintText) hintText.gameObject.SetActive(false);

        var defs = Game.I.upgradeSystem.GetRandomUpgrades(optionsCount);
        upgradeMenu.Show(defs, OnPick, _nextCost);
    }

    private void OnPick(UpgradeDefinition picked)
    {
        Game.I.upgradeSystem.ApplyUpgrade(picked);
        hud.UpdateEssence(-_nextCost); // списываем валюту апгрейдов
        _nextCost += costStep;
        _menuOpen = false;
    }
}
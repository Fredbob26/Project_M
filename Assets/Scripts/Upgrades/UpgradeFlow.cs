using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFlow : MonoBehaviour
{
    [Header("Refs")]
    public UpgradeDatabase database;   // <- УКАЖИ asset UpgradeDatabase
    public UpgradeMenu menu;           // <- УКАЖИ объект UpgradePanel (где висит UpgradeMenu)
    public PlayerStats stats;          // <- Можно оставить пусто: возьмёт с Game.I.player

    [Header("Economy")]
    public int basePrice = 5;
    public float priceMult = 1.35f;

    [Header("UX")]
    public KeyCode openKey = KeyCode.E;
    [Range(1, 5)] public int offerCount = 5;

    [Header("Debug")]
    public bool debugLogs = true;      // включи для диагностики
    public bool relaxHint = false;     // временно: показывать хинт, если просто хватает денег (игнорируя доступность апгрейдов)

    private int _currentPrice;
    private UpgradeSystem _system;
    private List<UpgradeDefinition> _pendingOptions; // кэш для хинта -> открытия

    private void Start()
    {
        // Подхватим PlayerStats
        if (!stats && Game.I && Game.I.player) stats = Game.I.player.GetComponent<PlayerStats>();

        if (database == null)
        {
            LogErr("[UpgradeFlow] database = NULL. Перетащи UpgradeDatabase в инспекторе.");
            return;
        }
        if (stats == null)
        {
            LogErr("[UpgradeFlow] stats = NULL. На игроке нет PlayerStats или ссылка не проставлена.");
            return;
        }

        _system = new UpgradeSystem(database, stats);
        Game.I.upgradeSystem = _system;

        _currentPrice = Mathf.Max(1, basePrice);

        if (debugLogs)
        {
            Log($"[UpgradeFlow] Init OK. basePrice={basePrice}, priceMult={priceMult}, offerCount={offerCount}");
            Log($"[UpgradeFlow] DB upgrades count = {database.upgrades?.Count ?? 0}");
        }
    }

    private void Update()
    {
        if (!Game.I || !Game.I.GameReady) return;
        if (Input.GetKeyDown(openKey))
            TryOpen();
    }

    /// <summary>
    /// Хинт вызывает это каждый кадр. Генерим и кэшируем тот же набор,
    /// который покажем при нажатии E.
    /// </summary>
    public bool CanOpenNow()
    {
        // базовые проверки
        if (!Game.I || !Game.I.GameReady) { LogDbg("CanOpenNow: Game not ready"); return false; }
        if (menu == null) { LogErr("CanOpenNow: menu == NULL (укажи UpgradePanel с UpgradeMenu)"); return false; }
        if (menu.IsOpen) { LogDbg("CanOpenNow: menu already open"); return false; }
        if (database == null) { LogErr("CanOpenNow: database == NULL"); return false; }
        if (Game.I.hud == null) { LogErr("CanOpenNow: HUD == NULL"); return false; }

        int balance = Game.I.hud.CurrentEssence;
        if (debugLogs) Log($"CanOpenNow: balance={balance}, price={_currentPrice}");

        // Если база пустая — нечего предлагать
        int dbCount = database.upgrades != null ? database.upgrades.Count : 0;
        if (dbCount <= 0)
        {
            LogErr("CanOpenNow: UpgradeDatabase пуст. Добавь UpgradeDefinition-ы.");
            _pendingOptions = null;
            return false;
        }

        // Быстрый режим: разрешить хинт просто по деньгам (для проверки цепочки)
        if (relaxHint)
        {
            bool ok = balance >= _currentPrice;
            if (debugLogs) Log($"CanOpenNow(relaxHint): {ok}");
            _pendingOptions = _system.GetRandomUpgrades(Mathf.Clamp(offerCount, 1, dbCount));
            return ok;
        }

        // Нормальный режим: нужны варианты и хотя бы ОДИН покупаемый
        var options = _system.GetRandomUpgrades(Mathf.Clamp(offerCount, 1, dbCount));
        bool anyBuyable = false;

        for (int i = 0; i < options.Count; i++)
        {
            var def = options[i];
            if (def == null) continue;

            bool capped = _system.IsCapped(def);
            bool canBuyMoney = balance >= _currentPrice;
            bool canBuy = !capped && canBuyMoney;

            if (debugLogs)
                Log($"Option[{i}]: {def.type} | capped={capped} | moneyOK={canBuyMoney} | result={canBuy}");

            if (canBuy) { anyBuyable = true; break; }
        }

        _pendingOptions = anyBuyable ? options : null;

        if (!anyBuyable && debugLogs)
            Log("CanOpenNow: нет покупаемых опций (или денег мало, или всё капнуто).");

        return anyBuyable;
    }

    private void TryOpen()
    {
        if (menu == null || database == null) return;

        // если CanOpenNow ещё не кэшировал — сделаем сейчас
        if (_pendingOptions == null)
        {
            if (!CanOpenNow()) return; // внутри заполняется причина
        }
        if (_pendingOptions == null || _pendingOptions.Count == 0)
        {
            if (debugLogs) Log("TryOpen: _pendingOptions is empty.");
            return;
        }

        if (debugLogs) Log("TryOpen: opening menu with cached options.");
        menu.Show(_pendingOptions, CanBuy, OnPick, _currentPrice);
        _pendingOptions = null; // сбросим кэш
    }

    private bool CanBuy(UpgradeDefinition def)
    {
        if (_system.IsCapped(def)) return false;
        return Game.I.hud != null && Game.I.hud.CurrentEssence >= _currentPrice;
    }

    private void OnPick(UpgradeDefinition def)
    {
        if (_system.IsCapped(def)) return;

        if (Game.I.TrySpendEssence(_currentPrice))
        {
            _system.ApplyUpgrade(def);
            StepPrice();
        }
        else
        {
            if (debugLogs) Log("OnPick: денег не хватило в момент покупки.");
        }
    }

    private void StepPrice()
    {
        int old = _currentPrice;
        _currentPrice = Mathf.Max(_currentPrice + 1, Mathf.RoundToInt(_currentPrice * priceMult));
        if (debugLogs) Log($"Price step: {old} -> {_currentPrice}");
    }

    // ---- logging helpers ----
    private void Log(string msg) { if (debugLogs) Debug.Log(msg); }
    private void LogDbg(string msg) { if (debugLogs) Debug.Log($"[DBG] {msg}"); }
    private void LogErr(string msg) { Debug.LogError(msg); }
}

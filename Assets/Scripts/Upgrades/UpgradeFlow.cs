using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFlow : MonoBehaviour
{
    [Header("Pricing")]
    [SerializeField] private int basePrice = 5;
    [SerializeField] private float priceMult = 1.35f;

    [Header("Offer")]
    [SerializeField] private int offerCount = 5;

    [Header("Input")]
    [SerializeField] private KeyCode openKey = KeyCode.E;

    [Header("Debug")]
    public bool logReasons = false;

    private UpgradeMenu _menu;
    private List<UpgradeDefinition> _cachedOffers = new();
    private int _purchaseIndex = 0;

    void Start()
    {
        _menu = FindObjectOfType<UpgradeMenu>(true);
        if (_menu == null) Debug.LogError("[UpgradeFlow] Не найден UpgradeMenu на сцене.");

        if (logReasons)
        {
            Debug.Log($"[UpgradeFlow] Init OK. basePrice={basePrice}, priceMult={priceMult}, offerCount={offerCount}");
            Debug.Log($"[UpgradeFlow] DB upgrades count = {Game.I?.upgradeDatabase?.upgrades?.Count ?? 0}");
        }
    }

    void Update()
    {
        if (Game.I == null || !Game.I.GameReady) return;
        if (Input.GetKeyDown(openKey)) TryOpen();
    }

    public void TryOpen()
    {
        if (_menu == null) return;
        if (_menu.IsOpen) return;

        if (!CanOpenNow(out var offers))
        {
            if (logReasons) Debug.Log("[UpgradeFlow] TryOpen: conditions not met.");
            return;
        }

        _cachedOffers = offers;

        int price = CurrentPrice();
        if (logReasons) Debug.Log("[UpgradeFlow] TryOpen: opening menu with cached options.");

        _menu.Show(
            _cachedOffers,
            canAfford: (def) => Game.I != null && Game.I.hud != null && Game.I.hud.CurrentEssence >= price,
            onPick: OnPick,
            price: price
        );
    }

    void OnPick(UpgradeDefinition def)
    {
        if (def == null || Game.I == null) return;

        int price = CurrentPrice();
        if (!Game.I.TrySpendEssence(price))
        {
            Debug.LogWarning("[UpgradeFlow] Недостаточно эссенции при подтверждённой покупке.");
            return;
        }

        Game.I.upgradeSystem.ApplyUpgrade(def);
        _purchaseIndex++;

        if (logReasons) Debug.Log($"[UpgradeFlow] Bought {def.type}. New priceIndex={_purchaseIndex}, nextPrice={CurrentPrice()}");
    }

    int CurrentPrice()
    {
        double p = basePrice * Math.Pow(priceMult, _purchaseIndex);
        return Mathf.Max(1, Mathf.RoundToInt((float)p));
    }

    public bool CanOpenNow(out List<UpgradeDefinition> offers)
    {
        offers = null;
        if (Game.I == null || Game.I.upgradeDatabase == null || Game.I.upgradeDatabase.upgrades == null)
        {
            if (logReasons) Debug.LogError("[UpgradeFlow] CanOpenNow: Game or UpgradeDatabase is null.");
            return false;
        }

        int price = CurrentPrice();
        int balance = Game.I.hud ? Game.I.hud.CurrentEssence : 0;
        if (logReasons) Debug.Log($"[UpgradeFlow] CanOpenNow: balance={balance}, price={price}");
        if (balance < price) return false;

        var db = Game.I.upgradeDatabase.upgrades;
        if (db.Count == 0)
        {
            if (logReasons) Debug.LogError("[UpgradeFlow] CanOpenNow: UpgradeDatabase пуст.");
            return false;
        }

        var viable = new List<UpgradeDefinition>(db.Count);
        var stats = Game.I.player.GetComponent<PlayerStats>();

        foreach (var def in db)
        {
            bool capped = false;
            if (def.type == UpgradeType.CritChance && stats.CritChance >= 1f) capped = true;
            if (def.type == UpgradeType.RicochetChance && stats.RicochetChance >= 1f) capped = true;

            bool moneyOK = balance >= price;
            bool result = (!capped) && moneyOK;

            if (logReasons)
                Debug.Log($"[UpgradeFlow] Option[{def.type}]: capped={capped} | moneyOK={moneyOK} | result={result}");
            if (result) viable.Add(def);
        }

        if (viable.Count == 0)
        {
            if (logReasons) Debug.Log("[UpgradeFlow] CanOpenNow: нет валидных опций.");
            return false;
        }

        offers = PickRandom(viable, Mathf.Min(offerCount, viable.Count));
        return true;
    }

    List<UpgradeDefinition> PickRandom(List<UpgradeDefinition> src, int count)
    {
        var pool = new List<UpgradeDefinition>(src);
        var res = new List<UpgradeDefinition>(count);
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            res.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return res;
    }
}

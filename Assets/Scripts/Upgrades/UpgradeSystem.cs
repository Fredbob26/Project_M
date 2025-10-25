using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public UpgradeType type;
    public float step;
    public int level;

    public Upgrade(UpgradeType type, float step)
    {
        this.type = type;
        this.step = step;
        level = 0;
    }
}

public class UpgradeSystem
{
    private readonly PlayerStats _stats;
    private readonly Dictionary<UpgradeType, Upgrade> _upgrades = new();

    public UpgradeSystem(UpgradeDatabase db, PlayerStats stats)
    {
        _stats = stats;

        // инициализируем из базы
        foreach (var def in db.upgrades)
        {
            if (!_upgrades.ContainsKey(def.type))
                _upgrades[def.type] = new Upgrade(def.type, def.valueStep);
        }
    }

    public void ApplyUpgrade(UpgradeDefinition def)
    {
        if (!_upgrades.TryGetValue(def.type, out var u)) return;

        // криты и рикошеты Ч максимум 100%
        if ((def.type == UpgradeType.CritChance && _stats.CritChance >= 1f) ||
            (def.type == UpgradeType.RicochetChance && _stats.RicochetChance >= 1f))
            return;

        _stats.ApplyUpgrade(def.type, def.valueStep);
        u.level++;
    }

    public int GetLevel(UpgradeType type)
    {
        return _upgrades.TryGetValue(type, out var u) ? u.level : 0;
    }

    // дл€ совместимости с UpgradeMenu, которое передаЄт UpgradeDefinition
    public int GetCurrentLevel(UpgradeDefinition def) => GetLevel(def.type);

    public List<UpgradeDefinition> GetRandomUpgrades(int count)
    {
        var db = Game.I.upgradeDatabase;
        var pool = new List<UpgradeDefinition>(db.upgrades);
        var result = new List<UpgradeDefinition>(count);

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }
}
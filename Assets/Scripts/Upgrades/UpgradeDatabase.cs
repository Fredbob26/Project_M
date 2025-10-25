using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Database")]
public class UpgradeDatabase : ScriptableObject
{
    public List<UpgradeDefinition> upgrades = new List<UpgradeDefinition>();

    public UpgradeDefinition GetDefinition(UpgradeType type)
    {
        return upgrades.Find(u => u.type == type);
    }
}